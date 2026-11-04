// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Enums;
using DoeA.Data.Helper;
using DoeA.Data.Mapper;
using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO;
using DoeA.Data.Models.DTO.Stammdaten;
using DoeA.Interfaces;
using DoeA.Validator;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using ILogger = Serilog.ILogger;

namespace DoeA.Services.Ausschreibungen;

/// <summary>Lädt und speichert die Lose einer Ausschreibung samt Offerten, Offertenzuständen und BKP-Nummern.</summary>
public class LosService : BaseService<Los, TLos, LosValidator>, ILosService
{
    /// <summary>NaturalId des Offertezustands "Angebot abgelehnt" – der Standard-Schlusszustand.</summary>
    private const string NaturalIdAbgelehnt = "abgelehnt";

    /// <summary>NaturalId des Offertezustands "Widerrufen".</summary>
    private const string NaturalIdWiderrufen = "widerrufen";

    /// <summary>NaturalId des Offertezustands "Zuschlag erhalten" (ohne Präqualifikation, die das Los nicht beendet).</summary>
    private const string NaturalIdZuschlag = "zuschlag";

    /// <summary>NaturalId des Offertezustands "Zuschlag Präqualifikation".</summary>
    private const string NaturalIdZuschlagPraequali = "zuschlagpraequali";

    private const string FehlerPraequaliDatenUnterschiedlich = "ERROR_AUSSCHREIBUNG_PRAEQUALI_DATEN_UNTERSCHIEDLICH";

    /// <summary>
    /// Der bereits gespeicherte Objektbaum einer Ausschreibung (Lose → Offerten → Zustände) samt BKP-Zuordnungen.
    /// Er wird einmal geladen und dient allen Abgleichen als Vergleichsbasis.
    /// </summary>
    private sealed record Bestand(
        List<TLos> Lose,
        List<TOfferte> Offerten,
        List<TOfferteOffertezustand> Zustaende,
        List<TLosBkpNummer> BkpNummern)
    {
        public HashSet<Guid> LosIds { get; } = Lose.Select(l => l.LosId).ToHashSet();
        public HashSet<Guid> OfferteIds { get; } = Offerten.Select(o => o.OffId).ToHashSet();
        public HashSet<Guid> ZustandIds { get; } = Zustaende.Select(z => z.OozId).ToHashSet();

        /// <summary>
        /// Ids, die der Client weiterhin schickt — über den gesamten Tab hinweg, nicht nur beim
        /// bisherigen Parent. Massgeblich fürs Löschen, damit ein zwischen Losen bzw. Offerten
        /// umgehängter Datensatz nicht fälschlich entfernt wird.
        /// </summary>
        public HashSet<Guid> BehalteneOfferteIds { get; init; } = [];
        public HashSet<Guid> BehalteneZustandIds { get; init; } = [];
    }

    /// <summary>Aus den Offertezustand-Stammdaten abgeleitete Angaben für die Ermittlung des Loszustands.</summary>
    private sealed record Zustandsregeln(HashSet<Guid> SchlusszustandIds, HashSet<Guid> WiderrufIds, HashSet<Guid> ZuschlagIds,
        HashSet<Guid> ZuschlagPraequaliIds, HashSet<Guid> PublikationIds);

    private readonly LosBkpNummerMapper _losBkpNummerMapper;
    private readonly OfferteMapper _offerteMapper;
    private readonly OfferteOffertezustandMapper _offerteOffertezustandMapper;

    public LosService(IErrorHandlerService errorHandlerService, IDbContextFactory<DoeAContext> dbFactory,
        LosMapper mapper, OfferteMapper offerteMapper, OfferteOffertezustandMapper offerteOffertezustandMapper,
        LosBkpNummerMapper losBkpNummerMapper, ILogger logger, IAktuellerBenutzer aktuellerBenutzer)
        : base(errorHandlerService, dbFactory, mapper, logger, aktuellerBenutzer, new LosValidator(), "Los")
    {
        _losBkpNummerMapper = losBkpNummerMapper;
        _offerteMapper = offerteMapper;
        _offerteOffertezustandMapper = offerteOffertezustandMapper;
    }

    /// <summary>Lädt alle Lose einer Ausschreibung inklusive ihrer BKP-Nummern.</summary>
    public async Task<ObservableCollection<Los>> Load(Guid ausId)
    {
        await using var dbContext = await DbFactory.CreateDbContextAsync();
        var query = from los in dbContext.TLos
            where los.LosAusId == ausId
            select new Los
            {
                Id = los.LosId,
                AusId = los.LosAusId,
                Abbruchdatum = los.LosAbbruchdatum,
                Eingereicht = los.LosEingereicht,
                VerzoegerungDurchAuftraggeber = los.LosVerzoegerungDurchAuftraggeber,
                Bezeichnung = los.LosBezeichnung,
                Zustand = Enum.Parse<EnumLosZustand>(los.LosZustand),
                InsertedDate = los.LosInsertedDate,
                InsertedUser = los.LosInsertedUser,
                ChangedDate = los.LosChangedDate,
                ChangedUser = los.LosChangedUser,
                BkpNummern = (
                    from lbn in dbContext.TLosBkpNummer
                    join bkp in dbContext.TBkpNummer on lbn.LbnBkpId equals bkp.BkpId
                    where lbn.LbnLosId == los.LosId
                    orderby bkp.BkpNummer
                    select new LosBkpNummer
                    {
                        Id = lbn.LbnId,
                        LosId = lbn.LbnLosId,
                        BkpId = lbn.LbnBkpId,
                        InsertedDate = lbn.LbnInsertedDate,
                        InsertedUser = lbn.LbnInsertedUser,
                        ChangedDate = lbn.LbnChangedDate,
                        ChangedUser = lbn.LbnChangedUser,
                        Aktiv = lbn.LbnAktiv,
                        BkpNummer = new BkpNummer
                        {
                            Id = bkp.BkpId,
                            NummerStr = bkp.BkpNummerStr,
                            Bezeichnung = bkp.BkpBezeichnung,
                            Nummer = bkp.BkpNummer
                        }
                    }).ToList()
            };

        // Kein Distinct: TLos-Zeilen sind pro Ausschreibung eindeutig, und EF kann Distinct
        // nicht mit einer Collection-Projektion (BkpNummern) kombinieren.
        var result = await query.OrderBy(l => l.Bezeichnung).ToListAsync();
        return new ObservableCollection<Los>(result);
    }

    /// <summary>
    /// Schliesst ein offenes Los ab: Jede Offerte des Los, die noch keinen Schlusszustand besitzt,
    /// erhält den Standard-Schlusszustand "Angebot abgelehnt" mit dem heutigen Datum. Ist danach
    /// kein Los der Ausschreibung mehr offen, wird die Ausschreibung als abgeschlossen markiert.
    /// </summary>
    public async Task Abschliessen(Guid losId)
    {
        await using var dbContext = await DbFactory.CreateDbContextAsync();
        var jetzt = DateTime.Now;

        var tLos = await dbContext.TLos.FirstOrDefaultAsync(l => l.LosId == losId);
        if (tLos == null)
        {
            return;
        }

        // Nur offene Lose dürfen abgeschlossen werden, sonst würde ein Abbruch oder Widerruf
        // überschrieben. Die UI graut den Chip aus; hier gegen parallele Änderungen absichern.
        if (tLos.LosZustand != EnumLosZustand.Offen.ToString())
        {
            throw new InvalidOperationException("ERROR_AUSSCHREIBUNG_LOS_ABSCHLIESSEN_NUR_OFFEN");
        }

        // Das Los gilt auch dann als abgeschlossen, wenn es gar keine Offerten hat.
        tLos.LosZustand = EnumLosZustand.Abgeschlossen.ToString();
        tLos.LosChangedUser = User;
        tLos.LosChangedDate = jetzt.Ticks;

        // Der Zustand "abgelehnt" ist fachliche Voraussetzung fürs Abschliessen; fehlt er
        // (unvollständige Datenübernahme), bricht der Vorgang ab, statt Offerten still offen zu lassen.
        var abgelehnt = await dbContext.TOffertezustand
            .Where(z => z.OfzNaturalId == NaturalIdAbgelehnt)
            .Select(z => new { z.OfzId, z.OfzBezeichnung })
            .FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("ERROR_OFFERTEZUSTAND_ABGELEHNT_FEHLT");
        var regeln = await LadeZustandsregeln(dbContext);
        var offerteIds = await dbContext.TOfferte
            .Where(o => o.OffLosId == losId)
            .Select(o => o.OffId)
            .ToListAsync();

        if (offerteIds.Count > 0)
        {
            var schlusszustandIds = regeln.SchlusszustandIds;
            var offertenMitSchlusszustand = await dbContext.TOfferteOffertezustand
                .Where(z => offerteIds.Contains(z.OozOffId) && schlusszustandIds.Contains(z.OozOfzId))
                .Select(z => z.OozOffId)
                .Distinct()
                .ToListAsync();

            foreach (var offerteId in offerteIds.Where(id => !offertenMitSchlusszustand.Contains(id)))
            {
                var zustand = new OfferteOffertezustand
                {
                    OffId = offerteId,
                    OfzId = abgelehnt.OfzId,
                    Bezeichnung = abgelehnt.OfzBezeichnung,
                    Datum = jetzt
                };
                StampForSave(zustand, true, jetzt.Ticks);
                dbContext.TOfferteOffertezustand.Add(_offerteOffertezustandMapper.ZuEntity(zustand));
            }
        }

        // Eine Ausschreibung gilt als abgeschlossen, sobald kein Los mehr im Zustand "Offen" ist (analog SaveTab).
        var zustandOffen = EnumLosZustand.Offen.ToString();
        var hatOffeneLose = await dbContext.TLos.AnyAsync(l =>
            l.LosAusId == tLos.LosAusId && l.LosId != losId && l.LosZustand == zustandOffen);
        if (!hatOffeneLose)
        {
            var tAus = await dbContext.TAusschreibung.FirstOrDefaultAsync(a => a.AusId == tLos.LosAusId);
            if (tAus != null)
            {
                tAus.AusAbgeschlossen = true;
            }
        }

        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Gleicht Lose, Offerten und Offertenzustände des Tabs «Lose und Offerten» in einer Transaktion ab (insert/update/delete).
    /// «Neu» wird am Fehlen der client-seitig vergebenen Id in der DB erkannt, damit neue Untereinträge (Offerten/Zustände) in-memory
    /// auf ihren Parent verweisen können; Unverändertes wird nicht neu gestempelt, Löschungen und Ausschreibungs-Felder sind bewusst Last-write-wins.
    /// </summary>
    public async Task SaveTab(Guid ausId, IEnumerable<Los> lose, int? pflichtenhefter)
    {
        var loseList = lose.ToList();

        // Es muss immer mindestens ein Los existieren.
        if (loseList.Count == 0)
        {
            throw new InvalidOperationException("ERROR_AUSSCHREIBUNG_LOS_MINDESTENS_EINS");
        }

        await using var dbContext = await DbFactory.CreateDbContextAsync();

        // Freihändig (Zuschlagsempfänger): pro Los genau eine Offerte erlaubt.
        if (await IstFreihaendig(dbContext, ausId) && loseList.Any(l => l.Offerten.Count > 1))
        {
            throw new InvalidOperationException("ERROR_AUSSCHREIBUNG_OFFERTE_FREIHAENDIG_NUR_EINE");
        }

        // Pro Los darf ein Anbieter nur eine Offerte einreichen.
        if (loseList.Any(l => l.Offerten
                .Where(o => o.AnbId.HasValue)
                .GroupBy(o => o.AnbId!.Value)
                .Any(g => g.Count() > 1)))
        {
            throw new InvalidOperationException("ERROR_AUSSCHREIBUNG_OFFERTE_ANBIETER_DOPPELT");
        }

        await using var tx = await dbContext.Database.BeginTransactionAsync();

        var now = DateTime.Now.Ticks;
        var regeln = await LadeZustandsregeln(dbContext);
        PruefePraequaliDaten(loseList, regeln);
        var bestand = await LadeBestand(dbContext, ausId, loseList);

        // Das Ausschreibungs-Feld "Pflichtenhefter" gehört zum Tab-Inhalt und wird im selben Vorgang mitgeschrieben.
        var tAus = await dbContext.TAusschreibung.FirstOrDefaultAsync(a => a.AusId == ausId);
        if (tAus != null)
        {
            tAus.AusPflichtenhefter = pflichtenhefter;
        }

        EntferneGeloeschteLose(dbContext, bestand, loseList);

        foreach (var los in loseList)
        {
            los.AusId = ausId;
            await SpeichereLos(dbContext, bestand, regeln, los, now);
        }

        // Eine Ausschreibung gilt als abgeschlossen, sobald kein Los mehr im Zustand "Offen" ist.
        if (tAus != null)
        {
            tAus.AusAbgeschlossen = loseList.All(l => l.Zustand != EnumLosZustand.Offen);
        }

        await dbContext.SaveChangesAsync();
        await tx.CommitAsync();
    }

    /// <summary>Liest die Offertezustand-Stammdaten einmalig und leitet daraus die Zustandsregeln ab.</summary>
    private static async Task<Zustandsregeln> LadeZustandsregeln(DoeAContext dbContext)
    {
        var zustaende = await dbContext.TOffertezustand
            .Select(z => new { z.OfzId, z.OfzNaturalId, z.OfzSchlusszustand, z.OfzPublikation })
            .ToListAsync();

        return new Zustandsregeln(
            zustaende.Where(z => z.OfzSchlusszustand).Select(z => z.OfzId).ToHashSet(),
            zustaende.Where(z => z.OfzNaturalId == NaturalIdWiderrufen).Select(z => z.OfzId).ToHashSet(),
            zustaende.Where(z => z.OfzNaturalId == NaturalIdZuschlag).Select(z => z.OfzId).ToHashSet(),
            zustaende.Where(z => z.OfzNaturalId == NaturalIdZuschlagPraequali).Select(z => z.OfzId).ToHashSet(),
            zustaende.Where(z => z.OfzPublikation).Select(z => z.OfzId).ToHashSet());
    }

    /// <summary>
    /// Die Präqualifikation ist eine Publikation pro Ausschreibung, die Auswertungen zählen sie einmal: Alle Zuschläge
    /// Präqualifikation der Ausschreibung müssen dasselbe «Eingereicht» und dasselbe Datum (Publikation) haben.
    /// </summary>
    private static void PruefePraequaliDaten(List<Los> loseList, Zustandsregeln regeln)
    {
        var verschiedeneDaten = loseList
            .SelectMany(l => l.Offerten)
            .SelectMany(o => o.Offertenzustaende)
            .Where(z => regeln.ZuschlagPraequaliIds.Contains(z.OfzId))
            .Select(z => (z.Eingereicht, Datum: z.Datum?.Date))
            .Distinct()
            .Count();

        if (verschiedeneDaten > 1)
        {
            throw new InvalidOperationException(FehlerPraequaliDatenUnterschiedlich);
        }
    }

    /// <summary>Lädt den gespeicherten Objektbaum der Ausschreibung als Vergleichsbasis für den Abgleich.</summary>
    private static async Task<Bestand> LadeBestand(DoeAContext dbContext, Guid ausId, List<Los> loseList)
    {
        var lose = await dbContext.TLos.Where(l => l.LosAusId == ausId).ToListAsync();
        var losIds = lose.Select(l => l.LosId).ToList();

        var offerten = await dbContext.TOfferte
            .Where(o => losIds.Contains(o.OffLosId)).ToListAsync();
        var offerteIds = offerten.Select(o => o.OffId).ToList();

        var zustaende = await dbContext.TOfferteOffertezustand
            .Where(z => offerteIds.Contains(z.OozOffId)).ToListAsync();
        var bkpNummern = await dbContext.TLosBkpNummer
            .Where(b => losIds.Contains(b.LbnLosId)).ToListAsync();

        return new Bestand(lose, offerten, zustaende, bkpNummern)
        {
            BehalteneOfferteIds = loseList.SelectMany(l => l.Offerten).Select(o => o.Id).ToHashSet(),
            BehalteneZustandIds = loseList
                .SelectMany(l => l.Offerten)
                .SelectMany(o => o.Offertenzustaende)
                .Select(z => z.Id)
                .ToHashSet()
        };
    }

    /// <summary>Entfernt alle nicht mehr übergebenen Lose samt Offerten, Offertenzuständen und BKP-Zuordnungen.</summary>
    private static void EntferneGeloeschteLose(DoeAContext dbContext, Bestand bestand, List<Los> loseList)
    {
        var behaltenIds = loseList.Select(l => l.Id).ToHashSet();
        var geloeschte = bestand.Lose.Where(l => !behaltenIds.Contains(l.LosId)).ToList();
        if (geloeschte.Count == 0)
        {
            return;
        }

        // Offerten eines gelöschten Los, die der Client einem anderen Los zugeordnet hat, bleiben erhalten.
        var geloeschteIds = geloeschte.Select(l => l.LosId).ToHashSet();
        EntferneOfferten(dbContext, bestand, bestand.Offerten
            .Where(o => geloeschteIds.Contains(o.OffLosId) && !bestand.BehalteneOfferteIds.Contains(o.OffId))
            .ToList());
        dbContext.TLosBkpNummer.RemoveRange(bestand.BkpNummern.Where(b => geloeschteIds.Contains(b.LbnLosId)));
        dbContext.TLos.RemoveRange(geloeschte);
    }

    /// <summary>Entfernt Offerten samt ihren Offertenzuständen.</summary>
    private static void EntferneOfferten(DoeAContext dbContext, Bestand bestand, List<TOfferte> offerten)
    {
        if (offerten.Count == 0)
        {
            return;
        }

        var offerteIds = offerten.Select(o => o.OffId).ToHashSet();
        dbContext.TOfferteOffertezustand.RemoveRange(bestand.Zustaende.Where(z => offerteIds.Contains(z.OozOffId)));
        dbContext.TOfferte.RemoveRange(offerten);
    }

    /// <summary>Legt ein Los an oder aktualisiert es und gleicht anschliessend Offerten und BKP-Nummern ab.</summary>
    private async Task SpeichereLos(DoeAContext dbContext, Bestand bestand, Zustandsregeln regeln, Los los, long now)
    {
        var tLos = IstNeu(los.Id, bestand.LosIds) ? null : bestand.Lose.Single(l => l.LosId == los.Id);

        los.Zustand = BestimmeLoszustand(los, regeln);

        // Eingereicht und Verzögerung gehören zum Abbruch; ohne Abbruchdatum verfälschten sie die Auswertungen.
        if (los.Abbruchdatum == null)
        {
            los.Eingereicht = null;
            los.VerzoegerungDurchAuftraggeber = false;
        }

        if (tLos == null)
        {
            StampForSave(los, true, now);
            await Validate(new LosValidator(), los);
            dbContext.TLos.Add(Mapper.ZuEntity(los));
        }
        else if (LosGeaendert(los, tLos))
        {
            PruefeConcurrency(los, tLos.LosChangedDate);
            StampForSave(los, false, now);
            await Validate(new LosValidator(), los);
            tLos.LosBezeichnung = los.Bezeichnung;
            tLos.LosAbbruchdatum = los.Abbruchdatum;
            tLos.LosEingereicht = los.Eingereicht;
            tLos.LosVerzoegerungDurchAuftraggeber = los.VerzoegerungDurchAuftraggeber;
            tLos.LosZustand = los.Zustand.ToString();
            tLos.LosChangedDate = los.ChangedDate;
            tLos.LosChangedUser = los.ChangedUser!;
        }

        await SyncOfferten(dbContext, bestand, regeln, los, now);
        SyncBkpNummern(dbContext, bestand, los, now);
    }

    /// <summary>Gleicht die Offerten (und deren Zustände) eines Los gegen den Bestand ab.</summary>
    private async Task SyncOfferten(DoeAContext dbContext, Bestand bestand, Zustandsregeln regeln, Los los, long now)
    {
        // Löschen nur, was im gesamten Tab nicht mehr vorkommt — sonst würde eine in ein anderes
        // Los verschobene Offerte hier gelöscht und dort gleichzeitig aktualisiert.
        EntferneOfferten(dbContext, bestand, bestand.Offerten
            .Where(o => o.OffLosId == los.Id && !bestand.BehalteneOfferteIds.Contains(o.OffId))
            .ToList());

        foreach (var offerte in los.Offerten)
        {
            offerte.LosId = los.Id;

            // Lookup über den gesamten Bestand, damit ein umgehängter Datensatz gefunden wird.
            var tOff = IstNeu(offerte.Id, bestand.OfferteIds)
                ? null
                : bestand.Offerten.Single(o => o.OffId == offerte.Id);

            if (tOff == null)
            {
                StampForSave(offerte, true, now);
                await Validate(new OfferteValidator(publikationOfzIds: regeln.PublikationIds), offerte);
                dbContext.TOfferte.Add(_offerteMapper.ZuEntity(offerte));
            }
            else if (OfferteGeaendert(offerte, tOff))
            {
                PruefeConcurrency(offerte, tOff.OffChangedDate);
                StampForSave(offerte, false, now);
                await Validate(new OfferteValidator(publikationOfzIds: regeln.PublikationIds), offerte);

                // Auch das Los mitschreiben: die Offerte kann einem anderen Los zugeordnet worden sein.
                tOff.OffLosId = los.Id;

                // AnbId ist durch den OfferteValidator (NotEmpty) garantiert gesetzt; defensiv ?? Guid.Empty.
                tOff.OffAnbId = offerte.AnbId ?? Guid.Empty;
                tOff.OffZuschlagswertChf = offerte.ZuschlagswertChf;
                tOff.OffAnzahlMitarbeiter = offerte.AnzahlMitarbeiter;
                tOff.OffBemerkung = offerte.Bemerkung;
                tOff.OffChangedDate = offerte.ChangedDate;
                tOff.OffChangedUser = offerte.ChangedUser!;
            }

            await SyncZustaende(dbContext, bestand, regeln, offerte, now);
        }
    }

    /// <summary>Gleicht die Offertenzustände einer Offerte gegen den Bestand ab.</summary>
    private async Task SyncZustaende(DoeAContext dbContext, Bestand bestand, Zustandsregeln regeln, Offerte offerte, long now)
    {
        // Analog zu SyncOfferten: nur löschen, was im gesamten Tab nicht mehr vorkommt.
        dbContext.TOfferteOffertezustand.RemoveRange(bestand.Zustaende
            .Where(z => z.OozOffId == offerte.Id && !bestand.BehalteneZustandIds.Contains(z.OozId)));

        foreach (var (index, zustand) in offerte.Offertenzustaende.Index())
        {
            zustand.OffId = offerte.Id;

            // Lookup über den gesamten Bestand, damit ein umgehängter Datensatz gefunden wird.
            var tZustand = IstNeu(zustand.Id, bestand.ZustandIds)
                ? null
                : bestand.Zustaende.Single(z => z.OozId == zustand.Id);

            if (tZustand == null)
            {
                // Eindeutiges InsertedDate in Listenreihenfolge, damit bei gleichem Datum überall der zuletzt
                // erfasste Zustand gilt (OfferteHelper.LetzterZustand) und nicht die zufällige Id.
                StampForSave(zustand, true, now + index);
                await Validate(new OfferteOffertezustandValidator(publikationOfzIds: regeln.PublikationIds), zustand);
                dbContext.TOfferteOffertezustand.Add(_offerteOffertezustandMapper.ZuEntity(zustand));
            }
            else if (ZustandGeaendert(zustand, tZustand))
            {
                PruefeConcurrency(zustand, tZustand.OozChangedDate);
                StampForSave(zustand, false, now);
                await Validate(new OfferteOffertezustandValidator(publikationOfzIds: regeln.PublikationIds), zustand);

                // Auch die Offerte mitschreiben: der Zustand kann einer anderen zugeordnet worden sein.
                tZustand.OozOffId = zustand.OffId;
                tZustand.OozOfzId = zustand.OfzId;
                tZustand.OozDatum = ZustandDatum(zustand);
                tZustand.OozEingereicht = zustand.Eingereicht;
                tZustand.OozVerzoegerungDurchAuftraggeber = zustand.VerzoegerungDurchAuftraggeber;
                tZustand.OozChangedDate = zustand.ChangedDate;
                tZustand.OozChangedUser = zustand.ChangedUser!;
            }
        }
    }

    /// <summary>Gleicht die BKP-Nummern eines Los gegen den Bestand ab (Vergleich über die BkpId, nicht über die Id).</summary>
    private void SyncBkpNummern(DoeAContext dbContext, Bestand bestand, Los los, long now)
    {
        var dbBkps = bestand.BkpNummern.Where(b => b.LbnLosId == los.Id).ToList();

        dbContext.TLosBkpNummer.RemoveRange(
            dbBkps.Where(b => los.BkpNummern.All(x => x.BkpId != b.LbnBkpId)));

        foreach (var lbn in los.BkpNummern.Where(x => dbBkps.All(b => b.LbnBkpId != x.BkpId)))
        {
            lbn.LosId = los.Id;
            StampForSave(lbn, true, now);
            dbContext.TLosBkpNummer.Add(_losBkpNummerMapper.ZuEntity(lbn));
        }
    }

    /// <summary>Ob der aktuellste Zustand einer Offerte zu einer der übergebenen Zustandsarten gehört.</summary>
    private static bool LetzterZustandGehoertZu(Offerte offerte, HashSet<Guid> ofzIds)
    {
        var letzter = offerte.LetzterZustand();
        return letzter != null && ofzIds.Contains(letzter.OfzId);
    }

    /// <summary>
    /// Bestimmt den Loszustand anhand der Geschäftsregeln:
    /// gesetztes Abbruchdatum => Abgebrochen (Vorrang); sonst eine widerrufene Offerte ohne gültigen Zuschlag an eine
    /// andere => Widerrufen; sonst alle Offerten in einem Schlusszustand => Abgeschlossen; andernfalls Offen.
    /// </summary>
    private static EnumLosZustand BestimmeLoszustand(Los los, Zustandsregeln regeln)
    {
        if (los.Abbruchdatum != null)
        {
            return EnumLosZustand.Abgebrochen;
        }

        // Nach dem Widerruf kann der Zuschlag neu an eine andere Offerte gehen: Dann ist das Los nicht mehr widerrufen.
        if (los.Offerten.Any(o => LetzterZustandGehoertZu(o, regeln.WiderrufIds)) &&
            !los.Offerten.Any(o => LetzterZustandGehoertZu(o, regeln.ZuschlagIds)))
        {
            return EnumLosZustand.Widerrufen;
        }

        // Abgeschlossen nur, wenn Offerten vorhanden sind und alle einen Schlusszustand haben.
        if (los.Offerten.Count > 0 && los.Offerten.All(o => LetzterZustandGehoertZu(o, regeln.SchlusszustandIds)))
        {
            return EnumLosZustand.Abgeschlossen;
        }

        return EnumLosZustand.Offen;
    }

    /// <summary>Neu ist ein Datensatz, dessen client-seitig vergebene Id im Bestand noch nicht vorkommt.</summary>
    private static bool IstNeu(Guid id, HashSet<Guid> bestandIds) => id == Guid.Empty || !bestandIds.Contains(id);

    /// <summary>Ob sich das Los gegenüber dem gespeicherten Stand geändert hat.</summary>
    private static bool LosGeaendert(Los los, TLos tLos) =>
        los.Bezeichnung != tLos.LosBezeichnung ||
        los.Abbruchdatum != tLos.LosAbbruchdatum ||
        los.Eingereicht != tLos.LosEingereicht ||
        los.VerzoegerungDurchAuftraggeber != tLos.LosVerzoegerungDurchAuftraggeber ||
        los.Zustand.ToString() != tLos.LosZustand;

    /// <summary>Ob sich die Offerte gegenüber dem gespeicherten Stand geändert hat (inkl. Umhängen an ein anderes Los).</summary>
    private static bool OfferteGeaendert(Offerte offerte, TOfferte tOff) =>
        offerte.LosId != tOff.OffLosId ||
        (offerte.AnbId ?? Guid.Empty) != tOff.OffAnbId ||
        offerte.ZuschlagswertChf != tOff.OffZuschlagswertChf ||
        offerte.AnzahlMitarbeiter != tOff.OffAnzahlMitarbeiter ||
        offerte.Bemerkung != tOff.OffBemerkung;

    /// <summary>Ob sich der Offertenzustand gegenüber dem gespeicherten Stand geändert hat (inkl. Umhängen an eine andere Offerte).</summary>
    private static bool ZustandGeaendert(OfferteOffertezustand zustand, TOfferteOffertezustand tZustand) =>
        zustand.OffId != tZustand.OozOffId ||
        zustand.OfzId != tZustand.OozOfzId ||
        ZustandDatum(zustand) != tZustand.OozDatum ||
        zustand.Eingereicht != tZustand.OozEingereicht ||
        zustand.VerzoegerungDurchAuftraggeber != tZustand.OozVerzoegerungDurchAuftraggeber;

    private static DateOnly ZustandDatum(OfferteOffertezustand zustand) =>
        zustand.Datum.HasValue ? DateOnly.FromDateTime(zustand.Datum.Value) : default;

    /// <summary>Ermittelt, ob die Ausschreibung die Verfahrensart "Freihändig" (Zuschlagsempfänger) hat.</summary>
    private static async Task<bool> IstFreihaendig(DoeAContext dbContext, Guid ausId)
    {
        var naturalId = await (from aus in dbContext.TAusschreibung
                               join vea in dbContext.TVerfahrensart on aus.AusVeaId equals vea.VeaId
                               where aus.AusId == ausId
                               select vea.VeaNaturalId).FirstOrDefaultAsync();

        return VerfahrensartRegeln.IstZuschlagsempfaengerModus(naturalId);
    }
}
