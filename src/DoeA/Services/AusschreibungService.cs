// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Constants;
using DoeA.Data.Enums;
using DoeA.Data.Helper;
using DoeA.Data.Mapper;
using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO;
using DoeA.Data.Models.DTO.Stammdaten;
using DoeA.Interfaces;
using DoeA.Validator;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Collections.ObjectModel;
using System.Data;
using ILogger = Serilog.ILogger;

namespace DoeA.Services;

/// <summary>
/// Lädt, sucht und speichert Ausschreibungen samt ihrer direkt angehängten Daten
/// (CPV-Nummern, Sprachen/Dokumentarten, Berichtigungen) und vergibt die Ausschreibungsnummer.
/// </summary>
public class AusschreibungService : BaseService<Ausschreibung, TAusschreibung, AusschreibungValidator>, IAusschreibungService
{
    /// <summary>Bezeichnung des Loses, das jede neue Ausschreibung automatisch erhält.</summary>
    private const string ErstesLos = "1";

    private const string FehlerHatUnterprojekte = "ERROR_AUSSCHREIBUNG_HAT_UNTERPROJEKTE";
    private const string FehlerKeineVorankuendigung = "ERROR_AUSSCHREIBUNG_KEINE_VORANKUENDIGUNG";
    private const string FehlerPubliziert = "ERROR_AUSSCHREIBUNG_PUBLIZIERT_NICHT_LOESCHBAR";
    private const string RolleLogistikZentralNaturalId = "LogistikZentral";

    // AusNummer im Format JJ999 = 2-stelliges Jahr * 1000 + Laufnummer ab 0
    private const int NummerJahrFaktor = 1000;
    private const int NummerJahrModulo = 100;

    /// <summary>Speicherversuche für eine neue Ausschreibung, falls die Nummernvergabe kollidiert.</summary>
    private const int MaxNummernVersuche = 3;

    private readonly AusschreibungCpvNummerMapper _ausschreibungCpvNummerMapper;
    private readonly AusschreibungSpracheMapper _ausschreibungSpracheMapper;
    private readonly BerichtigungMapper _berichtigungMapper;

    public AusschreibungService(IErrorHandlerService errorHandlerService, IDbContextFactory<DoeAContext> dbFactory,
        AusschreibungMapper mapper, AusschreibungCpvNummerMapper ausschreibungCpvNummerMapper, AusschreibungSpracheMapper ausschreibungSpracheMapper,
        BerichtigungMapper berichtigungMapper, ILogger logger, IAktuellerBenutzer aktuellerBenutzer)
        : base(errorHandlerService, dbFactory, mapper, logger, aktuellerBenutzer, new AusschreibungValidator(), "Aus")
    {
        _ausschreibungCpvNummerMapper = ausschreibungCpvNummerMapper;
        _ausschreibungSpracheMapper = ausschreibungSpracheMapper;
        _berichtigungMapper = berichtigungMapper;
    }

    // Beide Spalten haben kein DTO-Gegenstück: AusLaufnummer vergibt GeneriereAusNummer beim Anlegen,
    // AusAbgeschlossen führt der LosService (bei RFI und Vorankündigungen Save über das Publikationsdatum). Ohne Schutz würde
    // jedes Update sie auf null/false zurücksetzen.
    protected override IEnumerable<string> GeschuetzteProperties =>
        [nameof(TAusschreibung.AusLaufnummer), nameof(TAusschreibung.AusAbgeschlossen)];

    /// <summary>
    /// Sucht Ausschreibungen über Bezeichnung, Nummer, SIMAP-Nummer, Sachbearbeiter (DöA, RD, BBL), Extern und
    /// Anbietername; ein Offertzustand schränkt nur die Anbieter-Treffer auf Offerten ein, deren aktueller Zustand er ist.
    /// Alle übrigen Parameter wirken nur, wenn sie gesetzt sind.
    /// </summary>
    public async Task<ObservableCollection<AusschreibungSuche>> Suche(int? jahr, string searchValue, DateOnly? publikationVon = null, DateOnly? publikationBis = null, bool forHauptprojekt = false, string[]? verfahrensartFilter = null, decimal? betragVon = null, decimal? betragBis = null, Guid? offertezustandId = null)
    {
        // ToLower statt Like/ILike: case-insensitiv, und %/_ im Suchbegriff wirken nicht als Platzhalter.
        var suchtext = searchValue.ToLower();

        await using var dbContext = await DbFactory.CreateDbContextAsync();
        var query = from aus in dbContext.TAusschreibung
                    join lea in dbContext.TLeistungsart on aus.AusLeaId equals lea.LeaId into leaJoin
                    from lea in leaJoin.DefaultIfEmpty()
                    join vea in dbContext.TVerfahrensart on aus.AusVeaId equals vea.VeaId into veaJoin
                    from vea in veaJoin.DefaultIfEmpty()
                    join ria in dbContext.TRolleInAusschreibung on aus.AusRiaId equals ria.RiaId into reaJoin
                    from ria in reaJoin.DefaultIfEmpty()
                    join sacDoeA in dbContext.TSachbearbeiter on aus.AusSacDoeAId equals sacDoeA.SacId into sacDoeAJoin
                    from sacDoeA in sacDoeAJoin.DefaultIfEmpty()
                    join sacRd in dbContext.TSachbearbeiter on aus.AusSacRdId equals sacRd.SacId into sacRdJoin
                    from sacRd in sacRdJoin.DefaultIfEmpty()
                    join sacBbl in dbContext.TSachbearbeiter on aus.AusSacBblId equals sacBbl.SacId into sacBblJoin
                    from sacBbl in sacBblJoin.DefaultIfEmpty()
                    join amt in dbContext.TAmt on aus.AusAmtId equals amt.AmtId into amtJoin
                    from amt in amtJoin.DefaultIfEmpty()
                    join res in dbContext.TRessort on aus.AusResId equals res.ResId into resJoin
                    from res in resJoin.DefaultIfEmpty()
                    join pra in dbContext.TProjektart on aus.AusPraId equals pra.PraId into praJoin
                    from pra in praJoin.DefaultIfEmpty()
                    join voa in dbContext.TAusschreibung on aus.AusVorankuendigungId equals voa.AusId into voaJoin
                    from voa in voaJoin.DefaultIfEmpty()
                    join rfi in dbContext.TAusschreibung on aus.AusRfiId equals rfi.AusId into rfiJoin
                    from rfi in rfiJoin.DefaultIfEmpty()
                    where (jahr == null || aus.AusJahr == jahr.Value)
                    && (publikationVon == null || aus.AusPublikation >= publikationVon)
                    && (publikationBis == null || aus.AusPublikation <= publikationBis)
                    && (
                        aus.AusBezeichnung!.ToLower().Contains(suchtext) ||
                        aus.AusNummerStr!.ToLower().Contains(suchtext) ||
                        aus.AusSimapNr!.ToLower().Contains(suchtext) ||
                        sacDoeA.SacKurzbezeichnung.ToLower().Contains(suchtext) ||
                        sacDoeA.SacVorname.ToLower().Contains(suchtext) ||
                        sacDoeA.SacNachname.ToLower().Contains(suchtext) ||
                        sacRd.SacKurzbezeichnung.ToLower().Contains(suchtext) ||
                        sacRd.SacVorname.ToLower().Contains(suchtext) ||
                        sacRd.SacNachname.ToLower().Contains(suchtext) ||
                        sacBbl.SacKurzbezeichnung.ToLower().Contains(suchtext) ||
                        sacBbl.SacVorname.ToLower().Contains(suchtext) ||
                        sacBbl.SacNachname.ToLower().Contains(suchtext) ||
                        aus.AusExtern!.ToLower().Contains(suchtext) ||
                        // Anbieter der Offerten (über die Lose der Ausschreibung)
                        dbContext.TOfferte.Any(off => off.OffLos.LosAusId == aus.AusId
                            && off.OffAnb.AnbName.ToLower().Contains(suchtext)
                            // aktueller Zustand = letzter (jüngstes Datum, dann zuletzt erfasst), wie in den Auswertungen
                            && (offertezustandId == null || dbContext.TOfferteOffertezustand
                                .Where(ooz => ooz.OozOffId == off.OffId)
                                .OrderByDescending(ooz => ooz.OozDatum)
                                .ThenByDescending(ooz => ooz.OozInsertedDate)
                                .ThenByDescending(ooz => ooz.OozId)
                                .Select(ooz => (Guid?)ooz.OozOfzId)
                                .FirstOrDefault() == offertezustandId))
                    )
                    // Hauptprojekt-Auswahl: nur Bau-Ausschreibungen, die selbst keinem Hauptprojekt zugewiesen sind
                    && (!forHauptprojekt || (aus.AusPra.PraNaturalId == nameof(EnumProjektart.Bau) && aus.AusAusId == null))
                    && (verfahrensartFilter == null || verfahrensartFilter.Contains(vea.VeaNaturalId))
                    // Betragsbereich: nur Ausschreibungen mit mindestens einer Offerte, deren Zuschlagswert im Bereich liegt
                    && ((betragVon == null && betragBis == null) || dbContext.TOfferte.Any(off =>
                        off.OffLos.LosAusId == aus.AusId
                        && off.OffZuschlagswertChf != null
                        && (betragVon == null || off.OffZuschlagswertChf >= betragVon)
                        && (betragBis == null || off.OffZuschlagswertChf <= betragBis)))
                    select new AusschreibungSuche
                    {
                        Id = aus.AusId,
                        Art = pra.PraBezeichnung!,
                        Bezeichnung = aus.AusBezeichnung!,
                        Jahr = aus.AusJahr,
                        Nummer = aus.AusNummer ?? 0,
                        NummerStr = aus.AusNummerStr ?? string.Empty,
                        Dienststellennummer = amt.AmtDienststellennummer,
                        Rolle = ria.RiaKurzbezeichnung,
                        Sachbearbeiter = sacDoeA.SacKurzbezeichnung,
                        Verfahrensart = vea.VeaBezeichnung,
                        Leistungsart = lea.LeaKurzbezeichnung,
                        Personalverleih = aus.AusPersonalverleih,
                        Publiziert = aus.AusPublikation,
                        Abgeschlossen = aus.AusAbgeschlossen,
                        // Weite Lesart: gesetzt, sobald mindestens ein Los der Ausschreibung den Zustand hat.
                        Abgebrochen = dbContext.TLos.Any(l => l.LosAusId == aus.AusId && l.LosZustand == nameof(EnumLosZustand.Abgebrochen)),
                        // RFI und Vorankündigungen haben keine Lose, ihr Widerruf steht in der Ausschreibung.
                        Widerrufen = aus.AusPublikationWiderruf != null ||
                                     dbContext.TLos.Any(l => l.LosAusId == aus.AusId && l.LosZustand == nameof(EnumLosZustand.Widerrufen)),
                        Ressort = res.ResKurzbezeichnung,
                        SimapNr = aus.AusSimapNr,
                        Abschlussjahr = aus.AusAbschlussJahr,
                        Standort = aus.AusStandort,
                        BeschaffungDezentral = aus.AusBeschaffungDezentral,
                        Rechtsdienst = sacRd.SacKurzbezeichnung,
                        Bbl = sacBbl.SacKurzbezeichnung,
                        Extern = aus.AusExtern,
                        EingabefristOfferte = aus.AusEingabefristTeilnahme,
                        VorankuendigungNummer = voa.AusNummerStr,
                        RfiNummer = rfi.AusNummerStr,
                        InsertedDate = aus.AusInsertedDate,
                        InsertedUser = aus.AusInsertedUser!,
                        ChangedDate = aus.AusChangedDate,
                        ChangedUser = aus.AusChangedUser!
                    };

        var result = await query.ToListAsync();
        return new ObservableCollection<AusschreibungSuche>(result);
    }

    /// <summary>Alle Jahre, in denen Ausschreibungen erfasst sind, absteigend.</summary>
    public async Task<List<int>> GetAlleJahre()
    {
        await using var dbContext = await DbFactory.CreateDbContextAsync();
        return await dbContext.TAusschreibung
            .Select(a => a.AusJahr)
            .Distinct()
            .OrderByDescending(j => j)
            .ToListAsync();
    }

    /// <summary>Alle vergebenen Abschlussjahre, absteigend.</summary>
    public async Task<List<int>> GetAlleAbschlussJahre()
    {
        await using var dbContext = await DbFactory.CreateDbContextAsync();
        return await dbContext.TAusschreibung
            .Where(a => a.AusAbschlussJahr != null)
            .Select(a => a.AusAbschlussJahr!.Value)
            .Distinct()
            .OrderByDescending(j => j)
            .ToListAsync();
    }

    /// <summary>Lädt eine Ausschreibung mit Stammdaten-Bezeichnungen, Verweisen (Hauptprojekt, Vorankündigung, RFI), CPV-Nummern, Sprachen und Berichtigungen; <c>null</c>, wenn sie nicht (mehr) existiert.</summary>
    public async Task<Ausschreibung?> Get(Guid ausId)
    {
        await using var dbContext = await DbFactory.CreateDbContextAsync();

        var result = await ErstelleDetailAbfrage(dbContext, ausId).ToListAsync();

        var ausschreibung = result.FirstOrDefault();
        if (ausschreibung == null)
        {
            return null;
        }
        ausschreibung.AusschreibungSprache = await LadeSprachenAsync(dbContext, ausId);
        ausschreibung.Berichtigungen = await LadeBerichtigungenAsync(dbContext, ausId);

        return ausschreibung;
    }

    /// <summary>
    /// Speichert eine Ausschreibung (neu oder bestehend) samt CPV-Nummern, Sprachen und Berichtigungen; eine neue erhält ein erstes Los
    /// und eine neue Nummer, eine Folgeausschreibung die Nummer ihrer Vorankündigung.
    /// </summary>
    public async Task<Ausschreibung> Save(Ausschreibung aus)
    {
        var istNeu = aus.Id == Guid.Empty;
        if (!istNeu)
        {
            // NothingChanged führt bewusst NICHT zum Abbruch: die Kind-Listen (CPV, Sprachen,
            // Berichtigungen) werden vom Vergleich nicht erfasst und müssen trotzdem synchronisiert werden.
            var status = await CheckChangesAndConcurrency<TAusschreibung>(aus, "Aus");
            if (status is GeneralConstants.ConcurrencyError or GeneralConstants.NotFoundError)
            {
                throw new InvalidOperationException(status);
            }
        }

        await Validate(new AusschreibungValidator(istNeu), aus, istNeu);
        var tAus = Mapper.ZuEntity(aus);

        await using var dbContext = await DbFactory.CreateDbContextAsync();

        var abgeschlossenMitPublikation = VerfahrensartRegeln.IstAbgeschlossenMitPublikation(
            await VerfahrensartNaturalIdAsync(dbContext, tAus.AusVeaId));
        if (abgeschlossenMitPublikation)
        {
            tAus.AusAbgeschlossen = aus.Publikation != null;
        }

        if (istNeu)
        {
            dbContext.TAusschreibung.Add(tAus);
            dbContext.TLos.Add(ErstelleErstesLos(tAus));
        }
        else
        {
            // Wechsel von RFI/Vorankündigung auf eine Verfahrensart mit Losen: Der Abschluss über das Publikationsdatum gilt
            // nicht mehr, er folgt wie im LosService den Losen (abgeschlossen, sobald keines mehr offen ist).
            var bisherVeaId = await dbContext.TAusschreibung.Where(a => a.AusId == tAus.AusId).Select(a => a.AusVeaId).FirstAsync();
            var wechselZuLosen = !abgeschlossenMitPublikation &&
                                 VerfahrensartRegeln.IstAbgeschlossenMitPublikation(await VerfahrensartNaturalIdAsync(dbContext, bisherVeaId));
            if (wechselZuLosen)
            {
                var zustandOffen = nameof(EnumLosZustand.Offen);
                tAus.AusAbgeschlossen = !await dbContext.TLos.AnyAsync(l => l.LosAusId == tAus.AusId && l.LosZustand == zustandOffen);
            }

            var eintrag = dbContext.Entry(tAus);
            eintrag.State = EntityState.Modified;
            SchuetzeErfassungsdaten(eintrag);

            // In diesen Fällen führt Save den Abschluss selbst, die sonst geschützte Spalte wird geschrieben.
            if (abgeschlossenMitPublikation || wechselZuLosen)
            {
                eintrag.Property(nameof(TAusschreibung.AusAbgeschlossen)).IsModified = true;
            }
        }

        await SyncCpvNummernAsync(dbContext, aus);
        await SyncSprachenAsync(dbContext, aus);
        await SyncBerichtigungenAsync(dbContext, aus);

        if (!istNeu)
        {
            await dbContext.SaveChangesAsync();
            return aus;
        }

        var nummerUebernommen = await UebernimmNummerDerVorankuendigung(dbContext, aus, tAus);

        // Nummernvergabe (Max+1) gegen gleichzeitige Neuanlagen absichern: Serializable erkennt den
        // Read-Write-Konflikt zweier paralleler Vergaben, der Versuch wird dann mit neu ermittelter
        // Nummer wiederholt. Ein Unique-Index scheidet aus, weil Folgeausschreibungen die Nummer
        // ihrer Vorankündigung bewusst doppelt führen (auch in den migrierten Altdaten).
        for (var versuch = 1; ; versuch++)
        {
            await using var transaktion = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                if (!nummerUebernommen)
                {
                    await GeneriereAusNummer(dbContext, aus, tAus);
                }

                // AcceptAllChanges erst nach dem Commit: Scheitert der Commit am Konflikt, gälte der
                // ChangeTracker sonst als "gespeichert" und der nächste Versuch würde nichts einfügen.
                await dbContext.SaveChangesAsync(acceptAllChangesOnSuccess: false);
                await transaktion.CommitAsync();
                dbContext.ChangeTracker.AcceptAllChanges();
                break;
            }
            catch (Exception ex) when (versuch < MaxNummernVersuche && IstSerialisierungskonflikt(ex))
            {
                // Rollback über das Dispose der Transaktion; die nächste Runde ermittelt die Nummer neu.
            }
        }

        return aus;
    }

    /// <summary>Erkennt den Serialisierungskonflikt (SQLSTATE 40001) gleichzeitiger Nummernvergaben.</summary>
    private static bool IstSerialisierungskonflikt(Exception ex) =>
        (ex as PostgresException ?? ex.InnerException as PostgresException)?.SqlState ==
        PostgresErrorCodes.SerializationFailure;

    /// <summary>
    /// Bereitet zu einer Vorankündigung die Folgeausschreibung als ungespeicherte Neuanlage vor, ohne in die DB zu schreiben:
    /// übernimmt Nummer und Grunddaten (Jahr, Projektart, Hauptprojekt, Amt, Ressort, Bezeichnung, Rolle BBL (ohne Rolle bei Bau
    /// «Logistik zentral»), Leistungsart, Standort, Personalverleih, Beschaffung dezentral, CPV-Nummern, Verantwortliche inkl.
    /// Extern, RFI-Verweis), leitet die Verfahrensart über <see cref="VerfahrensartRegeln.FolgeVerfahrensart"/> ab und belegt die
    /// Sprachmatrix wie bei einer neuen Ausschreibung vor. Angelegt wird sie erst von <see cref="Save"/>, mit der Nummer der
    /// Vorankündigung (<see cref="UebernimmNummerDerVorankuendigung"/>).
    /// </summary>
    public async Task<Ausschreibung> BereiteFolgeausschreibungVor(Guid vorankuendigungId)
    {
        var vor = await Get(vorankuendigungId)
                  ?? throw new InvalidOperationException($"{GeneralConstants.NullReference}?vorankuendigung");

        // Eigener Schlüssel, damit sich "nicht gefunden" und "ist keine Vorankündigung" im UI unterscheiden.
        if (!VerfahrensartRegeln.IstVorankuendigung(vor.Verfahrensart.NaturalId))
        {
            throw new InvalidOperationException(FehlerKeineVorankuendigung);
        }

        await using var dbContext = await DbFactory.CreateDbContextAsync();

        var folgeNaturalId = VerfahrensartRegeln.FolgeVerfahrensart(vor.Verfahrensart.NaturalId);
        var folgeVea = await dbContext.TVerfahrensart
            .AsNoTracking()
            .FirstAsync(v => v.VeaNaturalId == folgeNaturalId);

        // Rolle der Vorankündigung; fehlt sie, bei Bau wie bei einer neuen Bau-Publikation «Logistik zentral» vorschlagen.
        var riaId = vor.RiaId;
        if (riaId == null && vor.Projektart.NaturalId == nameof(EnumProjektart.Bau))
        {
            riaId = await dbContext.TRolleInAusschreibung.AsNoTracking()
                .Where(r => r.RiaNaturalId == RolleLogistikZentralNaturalId)
                .Select(r => (Guid?)r.RiaId)
                .FirstOrDefaultAsync();
        }

        return new Ausschreibung
        {
            Aktiv = true,
            Jahr = vor.Jahr,
            PraId = vor.PraId,
            Projektart = vor.Projektart,
            VeaId = folgeVea.VeaId,
            Verfahrensart = new Verfahrensart { Id = folgeVea.VeaId, Bezeichnung = folgeVea.VeaBezeichnung, NaturalId = folgeVea.VeaNaturalId ?? string.Empty },
            Hauptprojekt = vor.Hauptprojekt,
            AmtId = vor.AmtId,
            ResId = vor.ResId,
            Bezeichnung = vor.Bezeichnung,
            RiaId = riaId,
            LeaId = vor.LeaId,
            Leistungsart = vor.Leistungsart,
            Standort = vor.Standort,
            Personalverleih = vor.Personalverleih,
            BeschaffungDezentral = vor.BeschaffungDezentral,
            SacDoeAId = vor.SacDoeAId,
            SacRdId = vor.SacRdId,
            SacBblId = vor.SacBblId,
            Extern = vor.Extern,
            Nummer = vor.Nummer,
            NummerStr = vor.NummerStr,
            Vorankuendigung = new Ausschreibung { Id = vor.Id, Bezeichnung = vor.Bezeichnung },
            Rfi = vor.Rfi,
            // Neue Einträge (ohne Id): Save legt sie wie bei jeder Neuanlage an.
            CpvNummern = vor.CpvNummern
                .DistinctBy(c => c.CpvId)
                .Select(c => new AusschreibungCpvNummer { CpvId = c.CpvId, CpvNummer = c.CpvNummer })
                .ToList(),
            AusschreibungSprache = await StandardSprachenAsync(dbContext, folgeNaturalId)
        };
    }

    /// <summary>
    /// Löscht eine Ausschreibung samt abhängigen Daten (Lose, Offerten, CPV-Nummern, Sprachen, Verfahrensschritte, Berichtigungen)
    /// in einer Transaktion; da auf <c>AusId</c> keine DB-seitige Cascade existiert, werden die Kinder in FK-sicherer Reihenfolge entfernt.
    /// Hauptprojekte mit Unterprojekten und bereits publizierte Ausschreibungen können nicht gelöscht werden.
    /// </summary>
    public override async Task<Ausschreibung> Delete(Guid id)
    {
        await using var dbContext = await DbFactory.CreateDbContextAsync();

        var tAus = await dbContext.TAusschreibung.AsNoTracking().FirstOrDefaultAsync(a => a.AusId == id);
        if (tAus == null)
        {
            throw new InvalidOperationException($"{GeneralConstants.NullReference}?ausschreibung");
        }

        if (tAus.AusPublikation != null)
        {
            throw new InvalidOperationException(FehlerPubliziert);
        }

        if (await dbContext.TAusschreibung.AnyAsync(a => a.AusAusId == id))
        {
            throw new InvalidOperationException(FehlerHatUnterprojekte);
        }

        await using var tx = await dbContext.Database.BeginTransactionAsync();

        // Verweise anderer Ausschreibungen auf diese (als Vorankündigung/RFI) lösen, bevor sie gelöscht wird.
        // Es gibt keine DB-FK mit ON DELETE SET NULL, daher wird das hier explizit nachgebildet, damit keine
        // verwaisten Verweis-Ids zurückbleiben (die beim Laden des Referenzierers sonst zu Fehlern führen).
        await dbContext.TAusschreibung.Where(a => a.AusVorankuendigungId == id)
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.AusVorankuendigungId, (Guid?)null));
        await dbContext.TAusschreibung.Where(a => a.AusRfiId == id)
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.AusRfiId, (Guid?)null));

        var losIds = await dbContext.TLos.Where(l => l.LosAusId == id).Select(l => l.LosId).ToListAsync();
        var offIds = await dbContext.TOfferte.Where(o => losIds.Contains(o.OffLosId)).Select(o => o.OffId).ToListAsync();

        await dbContext.TOfferteOffertezustand.Where(z => offIds.Contains(z.OozOffId)).ExecuteDeleteAsync();
        await dbContext.TOfferte.Where(o => losIds.Contains(o.OffLosId)).ExecuteDeleteAsync();
        await dbContext.TLosBkpNummer.Where(b => losIds.Contains(b.LbnLosId)).ExecuteDeleteAsync();
        await dbContext.TLos.Where(l => l.LosAusId == id).ExecuteDeleteAsync();
        await dbContext.TAusschreibungCpvNummer.Where(c => c.AcnAusId == id).ExecuteDeleteAsync();
        await dbContext.TAusschreibungSprache.Where(s => s.AspAusId == id).ExecuteDeleteAsync();
        await dbContext.TAusschreibungVerfahrensschritt.Where(v => v.AvsAusId == id).ExecuteDeleteAsync();
        await dbContext.TBerichtigung.Where(b => b.BerAusId == id).ExecuteDeleteAsync();
        await dbContext.TAusschreibung.Where(a => a.AusId == id).ExecuteDeleteAsync();

        await tx.CommitAsync();

        return Mapper.ZuModel(tAus);
    }

    /// <summary>Die Ausschreibung mit allen Stammdaten-Bezeichnungen, Verweisen und CPV-Nummern.</summary>
    private static IQueryable<Ausschreibung> ErstelleDetailAbfrage(DoeAContext dbContext, Guid ausId)
    {
        return from aus in dbContext.TAusschreibung
               join lea in dbContext.TLeistungsart on aus.AusLeaId equals lea.LeaId into leaJoin
               from lea in leaJoin.DefaultIfEmpty()
               join vea in dbContext.TVerfahrensart on aus.AusVeaId equals vea.VeaId into veaJoin
               from vea in veaJoin.DefaultIfEmpty()
               join ria in dbContext.TRolleInAusschreibung on aus.AusRiaId equals ria.RiaId into reaJoin
               from ria in reaJoin.DefaultIfEmpty()
               join sacDoeA in dbContext.TSachbearbeiter on aus.AusSacDoeAId equals sacDoeA.SacId into sacDoeAJoin
               from sacDoeA in sacDoeAJoin.DefaultIfEmpty()
               join sacRd in dbContext.TSachbearbeiter on aus.AusSacRdId equals sacRd.SacId into sacRdJoin
               from sacRd in sacRdJoin.DefaultIfEmpty()
               join sacBbl in dbContext.TSachbearbeiter on aus.AusSacBblId equals sacBbl.SacId into sacBblJoin
               from sacBbl in sacBblJoin.DefaultIfEmpty()
               join amt in dbContext.TAmt on aus.AusAmtId equals amt.AmtId into amtJoin
               from amt in amtJoin.DefaultIfEmpty()
               join res in dbContext.TRessort on aus.AusResId equals res.ResId into resJoin
               from res in resJoin.DefaultIfEmpty()
               join pra in dbContext.TProjektart on aus.AusPraId equals pra.PraId into praJoin
               from pra in praJoin.DefaultIfEmpty()
               join hpt in dbContext.TAusschreibung on aus.AusAusId equals hpt.AusId into hptJoin
               from hpt in hptJoin.DefaultIfEmpty()
               join voa in dbContext.TAusschreibung on aus.AusVorankuendigungId equals voa.AusId into voaJoin
               from voa in voaJoin.DefaultIfEmpty()
               join rfi in dbContext.TAusschreibung on aus.AusRfiId equals rfi.AusId into rfiJoin
               from rfi in rfiJoin.DefaultIfEmpty()
               where aus.AusId == ausId
               select new Ausschreibung
               {
                   Id = aus.AusId,
                   PraId = pra.PraId,
                   AmtId = amt.AmtId,
                   ResId = res.ResId,
                   Hauptprojekt = aus.AusAusId == null ? null : new Ausschreibung
                   {
                       Id = hpt.AusId,
                       Bezeichnung = hpt.AusBezeichnung!
                   },
                   Vorankuendigung = aus.AusVorankuendigungId == null ? null : new Ausschreibung
                   {
                       Id = voa.AusId,
                       Bezeichnung = voa.AusBezeichnung!
                   },
                   Rfi = aus.AusRfiId == null ? null : new Ausschreibung
                   {
                       Id = rfi.AusId,
                       Bezeichnung = rfi.AusBezeichnung!
                   },
                   Projektart = new Projektart
                   {
                       Bezeichnung = pra.PraBezeichnung!,
                       NaturalId = pra.PraNaturalId!
                   },
                   Bezeichnung = aus.AusBezeichnung!,
                   Jahr = aus.AusJahr,
                   SimapNr = aus.AusSimapNr,
                   Nummer = aus.AusNummer ?? 0,
                   NummerStr = aus.AusNummerStr ?? string.Empty,
                   VeaId = vea.VeaId,
                   Standort = aus.AusStandort!,
                   Personalverleih = aus.AusPersonalverleih,
                   BeschaffungDezentral = aus.AusBeschaffungDezentral,
                   VerzoegerungDurchAuftraggeber = aus.AusVerzoegerungDurchAuftraggeber,
                   Verfahrensart = new Verfahrensart
                   {
                       Bezeichnung = vea.VeaBezeichnung!,
                       NaturalId = vea.VeaNaturalId!
                   },
                   LeaId = lea.LeaId,
                   Leistungsart = new Leistungsart
                   {
                       Bezeichnung = lea.LeaBezeichnung,
                       Kurzbezeichnung = lea.LeaKurzbezeichnung,
                   },
                   RiaId = aus.AusRiaId,
                   RolleInAusschreibung = new RolleInAusschreibung
                   {
                       Bezeichnung = ria.RiaBezeichnung,
                       Kurzbezeichnung = ria.RiaKurzbezeichnung
                   },
                   AbschlussJahr = aus.AusAbschlussJahr,
                   SacBblId = sacBbl.SacId,
                   SacRdId = sacRd.SacId,
                   SacDoeAId = sacDoeA.SacId,
                   Extern = aus.AusExtern!,
                   Eingereicht = aus.AusEingereicht,
                   Publikation = aus.AusPublikation,
                   EingereichtWiderruf = aus.AusEingereichtWiderruf,
                   PublikationWiderruf = aus.AusPublikationWiderruf,
                   VerzoegerungDurchAuftraggeberWiderruf = aus.AusVerzoegerungDurchAuftraggeberWiderruf,
                   EingabefristTeilnahme = aus.AusEingabefristTeilnahme,
                   OffertenFrist = aus.AusOffertenFrist,
                   VersandPh2 = aus.AusVersandPh2,
                   PflichtenhefterPraequali = aus.AusPflichtenhefterPraequali ?? 0,
                   Pflichtenhefter = aus.AusPflichtenhefter ?? 0,
                   AnzahlTeilnehmer = aus.AusAnzahlTeilnehmer ?? 0,
                   CpvNummern = (
                       from acnInner in dbContext.TAusschreibungCpvNummer
                       join cpvInner in dbContext.TCpvNummer on acnInner.AcnCpvId equals cpvInner.CpvId
                       where acnInner.AcnAusId == aus.AusId
                       select new AusschreibungCpvNummer
                       {
                           Id = acnInner.AcnId,
                           CpvId = acnInner.AcnCpvId,
                           CpvNummer = new CpvNummer
                           {
                               Id = cpvInner.CpvId,
                               NummerStr = cpvInner.CpvNummerStr,
                               Bezeichnung = cpvInner.CpvBezeichnung,
                           }
                       }).ToList(),
                   // Ausschreibungen, die diese als Vorankündigung oder RFI referenzieren
                   ReferenziertVon = (
                       from r in dbContext.TAusschreibung
                       join rvea in dbContext.TVerfahrensart on r.AusVeaId equals rvea.VeaId into rveaJoin
                       from rvea in rveaJoin.DefaultIfEmpty()
                       where r.AusVorankuendigungId == aus.AusId || r.AusRfiId == aus.AusId
                       select new Ausschreibung
                       {
                           Id = r.AusId,
                           Bezeichnung = r.AusBezeichnung!,
                           NummerStr = r.AusNummerStr ?? string.Empty,
                           Verfahrensart = new Verfahrensart
                           {
                               Bezeichnung = rvea.VeaBezeichnung!,
                               NaturalId = rvea.VeaNaturalId!
                           }
                       }).ToList(),
                   Bemerkung = aus.AusBemerkung!,
                   InsertedDate = aus.AusInsertedDate,
                   InsertedUser = aus.AusInsertedUser!,
                   ChangedDate = aus.AusChangedDate,
                   ChangedUser = aus.AusChangedUser!
               };
    }

    /// <summary>Die erfassten Sprache/Dokumentart-Kombinationen einer Ausschreibung.</summary>
    private static async Task<List<AusschreibungSprache>> LadeSprachenAsync(DoeAContext dbContext, Guid ausId)
    {
        return await dbContext.TAusschreibungSprache
            .AsNoTracking()
            .Where(a => a.AspAusId == ausId)
            .Select(a => new AusschreibungSprache
            {
                Id = a.AspId,
                AusId = a.AspAusId,
                Dokumentart = a.AspDoa.DoaBezeichnung,
                Sprache = a.AspSpr.SprBezeichnung,
                DoaId = a.AspDoaId,
                SprId = a.AspSprId
            })
            .ToListAsync();
    }

    /// <summary>Die Berichtigungen einer Ausschreibung, nach Eingabedatum und Erfassung sortiert.</summary>
    private static async Task<List<Berichtigung>> LadeBerichtigungenAsync(DoeAContext dbContext, Guid ausId)
    {
        return await dbContext.TBerichtigung
            .AsNoTracking()
            .Where(b => b.BerAusId == ausId)
            .OrderBy(b => b.BerEingereicht)
            .ThenBy(b => b.BerInsertedDate)
            .Select(b => new Berichtigung
            {
                Id = b.BerId,
                AusId = b.BerAusId,
                Eingereicht = b.BerEingereicht,
                Publiziert = b.BerPubliziert,
                VerzoegerungDurchAuftraggeber = b.BerVerzoegerungDurchAuftraggeber,
                Bemerkungen = b.BerBemerkungen,
                Aktiv = b.BerAktiv,
                InsertedDate = b.BerInsertedDate,
                InsertedUser = b.BerInsertedUser,
                ChangedDate = b.BerChangedDate,
                ChangedUser = b.BerChangedUser
            })
            .ToListAsync();
    }

    /// <summary>Gleicht die CPV-Nummern der Ausschreibung mit dem gespeicherten Bestand ab.</summary>
    private async Task SyncCpvNummernAsync(DoeAContext dbContext, Ausschreibung aus)
    {
        var bestand = await dbContext.TAusschreibungCpvNummer.Where(a => a.AcnAusId == aus.Id).ToListAsync();
        var neue = aus.CpvNummern.Where(c => bestand.All(b => b.AcnCpvId != c.CpvId)).ToList();

        foreach (var acn in neue)
        {
            await Validate(new BasisValidator(), acn, true);
            var tAcn = _ausschreibungCpvNummerMapper.ZuEntity(acn);
            // AusId erst hier setzen: bei einer neuen Ausschreibung existiert sie vorher nicht.
            tAcn.AcnAusId = aus.Id;
            dbContext.TAusschreibungCpvNummer.Add(tAcn);
        }

        dbContext.TAusschreibungCpvNummer.RemoveRange(
            bestand.Where(b => aus.CpvNummern.All(c => c.CpvId != b.AcnCpvId)));
    }

    private static Task<string?> VerfahrensartNaturalIdAsync(DoeAContext dbContext, Guid veaId) =>
        dbContext.TVerfahrensart.Where(v => v.VeaId == veaId).Select(v => v.VeaNaturalId).FirstOrDefaultAsync();

    /// <summary>Sprachmatrix einer neuen Ausschreibung der Verfahrensart (<see cref="VerfahrensartRegeln.StandardSprachen"/>).</summary>
    private static async Task<List<AusschreibungSprache>> StandardSprachenAsync(DoeAContext dbContext, string verfahrensartNaturalId)
    {
        var dokumentarten = await dbContext.TDokumentart.AsNoTracking().Where(d => d.DoaAktiv).ToListAsync();
        var sprachen = await dbContext.TSprache.AsNoTracking().Where(s => s.SprAktiv).ToListAsync();

        return (from vorgabe in VerfahrensartRegeln.StandardSprachen(verfahrensartNaturalId)
                join dokumentart in dokumentarten on vorgabe.Key equals dokumentart.DoaNaturalId
                from sprache in sprachen.Where(s => vorgabe.Value.Contains(s.SprNaturalId))
                select new AusschreibungSprache
                {
                    DoaId = dokumentart.DoaId,
                    Dokumentart = dokumentart.DoaBezeichnung,
                    SprId = sprache.SprId,
                    Sprache = sprache.SprBezeichnung,
                    Wert = true
                }).ToList();
    }

    /// <summary>Gleicht die Sprache/Dokumentart-Kombinationen der Ausschreibung mit dem Bestand ab.</summary>
    private async Task SyncSprachenAsync(DoeAContext dbContext, Ausschreibung aus)
    {
        var bestand = await dbContext.TAusschreibungSprache.Where(a => a.AspAusId == aus.Id).ToListAsync();
        var neue = aus.AusschreibungSprache
            .Where(s => bestand.All(b => b.AspDoaId != s.DoaId || b.AspSprId != s.SprId))
            .ToList();

        foreach (var sprache in neue)
        {
            await Validate(new BasisValidator(), sprache, true);
            var tAsp = _ausschreibungSpracheMapper.ZuEntity(sprache);
            // AusId erst hier setzen: bei einer neuen Ausschreibung existiert sie vorher nicht.
            tAsp.AspAusId = aus.Id;
            dbContext.TAusschreibungSprache.Add(tAsp);
        }

        dbContext.TAusschreibungSprache.RemoveRange(bestand.Where(b =>
            aus.AusschreibungSprache.All(s => s.SprId != b.AspSprId || s.DoaId != b.AspDoaId)));
    }

    /// <summary>
    /// Gleicht die Berichtigungen der Ausschreibung mit dem Bestand ab: entfernte löschen,
    /// neue anlegen, bestehende feldweise aktualisieren.
    /// </summary>
    private async Task SyncBerichtigungenAsync(DoeAContext dbContext, Ausschreibung aus)
    {
        var bestand = await dbContext.TBerichtigung.Where(b => b.BerAusId == aus.Id).ToListAsync();
        var behaltenIds = aus.Berichtigungen.Select(b => b.Id).ToHashSet();
        dbContext.TBerichtigung.RemoveRange(bestand.Where(b => !behaltenIds.Contains(b.BerId)));

        foreach (var ber in aus.Berichtigungen)
        {
            var istNeu = ber.Id == Guid.Empty || bestand.All(b => b.BerId != ber.Id);
            await Validate(new BerichtigungValidator(), ber, istNeu);

            if (istNeu)
            {
                var tBer = _berichtigungMapper.ZuEntity(ber);
                // AusId erst hier setzen: bei einer neuen Ausschreibung existiert sie vorher nicht.
                tBer.BerAusId = aus.Id;
                dbContext.TBerichtigung.Add(tBer);
            }
            else
            {
                var tBer = bestand.Single(b => b.BerId == ber.Id);
                tBer.BerEingereicht = ber.Eingereicht;
                tBer.BerPubliziert = ber.Publiziert;
                tBer.BerVerzoegerungDurchAuftraggeber = ber.VerzoegerungDurchAuftraggeber;
                tBer.BerBemerkungen = ber.Bemerkungen;
                tBer.BerChangedDate = ber.ChangedDate;
                tBer.BerChangedUser = ber.ChangedUser!;
            }
        }
    }

    /// <summary>Das erste Los, das jede neu angelegte Ausschreibung erhält (Audit-Werte der Ausschreibung).</summary>
    private static TLos ErstelleErstesLos(TAusschreibung tAus)
    {
        return new TLos
        {
            LosId = Guid.NewGuid(),
            LosAusId = tAus.AusId,
            LosBezeichnung = ErstesLos,
            LosZustand = nameof(EnumLosZustand.Offen),
            LosAktiv = true,
            LosInsertedDate = tAus.AusInsertedDate,
            LosInsertedUser = tAus.AusInsertedUser,
            LosChangedDate = tAus.AusChangedDate,
            LosChangedUser = tAus.AusChangedUser
        };
    }

    /// <summary>
    /// Folgeausschreibung (<see cref="BereiteFolgeausschreibungVor"/>): Eine Neuanlage, die eine Vorankündigung referenziert und
    /// deren Nummer trägt, übernimmt Nummer, Nummerntext und Laufnummer aus der DB statt eine neue Nummer zu erhalten. Eine manuell
    /// erfasste Neuanlage hat keine Nummer und erhält über <see cref="GeneriereAusNummer"/> eine eigene, auch mit Vorankündigung.
    /// </summary>
    private static async Task<bool> UebernimmNummerDerVorankuendigung(DoeAContext dbContext, Ausschreibung aus, TAusschreibung tAus)
    {
        if (aus.Nummer == 0 || aus.Vorankuendigung?.Id is not { } vorId || vorId == Guid.Empty)
        {
            return false;
        }

        var tVor = await dbContext.TAusschreibung
            .AsNoTracking()
            .Include(a => a.AusVea)
            .FirstOrDefaultAsync(a => a.AusId == vorId);

        if (tVor == null || !VerfahrensartRegeln.IstVorankuendigung(tVor.AusVea.VeaNaturalId) || tVor.AusNummer != aus.Nummer)
        {
            return false;
        }

        tAus.AusNummer = tVor.AusNummer;
        tAus.AusNummerStr = tVor.AusNummerStr;
        tAus.AusLaufnummer = tVor.AusLaufnummer;
        aus.NummerStr = tVor.AusNummerStr ?? string.Empty;
        return true;
    }

    /// <summary>
    /// Generiert für eine neue Ausschreibung die Laufnummer.
    /// <para>
    /// <c>AusNummer</c> hat das Format JJ999 (JJ = 2-stelliges Jahr, 999 = Laufnummer ab 0) und wird
    /// pro Projektart und Jahr fortlaufend vergeben.
    /// </para>
    /// <para>
    /// Bau-Unterprojekte (einem Hauptprojekt zugewiesen) übernehmen die <c>AusNummer</c> des Hauptprojekts
    /// und erhalten zusätzlich eine 2-stellige <c>AusLaufnummer</c> ab 1.
    /// </para>
    /// <para>
    /// <c>AusNummerStr</c> erhält bei Projektart "Bau" das Präfix "b" (bei Unterprojekten zusätzlich
    /// "-{Laufnummer}"), bei "Logistik" wird die <c>AusNummer</c> übernommen.
    /// </para>
    /// </summary>
    private async Task GeneriereAusNummer(DoeAContext dbContext, Ausschreibung aus, TAusschreibung tAus)
    {
        var projektart = await dbContext.TProjektart
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PraId == tAus.AusPraId);
        var istBau = projektart?.PraNaturalId == nameof(EnumProjektart.Bau);

        var hauptprojektId = aus.Hauptprojekt?.Id;
        var istUnterprojekt = istBau && hauptprojektId is { } hpId && hpId != Guid.Empty;

        if (istUnterprojekt)
        {
            // Unterprojekt: AusNummer vom Hauptprojekt übernehmen, AusLaufnummer fortlaufend ab 1
            var hauptprojekt = await dbContext.TAusschreibung
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AusId == hauptprojektId);

            tAus.AusAusId = hauptprojektId;
            tAus.AusNummer = hauptprojekt?.AusNummer;

            var maxLaufnummer = await dbContext.TAusschreibung
                .Where(a => a.AusAusId == hauptprojektId)
                .MaxAsync(a => (int?)a.AusLaufnummer);
            tAus.AusLaufnummer = (maxLaufnummer ?? 0) + 1;
        }
        else
        {
            // Hauptprojekt bzw. Logistik: neue Nummer pro Projektart und Jahr (Format JJ999, Laufnummer ab 0)
            var basis = (tAus.AusJahr % NummerJahrModulo) * NummerJahrFaktor;

            var maxNummer = await dbContext.TAusschreibung
                .Where(a => a.AusPraId == tAus.AusPraId && a.AusJahr == tAus.AusJahr && a.AusAusId == null)
                .MaxAsync(a => (int?)a.AusNummer);

            tAus.AusNummer = maxNummer.HasValue ? maxNummer.Value + 1 : basis;
            tAus.AusLaufnummer = null;
            tAus.AusAusId = null;
        }

        if (!istBau)
        {
            tAus.AusNummerStr = tAus.AusNummer?.ToString();
        }
        else if (istUnterprojekt)
        {
            tAus.AusNummerStr = $"b{tAus.AusNummer}-{tAus.AusLaufnummer}";
        }
        else
        {
            tAus.AusNummerStr = $"b{tAus.AusNummer}";
        }

        // Generierte Nummer ins DTO zurückschreiben, damit sie z.B. im Header angezeigt werden kann.
        aus.Nummer = tAus.AusNummer ?? 0;
        aus.NummerStr = tAus.AusNummerStr ?? string.Empty;
    }
}
