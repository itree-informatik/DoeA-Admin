// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Constants;
using DoeA.Data.Enums;
using DoeA.Data.Mapper.Exportgruppen;
using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO;
using DoeA.Data.Models.DTO.Exportgruppen;
using DoeA.Interfaces;
using DoeA.Validator;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DoeA.Services;

/// <summary>
/// Verwaltet die gespeicherten Filterdefinitionen (Exportgruppen) und führt sie gegen den
/// Ausschreibungsbestand aus — für die Ergebnisliste und den erweiterten Excel-Export.
/// </summary>
public class ExportgruppeService : IExportgruppeService
{
    private const string DokumentartOfferten = nameof(EnumDokumentart.Offerten);
    private const string DokumentartPublikation = nameof(EnumDokumentart.Publikation);
    private const string DokumentartUnterlagen = nameof(EnumDokumentart.Unterlagen);

    // Rolle eines Sachbearbeiter-Filters = Organisation, aus der die Grunddaten das Feld befüllen.
    private const string RolleBbl = nameof(EnumOrganisationszugehoerigkeit.BBL);
    private const string RolleDoeA = nameof(EnumOrganisationszugehoerigkeit.DoeA);
    private const string RolleRd = nameof(EnumOrganisationszugehoerigkeit.RD);

    /// <summary>NaturalId der Schweiz — jedes andere Land zählt in der Offerten-Auswertung als Ausland.</summary>
    private const string LandSchweiz = "Schweiz";

    /// <summary>Inhalt einer gesetzten Zelle der Sprachmatrix im Export.</summary>
    private const string MatrixZelleGesetzt = "X";

    private const string SpracheDe = "DE";
    private const string SpracheEn = "EN";
    private const string SpracheFr = "FR";
    private const string SpracheIt = "IT";

    /// <summary>Trenner der konkatenierten Sammelspalten (Kantone, CPV- und BKP-Nummern).</summary>
    private const string Trennzeichen = ", ";

    /// <summary>Anzahl Offerten eines Loses nach Sprachregion des Anbieter-Kantons bzw. Ausland.</summary>
    private sealed record OffertenHerkunft(int De, int Fr, int It, int Ausland);

    /// <summary>Bestand der n:m-Verknüpfungstabellen einer Exportgruppe, Sachbearbeiter je Rolle (bei Neuanlage überall leer).</summary>
    private sealed record Verknuepfungen(
        List<TExportgruppeAmt> Amt,
        List<TExportgruppeKanton> Kanton,
        List<TExportgruppeLeistungsart> Leistungsart,
        List<TExportgruppeProjektart> Projektart,
        List<TExportgruppeRessort> Ressort,
        List<TExportgruppeRolleInAusschreibung> RolleInAusschreibung,
        List<TExportgruppeSachbearbeiter> SachbearbeiterDoeA,
        List<TExportgruppeSachbearbeiter> SachbearbeiterRd,
        List<TExportgruppeSachbearbeiter> SachbearbeiterBbl,
        List<TExportgruppeVerfahrensart> Verfahrensart);

    private readonly AbstractValidator<Exportgruppe> _validator = new ExportgruppeValidator();
    private readonly IBaseFunctionService _baseFunctionService;
    private readonly IDbContextFactory<DoeAContext> _dbFactory;
    private readonly IErrorHandlerService _errorHandlerService;
    private readonly ExportgruppeMapper _exportgruppeMapper;

    public ExportgruppeService(IErrorHandlerService errorHandlerService, IDbContextFactory<DoeAContext> dbFactory, ExportgruppeMapper exportgruppeMapper, IBaseFunctionService baseFunctionService)
    {
        _dbFactory = dbFactory;
        _exportgruppeMapper = exportgruppeMapper;
        _baseFunctionService = baseFunctionService;
        _errorHandlerService = errorHandlerService;
    }

    /// <summary>Liefert alle Exportgruppen als Listeneinträge, alphabetisch nach Name.</summary>
    public async Task<List<ExportgruppeListItem>?> GetAllAsync()
    {
        return await _errorHandlerService.HandleListAsync(async () =>
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var entities = await db.TExportgruppe
                .OrderBy(e => e.ExpName)
                .Select(e => new ExportgruppeListItem
                {
                    Id = e.ExpId,
                    Name = e.ExpName,
                    ChangedDate = e.ExpChangedDate,
                    ChangedUser = e.ExpChangedUser,
                    InsertedDate = e.ExpInsertedDate,
                    InsertedUser = e.ExpInsertedUser
                })
                .ToListAsync();

            return new ServiceResult<ExportgruppeListItem>
            {
                Items = entities,
                Result = GeneralConstants.Ok
            };
        });
    }

    /// <summary>Lädt eine Exportgruppe samt den Ids ihrer n:m-Filterlisten; null, wenn es sie nicht (mehr) gibt.</summary>
    public async Task<Exportgruppe?> GetAsync(Guid expId)
    {
        return await _errorHandlerService.HandleAsync(async () =>
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var entity = await db.TExportgruppe.AsNoTracking().FirstOrDefaultAsync(e => e.ExpId == expId);
            var model = entity == null ? null : _exportgruppeMapper.ZuModel(entity);

            if (model != null)
            {
                await LadeFilterIdsAsync(db, expId, model);
            }

            return new ServiceResult<Exportgruppe>
            {
                Item = model,
                Result = GeneralConstants.Ok
            };
        });
    }

    /// <summary>
    /// Speichert eine Exportgruppe (neu oder bestehend) inkl. Diff-Sync der n:m-Filterlisten,
    /// analog <see cref="AusschreibungService.Save"/> (CPV-/Sprache-Sync).
    /// Die Konkurrenzprüfung sieht nur die Skalarfelder — reine Listenänderungen werden deshalb
    /// separat erkannt, damit sie weder verworfen noch fälschlich als Konflikt gemeldet werden.
    /// </summary>
    public async Task<Exportgruppe> SaveAsync(Exportgruppe model)
    {
        return await _errorHandlerService.HandleAsync(async () =>
        {
            var istNeu = model.Id == Guid.Empty;

            await using var db = await _dbFactory.CreateDbContextAsync();

            // Bestand der Verknüpfungstabellen laden: wird für den Sync gebraucht und
            // (bei Update) für die Listen-Änderungsprüfung. Bei Neuanlage (Id = Empty) leer.
            var bestand = await LadeVerknuepfungenAsync(db, model.Id);

            TExportgruppe tExp;

            if (istNeu)
            {
                await _baseFunctionService.Validate(_validator, model, true);
                tExp = _exportgruppeMapper.ZuEntity(model);
                db.TExportgruppe.Add(tExp);
            }
            else
            {
                var status = await _baseFunctionService.CheckChangesAndConcurrency<TExportgruppe>(model, "Exp");
                if (status is GeneralConstants.ConcurrencyError or GeneralConstants.NotFoundError)
                {
                    throw new InvalidOperationException(status);
                }

                if (status == GeneralConstants.NothingChanged && !HatListenaenderungen(bestand, model))
                {
                    return new ServiceResult<Exportgruppe>
                    {
                        Item = model,
                        Result = GeneralConstants.Ok
                    };
                }

                await _baseFunctionService.Validate(_validator, model, false);
                tExp = _exportgruppeMapper.ZuEntity(model);
                db.Entry(tExp).State = EntityState.Modified;
            }

            SyncVerknuepfungen(db, bestand, model, tExp);

            await db.SaveChangesAsync();

            return new ServiceResult<Exportgruppe>
            {
                Item = model,
                Result = GeneralConstants.Ok
            };
        });
    }

    /// <summary>
    /// Speichert gezielt NUR die Spaltensichtbarkeit (Auto-Save beim Schliessen des
    /// Spalten-Popups) — andere, noch ungespeicherte Formularänderungen bleiben unberührt.
    /// Bewusst ohne Konkurrenzprüfung: es werden ausschliesslich ExpSpalten und die
    /// Änderungs-Auditfelder überschrieben.
    /// </summary>
    public async Task<Exportgruppe> SaveSpaltenAsync(Exportgruppe model)
    {
        return await _errorHandlerService.HandleAsync(async () =>
        {
            // Stempelt ChangedDate/ChangedUser am Modell (und validiert)
            await _baseFunctionService.Validate(_validator, model, false);

            await using var db = await _dbFactory.CreateDbContextAsync();
            var entity = await db.TExportgruppe.SingleAsync(e => e.ExpId == model.Id);
            entity.ExpSpalten = model.Spalten;
            entity.ExpChangedDate = model.ChangedDate;
            entity.ExpChangedUser = model.ChangedUser!;
            await db.SaveChangesAsync();

            return new ServiceResult<Exportgruppe>
            {
                Item = model,
                Result = GeneralConstants.Ok
            };
        });
    }

    /// <summary>
    /// Speichert gezielt NUR den Namen (Auto-Save beim Verlassen des Namensfelds) — andere,
    /// noch ungespeicherte Formularänderungen bleiben unberührt. Bewusst ohne Konkurrenzprüfung:
    /// es werden ausschliesslich ExpName und die Änderungs-Auditfelder überschrieben.
    /// </summary>
    public async Task<Exportgruppe> SaveNameAsync(Exportgruppe model)
    {
        return await _errorHandlerService.HandleAsync(async () =>
        {
            // Stempelt ChangedDate/ChangedUser am Modell (und validiert)
            await _baseFunctionService.Validate(_validator, model, false);

            await using var db = await _dbFactory.CreateDbContextAsync();
            var entity = await db.TExportgruppe.SingleAsync(e => e.ExpId == model.Id);
            entity.ExpName = model.Name;
            entity.ExpChangedDate = model.ChangedDate;
            entity.ExpChangedUser = model.ChangedUser!;
            await db.SaveChangesAsync();

            return new ServiceResult<Exportgruppe>
            {
                Item = model,
                Result = GeneralConstants.Ok
            };
        });
    }

    /// <summary>
    /// Löscht eine Exportgruppe; die Zeilen der Verknüpfungstabellen entfernt die Datenbank
    /// per ON DELETE CASCADE.
    /// </summary>
    public async Task<Exportgruppe> DeleteAsync(Guid expId)
    {
        return await _errorHandlerService.HandleAsync(async () =>
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var entity = await db.TExportgruppe.SingleAsync(e => e.ExpId == expId);
            db.TExportgruppe.Remove(entity);
            await db.SaveChangesAsync();
            return new ServiceResult<Exportgruppe>
            {
                Item = _exportgruppeMapper.ZuModel(entity),
                Result = GeneralConstants.Ok
            };
        });
    }

    /// <summary>
    /// Führt die Filterdefinition gegen den Ausschreibungsbestand aus und liefert eine Zeile
    /// pro Ausschreibung für die Ergebnisliste.
    /// </summary>
    public async Task<List<Ergebnis>?> GetErgebnisAsync(Exportgruppe model)
    {
        return await _errorHandlerService.HandleListAsync(async () =>
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var items = await ErstelleErgebnisAbfrage(db, model).ToListAsync();
            await ErgaenzeKantoneAsync(db, items);

            return new ServiceResult<Ergebnis>
            {
                Items = items,
                Result = GeneralConstants.Ok
            };
        });
    }

    /// <summary>Excel-Export «Alle Daten»: gleiche Selektion wie <see cref="GetErgebnisAsync"/>, aber eine Zeile pro Offertzustand (Left Join bis Anbieter, deshalb erscheinen auch Ausschreibungen ohne Lose, Lose ohne Offerten und Offerten ohne Zustand).</summary>
    public async Task<List<ErgebnisAlleDaten>?> GetErgebnisAlleDatenAsync(Exportgruppe model)
    {
        return await _errorHandlerService.HandleListAsync(async () =>
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var items = await ErstelleAlleDatenAbfrage(db, model).ToListAsync();
            await ErgaenzeAggregatSpaltenAsync(db, items);
            MarkiereAktuelleZustaende(items);

            return new ServiceResult<ErgebnisAlleDaten>
            {
                Items = items,
                Result = GeneralConstants.Ok
            };
        });
    }

    /// <summary>
    /// Gemeinsame Filterdefinition für Grid-Ergebnis und erweiterten Export («Alle Daten») —
    /// jeder Filter wirkt nur, wenn er befüllt ist (UND zwischen den Filtern, ODER innerhalb).
    /// </summary>
    private static IQueryable<TAusschreibung> GefilterteAusschreibungen(DoeAContext db, Exportgruppe model)
    {
        return db.TAusschreibung.AsNoTracking().Where(aus =>
            (model.AmtIds.Count == 0 || (aus.AusAmtId != null && model.AmtIds.Contains(aus.AusAmtId.Value))) &&
            // Kanton: trifft zu, wenn irgendein Anbieter aus einem gewählten Kanton
            // eine Offerte eingereicht hat (Los -> Offerte -> Anbieter -> Ortschaft).
            // Bewusst nur über diesen Weg: Ausschreibungen ohne Offerten haben keinen Kantonsbezug
            // und fallen bei gesetztem Kantonfilter daher immer aus dem Ergebnis.
            (model.KantonIds.Count == 0 ||
                db.TOfferte.Any(off => off.OffLos.LosAusId == aus.AusId &&
                                       off.OffAnb.AnbOrt.OrtKanId != null &&
                                       model.KantonIds.Contains(off.OffAnb.AnbOrt.OrtKanId.Value))) &&
            (model.LeistungsartIds.Count == 0 || (aus.AusLeaId != null && model.LeistungsartIds.Contains(aus.AusLeaId.Value))) &&
            (model.ProjektartIds.Count == 0 || model.ProjektartIds.Contains(aus.AusPraId)) &&
            (model.RessortIds.Count == 0 || (aus.AusResId != null && model.RessortIds.Contains(aus.AusResId.Value))) &&
            (model.RolleInAusschreibungIds.Count == 0 || (aus.AusRiaId != null && model.RolleInAusschreibungIds.Contains(aus.AusRiaId.Value))) &&
            (model.VerfahrensartIds.Count == 0 || model.VerfahrensartIds.Contains(aus.AusVeaId)) &&
            (model.SachbearbeiterDoeAIds.Count == 0 || (aus.AusSacDoeAId != null && model.SachbearbeiterDoeAIds.Contains(aus.AusSacDoeAId.Value))) &&
            (model.SachbearbeiterRdIds.Count == 0 || (aus.AusSacRdId != null && model.SachbearbeiterRdIds.Contains(aus.AusSacRdId.Value))) &&
            (model.SachbearbeiterBblIds.Count == 0 || (aus.AusSacBblId != null && model.SachbearbeiterBblIds.Contains(aus.AusSacBblId.Value))) &&
            (model.Jahr == null || aus.AusJahr == model.Jahr) &&
            (model.AbschlussJahr == null || aus.AusAbschlussJahr == model.AbschlussJahr) &&
            // Publikationsdatum: von/bis wirken einzeln («alle ab» bzw. «alle bis»);
            // Ausschreibungen ohne Publikationsdatum werden bei gesetztem Filter ausgeschlossen.
            (model.PublikationVon == null || aus.AusPublikation >= model.PublikationVon) &&
            (model.PublikationBis == null || aus.AusPublikation <= model.PublikationBis) &&
            (model.Personalverleih == null || aus.AusPersonalverleih == model.Personalverleih) &&
            (model.Abgeschlossen == null || aus.AusAbgeschlossen == model.Abgeschlossen) &&
            (model.BeschaffungDezentral == null || aus.AusBeschaffungDezentral == model.BeschaffungDezentral));
    }

    /// <summary>Eine Zeile pro gefilterter Ausschreibung, angereichert um die bezeichnenden Stammdaten.</summary>
    private static IQueryable<Ergebnis> ErstelleErgebnisAbfrage(DoeAContext db, Exportgruppe model)
    {
        return from aus in GefilterteAusschreibungen(db, model)
               join amt in db.TAmt on aus.AusAmtId equals amt.AmtId into amtJ
               from amt in amtJ.DefaultIfEmpty()
               join lea in db.TLeistungsart on aus.AusLeaId equals lea.LeaId into leaJ
               from lea in leaJ.DefaultIfEmpty()
               join vea in db.TVerfahrensart on aus.AusVeaId equals vea.VeaId
               join pra in db.TProjektart on aus.AusPraId equals pra.PraId
               join ria in db.TRolleInAusschreibung on aus.AusRiaId equals ria.RiaId into riaJ
               from ria in riaJ.DefaultIfEmpty()
               join res in db.TRessort on aus.AusResId equals res.ResId into resJ
               from res in resJ.DefaultIfEmpty()
               join sacD in db.TSachbearbeiter on aus.AusSacDoeAId equals sacD.SacId into sacDj
               from sacD in sacDj.DefaultIfEmpty()
               join sacR in db.TSachbearbeiter on aus.AusSacRdId equals sacR.SacId into sacRj
               from sacR in sacRj.DefaultIfEmpty()
               join sacB in db.TSachbearbeiter on aus.AusSacBblId equals sacB.SacId into sacBj
               from sacB in sacBj.DefaultIfEmpty()
               orderby aus.AusJahr descending, aus.AusNummerStr
               select new Ergebnis
               {
                   Id = aus.AusId,
                   Jahr = aus.AusJahr,
                   NummerStr = aus.AusNummerStr ?? string.Empty,
                   Bezeichnung = aus.AusBezeichnung ?? string.Empty,
                   SimapNr = aus.AusSimapNr,
                   Amt = amt != null ? amt.AmtBezeichnung : null,
                   Ressort = res != null ? res.ResBezeichnung : null,
                   Leistungsart = lea != null ? lea.LeaBezeichnung : null,
                   Projektart = pra.PraBezeichnung,
                   Verfahrensart = vea.VeaBezeichnung,
                   RolleInAusschreibung = ria != null ? ria.RiaBezeichnung : null,
                   SachbearbeiterDoeA = sacD != null ? sacD.SacKurzbezeichnung : null,
                   SachbearbeiterRd = sacR != null ? sacR.SacKurzbezeichnung : null,
                   SachbearbeiterBbl = sacB != null ? sacB.SacKurzbezeichnung : null,
                   Publikation = aus.AusPublikation,
                   OffertenFrist = aus.AusOffertenFrist,
                   Abgeschlossen = aus.AusAbgeschlossen,
                   Extern = aus.AusExtern,
                   Personalverleih = aus.AusPersonalverleih,
                   BeschaffungDezentral = aus.AusBeschaffungDezentral
               };
    }

    /// <summary>
    /// Eine Zeile pro Offertzustand der gefilterten Ausschreibungen (Los, Offerte, Offertzustand,
    /// Anbieter jeweils als Left Join) mit allen fachlichen Attributen für den erweiterten Export;
    /// die Zustände einer Offerte stehen chronologisch untereinander.
    /// </summary>
    private static IQueryable<ErgebnisAlleDaten> ErstelleAlleDatenAbfrage(DoeAContext db, Exportgruppe model)
    {
        return from aus in GefilterteAusschreibungen(db, model)
               join amt in db.TAmt on aus.AusAmtId equals amt.AmtId into amtJ
               from amt in amtJ.DefaultIfEmpty()
               join lea in db.TLeistungsart on aus.AusLeaId equals lea.LeaId into leaJ
               from lea in leaJ.DefaultIfEmpty()
               join vea in db.TVerfahrensart on aus.AusVeaId equals vea.VeaId
               join pra in db.TProjektart on aus.AusPraId equals pra.PraId
               join ria in db.TRolleInAusschreibung on aus.AusRiaId equals ria.RiaId into riaJ
               from ria in riaJ.DefaultIfEmpty()
               join res in db.TRessort on aus.AusResId equals res.ResId into resJ
               from res in resJ.DefaultIfEmpty()
               join sacD in db.TSachbearbeiter on aus.AusSacDoeAId equals sacD.SacId into sacDj
               from sacD in sacDj.DefaultIfEmpty()
               join sacR in db.TSachbearbeiter on aus.AusSacRdId equals sacR.SacId into sacRj
               from sacR in sacRj.DefaultIfEmpty()
               join sacB in db.TSachbearbeiter on aus.AusSacBblId equals sacB.SacId into sacBj
               from sacB in sacBj.DefaultIfEmpty()
               join haupt in db.TAusschreibung on aus.AusAusId equals haupt.AusId into hauptJ
               from haupt in hauptJ.DefaultIfEmpty()
               join vor in db.TAusschreibung on aus.AusVorankuendigungId equals vor.AusId into vorJ
               from vor in vorJ.DefaultIfEmpty()
               join rfi in db.TAusschreibung on aus.AusRfiId equals rfi.AusId into rfiJ
               from rfi in rfiJ.DefaultIfEmpty()
               join los in db.TLos on aus.AusId equals los.LosAusId into losJ
               from los in losJ.DefaultIfEmpty()
               join off in db.TOfferte on los.LosId equals off.OffLosId into offJ
               from off in offJ.DefaultIfEmpty()
               join ooz in db.TOfferteOffertezustand on off.OffId equals ooz.OozOffId into oozJ
               from ooz in oozJ.DefaultIfEmpty()
               join ofz in db.TOffertezustand on ooz.OozOfzId equals ofz.OfzId into ofzJ
               from ofz in ofzJ.DefaultIfEmpty()
               join anb in db.TAnbieter on off.OffAnbId equals anb.AnbId into anbJ
               from anb in anbJ.DefaultIfEmpty()
               join ort in db.TOrtschaft on anb.AnbOrtId equals ort.OrtId into ortJ
               from ort in ortJ.DefaultIfEmpty()
               join kan in db.TKanton on ort.OrtKanId equals kan.KanId into kanJ
               from kan in kanJ.DefaultIfEmpty()
               join lan in db.TLand on ort.OrtLanId equals lan.LanId into lanJ
               from lan in lanJ.DefaultIfEmpty()
               orderby aus.AusJahr descending, aus.AusNummerStr, los.LosBezeichnung, anb.AnbName, off.OffId, ooz.OozDatum, ooz.OozInsertedDate, ooz.OozId
               select new ErgebnisAlleDaten
               {
                   AusId = aus.AusId,
                   LosId = los != null ? los.LosId : null,
                   OffId = off != null ? off.OffId : null,
                   OozInsertedDate = ooz != null ? ooz.OozInsertedDate : null,
                   OozId = ooz != null ? ooz.OozId : null,
                   Jahr = aus.AusJahr,
                   NummerStr = aus.AusNummerStr ?? string.Empty,
                   Bezeichnung = aus.AusBezeichnung ?? string.Empty,
                   SimapNr = aus.AusSimapNr,
                   Dienststellennummer = amt != null ? amt.AmtDienststellennummer : null,
                   Amt = amt != null ? amt.AmtBezeichnung : null,
                   Ressort = res != null ? res.ResBezeichnung : null,
                   Leistungsart = lea != null ? lea.LeaBezeichnung : null,
                   Projektart = pra.PraBezeichnung,
                   Verfahrensart = vea.VeaBezeichnung,
                   RolleInAusschreibung = ria != null ? ria.RiaBezeichnung : null,
                   Hauptprojekt = haupt != null ? haupt.AusNummerStr : null,
                   Vorankuendigung = vor != null ? vor.AusNummerStr : null,
                   Rfi = rfi != null ? rfi.AusNummerStr : null,
                   SachbearbeiterDoeA = sacD != null ? sacD.SacKurzbezeichnung : null,
                   SachbearbeiterRd = sacR != null ? sacR.SacKurzbezeichnung : null,
                   SachbearbeiterBbl = sacB != null ? sacB.SacKurzbezeichnung : null,
                   Extern = aus.AusExtern,
                   Personalverleih = aus.AusPersonalverleih,
                   BeschaffungDezentral = aus.AusBeschaffungDezentral,
                   Eingereicht = aus.AusEingereicht,
                   Publikation = aus.AusPublikation,
                   VerzoegerungDurchAuftraggeber = aus.AusVerzoegerungDurchAuftraggeber,
                   EingereichtWiderruf = aus.AusEingereichtWiderruf,
                   PublikationWiderruf = aus.AusPublikationWiderruf,
                   VerzoegerungDurchAuftraggeberWiderruf = aus.AusVerzoegerungDurchAuftraggeberWiderruf,
                   EingabefristTeilnahme = aus.AusEingabefristTeilnahme,
                   PflichtenhefterPraequali = aus.AusPflichtenhefterPraequali,
                   AnzahlTeilnehmer = aus.AusAnzahlTeilnehmer,
                   VersandPh2 = aus.AusVersandPh2,
                   Pflichtenhefter = aus.AusPflichtenhefter,
                   OffertenFrist = aus.AusOffertenFrist,
                   AbschlussJahr = aus.AusAbschlussJahr,
                   Abgeschlossen = aus.AusAbgeschlossen,
                   Standort = aus.AusStandort,
                   Bemerkung = aus.AusBemerkung,
                   Aktiv = aus.AusAktiv,
                   LosBezeichnung = los != null ? los.LosBezeichnung : null,
                   LosZustand = los != null ? los.LosZustand : null,
                   LosEingereicht = los != null ? los.LosEingereicht : null,
                   LosAbbruchdatum = los != null ? los.LosAbbruchdatum : null,
                   LosVerzoegerungDurchAuftraggeber = los != null ? los.LosVerzoegerungDurchAuftraggeber : (bool?)null,
                   OfferteZuschlagswertChf = off != null ? off.OffZuschlagswertChf : null,
                   OfferteAnzahlMitarbeitende = off != null ? off.OffAnzahlMitarbeiter : null,
                   OfferteBemerkung = off != null ? off.OffBemerkung : null,
                   OfferteZustand = ofz != null ? ofz.OfzBezeichnung : null,
                   OfferteZustandDatum = ooz != null ? ooz.OozDatum : null,
                   OfferteZustandEingereicht = ooz != null ? ooz.OozEingereicht : null,
                   OfferteZustandVerzoegerungDurchAuftraggeber = ooz != null ? ooz.OozVerzoegerungDurchAuftraggeber : (bool?)null,
                   AnbieterName = anb != null ? anb.AnbName : null,
                   AnbieterAdresse = anb != null ? anb.AnbAdresse : null,
                   AnbieterAdresse2 = anb != null ? anb.AnbAdresse2 : null,
                   AnbieterAdresse3 = anb != null ? anb.AnbAdresse3 : null,
                   AnbieterPlz = ort != null ? ort.OrtPlz : null,
                   AnbieterOrt = ort != null ? ort.OrtOrt : null,
                   AnbieterKanton = kan != null ? kan.KanKurzbezeichnung : null,
                   AnbieterLand = lan != null ? lan.LanBezeichnung : null,
                   AnbieterKantonSprache = kan != null ? kan.KanSpr.SprBezeichnung : null,
                   AnbieterAktiv = anb != null ? anb.AnbAktiv : (bool?)null
               };
    }

    /// <summary>
    /// Befüllt die Kanton-Spalte nachgelagert: alle Kantone der Offerten-Anbieter je
    /// Ausschreibung (distinct, kommasepariert).
    /// </summary>
    private static async Task ErgaenzeKantoneAsync(DoeAContext db, List<Ergebnis> items)
    {
        if (items.Count == 0)
        {
            return;
        }

        var ausIds = items.Select(i => i.Id).ToList();
        var kantone = await (from los in db.TLos
                             join off in db.TOfferte on los.LosId equals off.OffLosId
                             join anb in db.TAnbieter on off.OffAnbId equals anb.AnbId
                             join ort in db.TOrtschaft on anb.AnbOrtId equals ort.OrtId
                             join kan in db.TKanton on ort.OrtKanId equals kan.KanId
                             where ausIds.Contains(los.LosAusId)
                             select new { los.LosAusId, kan.KanKurzbezeichnung })
                            .Distinct()
                            .ToListAsync();

        foreach (var item in items)
        {
            item.Kanton = string.Join(Trennzeichen, kantone
                .Where(k => k.LosAusId == item.Id)
                .Select(k => k.KanKurzbezeichnung)
                .OrderBy(k => k));
        }
    }

    /// <summary>
    /// Befüllt die Aggregat-Spalten des erweiterten Exports nachgelagert (statt per
    /// korrelierter Subquery pro Zeile): konkatenierte CPV-Nummern je Ausschreibung,
    /// konkatenierte BKP-Nummern je Los, die Sprachmatrix (Sprache × Dokumentart, «X» wenn
    /// vorhanden) sowie die Anzahl Offerten je Los nach Sprachregion des Anbieter-Kantons
    /// bzw. Ausland — Letztere pro Zeile desselben Loses redundant.
    /// </summary>
    private static async Task ErgaenzeAggregatSpaltenAsync(DoeAContext db, List<ErgebnisAlleDaten> items)
    {
        if (items.Count == 0)
        {
            return;
        }

        var ausIds = items.Select(i => i.AusId).Distinct().ToList();
        var losIds = items.Where(i => i.LosId != null).Select(i => i.LosId!.Value).Distinct().ToList();

        var sprachmatrix = await LadeSprachmatrixAsync(db, ausIds);
        var cpvJeAusschreibung = await LadeCpvNummernAsync(db, ausIds);
        var bkpJeLos = await LadeBkpNummernAsync(db, losIds);
        var offertenJeLos = await LadeOffertenHerkunftAsync(db, losIds);
        var berichtigungenJeAusschreibung = await LadeBerichtigungenAsync(db, ausIds);

        foreach (var item in items)
        {
            string? Zelle(string dokArt, string sprache) =>
                sprachmatrix.Contains((item.AusId, sprache, dokArt)) ? MatrixZelleGesetzt : null;

            item.CpvNummern = cpvJeAusschreibung.GetValueOrDefault(item.AusId);
            item.Berichtigungen = berichtigungenJeAusschreibung.GetValueOrDefault(item.AusId) ?? [];
            item.PublikationDe = Zelle(DokumentartPublikation, SpracheDe);
            item.PublikationFr = Zelle(DokumentartPublikation, SpracheFr);
            item.PublikationIt = Zelle(DokumentartPublikation, SpracheIt);
            item.PublikationEn = Zelle(DokumentartPublikation, SpracheEn);
            item.UnterlagenDe = Zelle(DokumentartUnterlagen, SpracheDe);
            item.UnterlagenFr = Zelle(DokumentartUnterlagen, SpracheFr);
            item.UnterlagenIt = Zelle(DokumentartUnterlagen, SpracheIt);
            item.UnterlagenEn = Zelle(DokumentartUnterlagen, SpracheEn);
            item.OffertenDe = Zelle(DokumentartOfferten, SpracheDe);
            item.OffertenFr = Zelle(DokumentartOfferten, SpracheFr);
            item.OffertenIt = Zelle(DokumentartOfferten, SpracheIt);
            item.OffertenEn = Zelle(DokumentartOfferten, SpracheEn);

            if (item.LosId is { } losId)
            {
                item.BkpNummern = bkpJeLos.GetValueOrDefault(losId);
                var anzahl = offertenJeLos.GetValueOrDefault(losId);
                item.AnzahlOffertenDeutsch = anzahl?.De ?? 0;
                item.AnzahlOffertenFranzoesisch = anzahl?.Fr ?? 0;
                item.AnzahlOffertenItalienisch = anzahl?.It ?? 0;
                item.AnzahlOffertenAusland = anzahl?.Ausland ?? 0;
            }
        }
    }

    /// <summary>
    /// Markiert je Offerte die Zeile ihres jüngsten Zustands (Regel von <see cref="OfferteHelper.LetzterZustand"/>),
    /// damit Summen auf Offertenebene über den Filter «Aktueller Offertzustand = Ja» auswertbar bleiben.
    /// </summary>
    private static void MarkiereAktuelleZustaende(List<ErgebnisAlleDaten> items)
    {
        foreach (var zeilenDerOfferte in items.Where(i => i.OffId != null).GroupBy(i => i.OffId))
        {
            var aktuell = OfferteHelper.JuengsterZustand(zeilenDerOfferte, z => z.OfferteZustandDatum, z => z.OozInsertedDate ?? 0,
                z => z.OozId ?? Guid.Empty);
            foreach (var zeile in zeilenDerOfferte)
            {
                zeile.OfferteZustandAktuell = zeile == aktuell;
            }
        }
    }

    /// <summary>Vorhandene Kombinationen aus Ausschreibung, Sprache und Dokumentart für die Sprachmatrix.</summary>
    private static async Task<HashSet<(Guid AusId, string Sprache, string? DokArt)>> LadeSprachmatrixAsync(DoeAContext db, List<Guid> ausIds)
    {
        return (await db.TAusschreibungSprache
                .Where(s => ausIds.Contains(s.AspAusId))
                .Select(s => new { s.AspAusId, Sprache = s.AspSpr.SprNaturalId, DokArt = s.AspDoa.DoaNaturalId })
                .ToListAsync())
            .Select(s => (s.AspAusId, s.Sprache, s.DokArt))
            .ToHashSet();
    }

    /// <summary>CPV-Nummern je Ausschreibung, aufsteigend sortiert und konkateniert.</summary>
    private static async Task<Dictionary<Guid, string>> LadeCpvNummernAsync(DoeAContext db, List<Guid> ausIds)
    {
        return (await db.TAusschreibungCpvNummer
                .Where(c => ausIds.Contains(c.AcnAusId))
                .Select(c => new { c.AcnAusId, Nummer = c.AcnCpv.CpvNummerStr })
                .ToListAsync())
            .GroupBy(c => c.AcnAusId)
            .ToDictionary(g => g.Key, g => string.Join(Trennzeichen, g.Select(x => x.Nummer).OrderBy(x => x)));
    }

    /// <summary>BKP-Nummern je Los, aufsteigend sortiert und konkateniert.</summary>
    private static async Task<Dictionary<Guid, string>> LadeBkpNummernAsync(DoeAContext db, List<Guid> losIds)
    {
        return (await db.TLosBkpNummer
                .Where(b => losIds.Contains(b.LbnLosId))
                .Select(b => new { b.LbnLosId, Nummer = b.LbnBkp.BkpNummerStr })
                .ToListAsync())
            .GroupBy(b => b.LbnLosId)
            .ToDictionary(g => g.Key, g => string.Join(Trennzeichen, g.Select(x => x.Nummer).OrderBy(x => x)));
    }

    /// <summary>Anzahl Offerten je Los (ohne reine Teilnahmeanträge), aufgeteilt nach Sprachregion des Anbieter-Kantons bzw. Ausland.</summary>
    private static async Task<Dictionary<Guid, OffertenHerkunft>> LadeOffertenHerkunftAsync(DoeAContext db, List<Guid> losIds)
    {
        var teilnahmeantraege = OfferteHelper.NurTeilnahmeantraege(db);
        var offerten = await (
            from off in db.TOfferte
            where losIds.Contains(off.OffLosId) && !teilnahmeantraege.Contains(off.OffId)
            join anb in db.TAnbieter on off.OffAnbId equals anb.AnbId into anbJ
            from anb in anbJ.DefaultIfEmpty()
            join ort in db.TOrtschaft on anb.AnbOrtId equals ort.OrtId into ortJ
            from ort in ortJ.DefaultIfEmpty()
            join kan in db.TKanton on ort.OrtKanId equals kan.KanId into kanJ
            from kan in kanJ.DefaultIfEmpty()
            join lan in db.TLand on ort.OrtLanId equals lan.LanId into lanJ
            from lan in lanJ.DefaultIfEmpty()
            select new
            {
                off.OffLosId,
                KantonSprache = kan != null ? kan.KanSpr.SprNaturalId : null,
                Ausland = lan != null && lan.LanNaturalId != LandSchweiz
            }).ToListAsync();

        return offerten
            .GroupBy(o => o.OffLosId)
            .ToDictionary(g => g.Key, g => new OffertenHerkunft(
                g.Count(x => x.KantonSprache == SpracheDe),
                g.Count(x => x.KantonSprache == SpracheFr),
                g.Count(x => x.KantonSprache == SpracheIt),
                g.Count(x => x.Ausland)));
    }

    /// <summary>Berichtigungen je Ausschreibung in Erfassungsreihenfolge.</summary>
    private static async Task<Dictionary<Guid, List<BerichtigungExport>>> LadeBerichtigungenAsync(DoeAContext db, List<Guid> ausIds)
    {
        return (await db.TBerichtigung
                .Where(b => ausIds.Contains(b.BerAusId))
                .OrderBy(b => b.BerInsertedDate)
                .Select(b => new { b.BerAusId, b.BerEingereicht, b.BerPubliziert, b.BerVerzoegerungDurchAuftraggeber, b.BerBemerkungen })
                .ToListAsync())
            .GroupBy(b => b.BerAusId)
            .ToDictionary(g => g.Key, g => g
                .Select(b => new BerichtigungExport(b.BerEingereicht, b.BerPubliziert, b.BerVerzoegerungDurchAuftraggeber, b.BerBemerkungen))
                .ToList());
    }

    /// <summary>Lädt die Ids der n:m-Filterlisten (Sachbearbeiter je Rolle) in das geladene Modell.</summary>
    private static async Task LadeFilterIdsAsync(DoeAContext db, Guid expId, Exportgruppe model)
    {
        model.AmtIds = await db.TExportgruppeAmt.Where(x => x.ExaExpId == expId).Select(x => x.ExaAmtId).ToListAsync();
        model.KantonIds = await db.TExportgruppeKanton.Where(x => x.EkaExpId == expId).Select(x => x.EkaKanId).ToListAsync();
        model.LeistungsartIds = await db.TExportgruppeLeistungsart.Where(x => x.ExlExpId == expId).Select(x => x.ExlLeaId).ToListAsync();
        model.ProjektartIds = await db.TExportgruppeProjektart.Where(x => x.EprExpId == expId).Select(x => x.EprPraId).ToListAsync();
        model.RessortIds = await db.TExportgruppeRessort.Where(x => x.EreExpId == expId).Select(x => x.EreResId).ToListAsync();
        model.RolleInAusschreibungIds = await db.TExportgruppeRolleInAusschreibung.Where(x => x.ExrExpId == expId).Select(x => x.ExrRiaId).ToListAsync();
        model.SachbearbeiterDoeAIds = await db.TExportgruppeSachbearbeiter.Where(x => x.ExsExpId == expId && x.ExsRolle == RolleDoeA).Select(x => x.ExsSacId).ToListAsync();
        model.SachbearbeiterRdIds = await db.TExportgruppeSachbearbeiter.Where(x => x.ExsExpId == expId && x.ExsRolle == RolleRd).Select(x => x.ExsSacId).ToListAsync();
        model.SachbearbeiterBblIds = await db.TExportgruppeSachbearbeiter.Where(x => x.ExsExpId == expId && x.ExsRolle == RolleBbl).Select(x => x.ExsSacId).ToListAsync();
        model.VerfahrensartIds = await db.TExportgruppeVerfahrensart.Where(x => x.EvaExpId == expId).Select(x => x.EvaVeaId).ToListAsync();
    }

    /// <summary>Lädt den Bestand aller Verknüpfungstabellen einer Exportgruppe.</summary>
    private static async Task<Verknuepfungen> LadeVerknuepfungenAsync(DoeAContext db, Guid expId)
    {
        return new Verknuepfungen(
            await db.TExportgruppeAmt.Where(x => x.ExaExpId == expId).ToListAsync(),
            await db.TExportgruppeKanton.Where(x => x.EkaExpId == expId).ToListAsync(),
            await db.TExportgruppeLeistungsart.Where(x => x.ExlExpId == expId).ToListAsync(),
            await db.TExportgruppeProjektart.Where(x => x.EprExpId == expId).ToListAsync(),
            await db.TExportgruppeRessort.Where(x => x.EreExpId == expId).ToListAsync(),
            await db.TExportgruppeRolleInAusschreibung.Where(x => x.ExrExpId == expId).ToListAsync(),
            await db.TExportgruppeSachbearbeiter.Where(x => x.ExsExpId == expId && x.ExsRolle == RolleDoeA).ToListAsync(),
            await db.TExportgruppeSachbearbeiter.Where(x => x.ExsExpId == expId && x.ExsRolle == RolleRd).ToListAsync(),
            await db.TExportgruppeSachbearbeiter.Where(x => x.ExsExpId == expId && x.ExsRolle == RolleBbl).ToListAsync(),
            await db.TExportgruppeVerfahrensart.Where(x => x.EvaExpId == expId).ToListAsync());
    }

    /// <summary>Ob sich mindestens eine der Filterlisten gegenüber dem Bestand geändert hat.</summary>
    private static bool HatListenaenderungen(Verknuepfungen bestand, Exportgruppe model)
    {
        return HatSichGeaendert(bestand.Amt, model.AmtIds, x => x.ExaAmtId) ||
               HatSichGeaendert(bestand.Kanton, model.KantonIds, x => x.EkaKanId) ||
               HatSichGeaendert(bestand.Leistungsart, model.LeistungsartIds, x => x.ExlLeaId) ||
               HatSichGeaendert(bestand.Projektart, model.ProjektartIds, x => x.EprPraId) ||
               HatSichGeaendert(bestand.Ressort, model.RessortIds, x => x.EreResId) ||
               HatSichGeaendert(bestand.RolleInAusschreibung, model.RolleInAusschreibungIds, x => x.ExrRiaId) ||
               HatSichGeaendert(bestand.SachbearbeiterDoeA, model.SachbearbeiterDoeAIds, x => x.ExsSacId) ||
               HatSichGeaendert(bestand.SachbearbeiterRd, model.SachbearbeiterRdIds, x => x.ExsSacId) ||
               HatSichGeaendert(bestand.SachbearbeiterBbl, model.SachbearbeiterBblIds, x => x.ExsSacId) ||
               HatSichGeaendert(bestand.Verfahrensart, model.VerfahrensartIds, x => x.EvaVeaId);
    }

    /// <summary>
    /// Gleicht alle Verknüpfungstabellen gegen die gewählten Filter-Ids ab; neue Zeilen
    /// übernehmen die Audit-Werte dieses Speichervorgangs.
    /// </summary>
    private static void SyncVerknuepfungen(DoeAContext db, Verknuepfungen bestand, Exportgruppe model, TExportgruppe tExp)
    {
        var expId = tExp.ExpId;
        var date = tExp.ExpChangedDate;
        var user = tExp.ExpChangedUser;

        SyncJunction(db, bestand.Amt, model.AmtIds, x => x.ExaAmtId,
            id => new TExportgruppeAmt { ExaId = Guid.NewGuid(), ExaExpId = expId, ExaAmtId = id, ExaAktiv = true, ExaInsertedDate = date, ExaInsertedUser = user, ExaChangedDate = date, ExaChangedUser = user });
        SyncJunction(db, bestand.Kanton, model.KantonIds, x => x.EkaKanId,
            id => new TExportgruppeKanton { EkaId = Guid.NewGuid(), EkaExpId = expId, EkaKanId = id, EkaAktiv = true, EkaInsertedDate = date, EkaInsertedUser = user, EkaChangedDate = date, EkaChangedUser = user });
        SyncJunction(db, bestand.Leistungsart, model.LeistungsartIds, x => x.ExlLeaId,
            id => new TExportgruppeLeistungsart { ExlId = Guid.NewGuid(), ExlExpId = expId, ExlLeaId = id, ExlAktiv = true, ExlInsertedDate = date, ExlInsertedUser = user, ExlChangedDate = date, ExlChangedUser = user });
        SyncJunction(db, bestand.Projektart, model.ProjektartIds, x => x.EprPraId,
            id => new TExportgruppeProjektart { EprId = Guid.NewGuid(), EprExpId = expId, EprPraId = id, EprAktiv = true, EprInsertedDate = date, EprInsertedUser = user, EprChangedDate = date, EprChangedUser = user });
        SyncJunction(db, bestand.Ressort, model.RessortIds, x => x.EreResId,
            id => new TExportgruppeRessort { EreId = Guid.NewGuid(), EreExpId = expId, EreResId = id, EreAktiv = true, EreInsertedDate = date, EreInsertedUser = user, EreChangedDate = date, EreChangedUser = user });
        SyncJunction(db, bestand.RolleInAusschreibung, model.RolleInAusschreibungIds, x => x.ExrRiaId,
            id => new TExportgruppeRolleInAusschreibung { ExrId = Guid.NewGuid(), ExrExpId = expId, ExrRiaId = id, ExrAktiv = true, ExrInsertedDate = date, ExrInsertedUser = user, ExrChangedDate = date, ExrChangedUser = user });
        SyncJunction(db, bestand.SachbearbeiterDoeA, model.SachbearbeiterDoeAIds, x => x.ExsSacId, id => NeuerSachbearbeiter(id, RolleDoeA));
        SyncJunction(db, bestand.SachbearbeiterRd, model.SachbearbeiterRdIds, x => x.ExsSacId, id => NeuerSachbearbeiter(id, RolleRd));
        SyncJunction(db, bestand.SachbearbeiterBbl, model.SachbearbeiterBblIds, x => x.ExsSacId, id => NeuerSachbearbeiter(id, RolleBbl));
        SyncJunction(db, bestand.Verfahrensart, model.VerfahrensartIds, x => x.EvaVeaId,
            id => new TExportgruppeVerfahrensart { EvaId = Guid.NewGuid(), EvaExpId = expId, EvaVeaId = id, EvaAktiv = true, EvaInsertedDate = date, EvaInsertedUser = user, EvaChangedDate = date, EvaChangedUser = user });

        TExportgruppeSachbearbeiter NeuerSachbearbeiter(Guid id, string rolle) =>
            new() { ExsId = Guid.NewGuid(), ExsExpId = expId, ExsSacId = id, ExsRolle = rolle, ExsAktiv = true, ExsInsertedDate = date, ExsInsertedUser = user, ExsChangedDate = date, ExsChangedUser = user };
    }

    /// <summary>
    /// Diff-Sync einer n:m-Verknüpfungstabelle gegen die gewählten Filter-Ids: überzählige
    /// Zeilen entfernen, fehlende neu anlegen (unveränderte bleiben unangetastet).
    /// </summary>
    private static void SyncJunction<TJunction>(DoeAContext db, List<TJunction> bestehend, List<Guid> ausgewaehlteIds,
        Func<TJunction, Guid> filterIdVon, Func<Guid, TJunction> erstelleNeu) where TJunction : class
    {
        foreach (var zeile in bestehend.Where(z => !ausgewaehlteIds.Contains(filterIdVon(z))))
        {
            db.Remove(zeile);
        }

        var vorhandeneIds = bestehend.Select(filterIdVon).ToHashSet();
        foreach (var filterId in ausgewaehlteIds.Where(id => !vorhandeneIds.Contains(id)))
        {
            db.Add(erstelleNeu(filterId));
        }
    }

    private static bool HatSichGeaendert<TJunction>(List<TJunction> bestehend, List<Guid> ausgewaehlteIds, Func<TJunction, Guid> filterIdVon)
    {
        return !bestehend.Select(filterIdVon).ToHashSet().SetEquals(ausgewaehlteIds);
    }
}
