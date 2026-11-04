// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Constants;
using DoeA.Data.Helper;
using DoeA.Data.Mapper;
using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO;
using DoeA.Data.Models.DTO.Stammdaten;
using DoeA.Interfaces;
using DoeA.Validator;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace DoeA.Services.Ausschreibungen;

/// <summary>Lädt, speichert und löscht einzelne Offerten samt ihren Offertenzuständen.</summary>
public class OfferteService : BaseService<Offerte, TOfferte, OfferteValidator>, IOfferteService
{
    private readonly OfferteOffertezustandMapper _offerteOffertezustandMapper;

    public OfferteService(IErrorHandlerService errorHandlerService, IDbContextFactory<DoeAContext> dbFactory,
        OfferteMapper mapper, OfferteOffertezustandMapper offerteOffertezustandMapper, ILogger logger, IAktuellerBenutzer aktuellerBenutzer)
        : base(errorHandlerService, dbFactory, mapper, logger, aktuellerBenutzer, new OfferteValidator(), "Off")
    {
        _offerteOffertezustandMapper = offerteOffertezustandMapper;
    }

    /// <summary>Lädt alle Offerten eines Los inklusive Anbieter und chronologisch sortierten Offertenzuständen.</summary>
    public async Task<List<Offerte>> Load(Guid losId)
    {
        await using var dbContext = await DbFactory.CreateDbContextAsync();

        var offerten = await (from off in dbContext.TOfferte
                              join anb in dbContext.TAnbieter on off.OffAnbId equals anb.AnbId
                              where off.OffLosId == losId
                              select new Offerte
                              {
                                  Id = off.OffId,
                                  LosId = off.OffLosId,
                                  AnbId = off.OffAnbId,
                                  Anbieter = new Anbieter { Id = anb.AnbId, Name = anb.AnbName },
                                  ZuschlagswertChf = off.OffZuschlagswertChf ?? 0,
                                  AnzahlMitarbeiter = off.OffAnzahlMitarbeiter,
                                  Bemerkung = off.OffBemerkung ?? string.Empty,
                                  Aktiv = off.OffAktiv,
                                  InsertedDate = off.OffInsertedDate,
                                  InsertedUser = off.OffInsertedUser,
                                  ChangedDate = off.OffChangedDate,
                                  ChangedUser = off.OffChangedUser
                              }).ToListAsync();

        if (offerten.Count == 0)
        {
            return offerten;
        }

        var offerteIds = offerten.Select(o => o.Id).ToList();

        // Bezeichnung kommt aus den Stammdaten und wird für die Anzeige mitgeladen.
        var zustaende = await (from ooz in dbContext.TOfferteOffertezustand
                               join ofz in dbContext.TOffertezustand on ooz.OozOfzId equals ofz.OfzId
                               where offerteIds.Contains(ooz.OozOffId)
                               select new OfferteOffertezustand
                               {
                                   Id = ooz.OozId,
                                   OffId = ooz.OozOffId,
                                   OfzId = ooz.OozOfzId,
                                   Bezeichnung = ofz.OfzBezeichnung,
                                   Datum = ooz.OozDatum.ToDateTime(TimeOnly.MinValue),
                                   Eingereicht = ooz.OozEingereicht,
                                   VerzoegerungDurchAuftraggeber = ooz.OozVerzoegerungDurchAuftraggeber,
                                   Aktiv = ooz.OozAktiv,
                                   InsertedDate = ooz.OozInsertedDate,
                                   InsertedUser = ooz.OozInsertedUser,
                                   ChangedDate = ooz.OozChangedDate,
                                   ChangedUser = ooz.OozChangedUser
                               }).ToListAsync();

        foreach (var offerte in offerten)
        {
            offerte.Offertenzustaende = zustaende
                .Where(z => z.OffId == offerte.Id)
                .OrderBy(z => z.Datum)
                .ThenBy(z => z.InsertedDate)
                .ThenBy(z => z.Id)
                .ToList();
        }

        return offerten;
    }

    /// <summary>Speichert eine einzelne Offerte samt ihren Offertenzuständen (insert/update/delete in einem Vorgang).</summary>
    public async Task<Offerte> Save(Offerte offerte)
    {
        var isNeu = offerte.Id == Guid.Empty;
        if (!isNeu)
        {
            // Vor Validate prüfen (das stempelt ChangedDate neu). NothingChanged führt bewusst NICHT
            // zum Abbruch: die Offertenzustände werden vom Vergleich nicht erfasst und müssen
            // trotzdem synchronisiert werden.
            var status = await CheckChangesAndConcurrency<TOfferte>(offerte, "Off");
            if (status is GeneralConstants.ConcurrencyError or GeneralConstants.NotFoundError)
            {
                throw new InvalidOperationException(status);
            }
        }

        await using var dbContext = await DbFactory.CreateDbContextAsync();

        // Bei Zuständen mit Publikation ist «Eingereicht» Pflicht.
        var publikationOfzIds = await dbContext.TOffertezustand
            .Where(z => z.OfzPublikation)
            .Select(z => z.OfzId)
            .ToHashSetAsync();

        await Validate(new OfferteValidator(publikationOfzIds: publikationOfzIds), offerte, isNeu);
        var tOff = Mapper.ZuEntity(offerte);

        // Bei Verfahrensart "Freihändig" (Zuschlagsempfänger) ist pro Los genau eine Offerte erlaubt.
        if (isNeu && await IstFreihaendig(dbContext, offerte.LosId)
                  && await dbContext.TOfferte.AnyAsync(o => o.OffLosId == offerte.LosId))
        {
            throw new InvalidOperationException("ERROR_AUSSCHREIBUNG_OFFERTE_FREIHAENDIG_NUR_EINE");
        }

        if (isNeu)
        {
            dbContext.TOfferte.Add(tOff);
        }
        else
        {
            var eintrag = dbContext.Entry(tOff);
            eintrag.State = EntityState.Modified;
            SchuetzeErfassungsdaten(eintrag);
        }

        await SyncZustaende(dbContext, offerte, publikationOfzIds);

        await dbContext.SaveChangesAsync();

        return offerte;
    }

    /// <summary>Löscht eine Offerte samt ihren Offertenzuständen.</summary>
    public override async Task<Offerte> Delete(Guid id)
    {
        await using var dbContext = await DbFactory.CreateDbContextAsync();

        var zustaende = await dbContext.TOfferteOffertezustand
            .Where(z => z.OozOffId == id)
            .ToListAsync();
        dbContext.TOfferteOffertezustand.RemoveRange(zustaende);

        // Wie die Basisklasse: eine unbekannte Id ist ein Fehler und kein stiller Erfolg.
        var tOff = await dbContext.TOfferte.FindAsync(id);
        if (tOff == null)
        {
            throw new InvalidOperationException($"{GeneralConstants.NullReference}?record");
        }

        dbContext.TOfferte.Remove(tOff);
        await dbContext.SaveChangesAsync();

        return Mapper.ZuModel(tOff);
    }

    /// <summary>Gleicht die Offertenzustände einer Offerte gegen den DB-Bestand ab.</summary>
    private async Task SyncZustaende(DoeAContext dbContext, Offerte offerte, HashSet<Guid> publikationOfzIds)
    {
        var dbZustaende = await dbContext.TOfferteOffertezustand
            .Where(z => z.OozOffId == offerte.Id)
            .ToListAsync();

        var behaltenIds = offerte.Offertenzustaende.Select(z => z.Id).ToHashSet();
        dbContext.TOfferteOffertezustand.RemoveRange(dbZustaende.Where(z => !behaltenIds.Contains(z.OozId)));

        foreach (var zustand in offerte.Offertenzustaende)
        {
            zustand.OffId = offerte.Id;
            var tZustand = zustand.Id == Guid.Empty
                ? null
                : dbZustaende.FirstOrDefault(z => z.OozId == zustand.Id);

            await Validate(new OfferteOffertezustandValidator(publikationOfzIds: publikationOfzIds), zustand, tZustand == null);

            if (tZustand == null)
            {
                dbContext.TOfferteOffertezustand.Add(_offerteOffertezustandMapper.ZuEntity(zustand));
            }
            else
            {
                tZustand.OozOfzId = zustand.OfzId;
                tZustand.OozDatum = zustand.Datum.HasValue
                    ? DateOnly.FromDateTime(zustand.Datum.Value)
                    : default;
                tZustand.OozEingereicht = zustand.Eingereicht;
                tZustand.OozVerzoegerungDurchAuftraggeber = zustand.VerzoegerungDurchAuftraggeber;
                tZustand.OozChangedDate = zustand.ChangedDate;
                tZustand.OozChangedUser = zustand.ChangedUser!;
            }
        }
    }

    /// <summary>Ermittelt, ob das Los zu einer Ausschreibung der Verfahrensart "Freihändig" gehört.</summary>
    private static async Task<bool> IstFreihaendig(DoeAContext dbContext, Guid losId)
    {
        var naturalId = await (from los in dbContext.TLos
                               join aus in dbContext.TAusschreibung on los.LosAusId equals aus.AusId
                               join vea in dbContext.TVerfahrensart on aus.AusVeaId equals vea.VeaId
                               where los.LosId == losId
                               select vea.VeaNaturalId).FirstOrDefaultAsync();

        return VerfahrensartRegeln.IstZuschlagsempfaengerModus(naturalId);
    }
}
