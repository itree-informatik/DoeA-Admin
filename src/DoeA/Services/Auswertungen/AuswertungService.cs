// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using ClosedXML.Excel;
using DoeA.Data.Constants;
using DoeA.Data.Enums;
using DoeA.Data.Helper;
using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO.Auswertungen;
using DoeA.Data.Models.DTO.Stammdaten;
using DoeA.Interfaces;
using DoeA.Interfaces.Auswertungen;
using GemBox.Document;
using ItreeMud.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using ILogger = Serilog.ILogger;

namespace DoeA.Services.Auswertungen;

/// <summary>Erstellt die Auswertungen als PDF (GemBox-Vorlagen) oder als Excel-Datei (ClosedXML).</summary>
public class AuswertungService : IAuswertungService
{
    private const string AlleAnzeige = "Alle";

    // Anbieter ohne Kanton (Sitz im Ausland) bilden im Zuschläge-Bericht eine eigene Zeile.
    private const string Ausland = "Ausland";

    // Kopfzellen sind über mehrere Spalten verbunden: rund 58 Zeichen füllen eine Zeile von
    // 20 Punkt Höhe — daraus wird die Zeilenhöhe für umbrochenen Text geschätzt.
    private const int ExcelZeichenProZeile = 58;
    private const int ExcelZeilenhoehe = 20;

    // Ganzzahl mit Tausendertrennung; das öffnende Excel stellt sie in seiner Region dar
    // (Schweizer Excel: 1'000).
    private const string ExcelZahlenformat = "#,##0";

    // NaturalIds der Offertezustände.
    private const string OffertezustandErfasst = "erfasst";
    private const string OffertezustandWiderrufen = "widerrufen";
    private const string OffertezustandZuschlag = "zuschlag";
    private const string OffertezustandZuschlagPraequali = "zuschlagpraequali";

    /// <summary>
    /// Letzte Zustände einer Offerte, mit denen das Verfahren noch läuft (Laufende WTO): erfasst, beim selektiven Verfahren
    /// zudem die Teilnahmeanträge der Phase 1 (ein abgelehnter beendet nur seine Offerte) und der Zuschlag Präqualifikation.
    /// </summary>
    private static readonly string[] OffertezustaendeLaufend =
        [OffertezustandErfasst, "teilnahmeantragerfasst", "teilnahmeantragabgelehnt", OffertezustandZuschlagPraequali];

    private const string ProjektartBau = "Bau";
    private const string ProjektartLogistik = "Logistik";

    // NaturalIds (stabil, für Filter) und Bezeichnungen (für die Anzeige in den Berichten,
    // identisch mit den UI-Labels) der Rollen in Ausschreibung.
    private const string RolleBezeichnungAemterDezentral = "Ämter dezentral";
    private const string RolleBezeichnungLogistikZentral = "Logistik zentral";
    private const string RolleInAusschreibungAemterDezentral = "AemterDezentral";
    private const string RolleInAusschreibungLogistikZentral = "LogistikZentral";

    private const string VerfahrensartEinladung = "Einladung";
    private const string VerfahrensartFreihändig = "Freihändig";
    private const string VerfahrensartOffen = "Offen";
    private const string VerfahrensartRFI = "RFI";
    private const string VerfahrensartSelektiv = "Selektiv";
    private const string VerfahrensartVorankündigungOffen = "VorankündigungOffen";
    private const string VerfahrensartVorankündigungSelektiv = "VorankündigungSelektiv";
    private const string VerfahrensartVorankündigungUnbekannt = "VorankündigungUnbekannt";

    // Die drei Vorankündigungs-Varianten erscheinen in den Berichtsköpfen als ein Kriterium.
    private const string VorankuendigungAnzeige = "Vorankündigung";

    // Position der Zielbeurteilung in einem Abschnitt der Zielerreichung (letzte Zeile, Spalte "Ziel erfüllt?").
    private const int ZielBeurteilungSpalte = 4;
    private const int ZielBeurteilungZeile = 3;

    // Spaltengruppen des Beschaffungscontrolling-Dashboards: Bau, Logistik-zentral,
    // Ämter-dezentral (gleiche Zuordnung wie die Seitenfilter der Zielerreichung).
    private static readonly Func<ControllingEreignis, bool>[] ControllingGruppen =
    [
        e => e.Projektart == ProjektartBau,
        e => e.Projektart == ProjektartLogistik && e.Rolle == RolleInAusschreibungLogistikZentral,
        e => e.Projektart == ProjektartLogistik && e.Rolle == RolleInAusschreibungAemterDezentral
    ];

    // Grau der Spaltentitel: Weiss 15% dunkler (Excel-Standardfarbe).
    private static readonly XLColor ExcelTitelHintergrund = XLColor.FromHtml("#D9D9D9");

    private static readonly string[] Projektarten = [ProjektartBau, ProjektartLogistik];

    // Sprachen in der Reihenfolge der Berichtszeilen mit ihrer Anzeigebezeichnung.
    private static readonly (string NaturalId, string Bezeichnung)[] Sprachen =
    [
        ("DE", "Deutsch"),
        ("FR", "Französisch"),
        ("IT", "Italienisch"),
        ("EN", "Englisch")
    ];

    // Sprachregion des Anbieterkantons (SprNaturalId) mit ihrer Bezeichnung im Bericht.
    private static readonly Dictionary<string, string> Sprachregionen = new()
    {
        { "DE", "Deutschschweiz" },
        { "FR", "Romandie" },
        { "IT", "Tessin" },
        { Ausland, Ausland }
    };

    // Ausschreibungen und Berichtigungen zählen in Beschaffungscontrolling und Zielerreichung nur bei diesen
    // Verfahrensarten (die Zielerreichung zeigt die Einladungen als eigenen Abschnitt), Abbrüche in allen Auswertungen.
    private static readonly string[] VerfahrensartenAusschreibung = [VerfahrensartOffen, VerfahrensartSelektiv, VerfahrensartEinladung];

    // Alle Vorankündigungs-Varianten (NaturalIds).
    private static readonly string[] VerfahrensartenVorankündigung =
        [VerfahrensartVorankündigungOffen, VerfahrensartVorankündigungSelektiv, VerfahrensartVorankündigungUnbekannt];

    // Wie VerfahrensartenAusschreibung, dazu Freihändig: Zuschläge in Beschaffungscontrolling und Zielerreichung,
    // Widerrufe in allen Auswertungen.
    private static readonly string[] VerfahrensartenZuschlagWiderruf =
        [VerfahrensartOffen, VerfahrensartSelektiv, VerfahrensartEinladung, VerfahrensartFreihändig];

    // RFI und Vorankündigungen haben keine Lose, ihr Widerruf steht in der Ausschreibung. Er zählt nur in
    // Beschaffungscontrolling und Zielerreichung, nicht im 5-Jahresvergleich der Abbrüche und Widerrufe.
    private static readonly string[] VerfahrensartenWiderrufOhneLose = [VerfahrensartRFI, .. VerfahrensartenVorankündigung];

    // Zellhintergründe für die Zielbeurteilung (dasselbe Grün/Rot wie in der Vorlage Zielerreichung.dotx).
    private static readonly Color ZielErfuelltFarbe = new(180, 229, 162);
    private static readonly Color ZielNichtErfuelltFarbe = new(255, 153, 153);

    private readonly IAusschreibungVerfahrensschrittService _ausschreibungVerfahrensschrittService;
    private readonly IDbContextFactory<DoeAContext> _dbFactory;
    private readonly IDownloadStorageService _storageService;
    private readonly IGemBoxDocumentService _gemBoxDocumentService;
    private readonly ILogger _logger;

    // Der GemBox-Service hält pro Scope genau ein Dokument: parallele Berichtserzeugungen
    // derselben Session (z.B. Doppelklick oder zweites Tile während der Erstellung) würden
    // sich das Dokument gegenseitig ersetzen. Die GemBox-basierten Berichte werden deshalb
    // pro Service-Instanz serialisiert; andere Sessions sind nicht betroffen (Scoped).
    private readonly SemaphoreSlim _dokumentLock = new(1, 1);

    public AuswertungService(IDbContextFactory<DoeAContext> dbFactory, IGemBoxDocumentService gemBoxDocumentService, IDownloadStorageService storageService, ILogger logger,
        IAusschreibungVerfahrensschrittService ausschreibungVerfahrensschrittService)
    {
        _ausschreibungVerfahrensschrittService = ausschreibungVerfahrensschrittService;
        _dbFactory = dbFactory;
        _gemBoxDocumentService = gemBoxDocumentService;
        _storageService = storageService;
        _logger = logger;
    }

    /// <summary>
    /// Erstellt die Auswertung der Abbrüche (<see cref="AbbruchPublikationen"/>) und Widerrufe
    /// (<see cref="WiderrufPublikationen"/>) über die fünf Jahre ab <paramref name="jahrVon"/>.
    /// </summary>
    public Task<string> GetAbbruchWiderrufAsync(int jahrVon, Departement? dep = null, List<Amt>? aemter = null, bool excel = false)
        => MitDokumentLockAsync(() => GetAbbruchWiderrufCoreAsync(jahrVon, dep, aemter, excel));

    /// <summary>Erstellt die Auswertung der effektiven Zuschläge (<see cref="EffektiveZuschlaege"/>) nach Kanton oder Sprachregion.</summary>
    public Task<string> GetZuschlaegeAsync(ZuschlaegeParameter parameter)
        => MitDokumentLockAsync(() => GetZuschlaegeCoreAsync(parameter));

    /// <summary>
    /// Erstellt die Auswertung der Ausschreibungen, entweder nach Jahr/Quartal
    /// (<see cref="AusschreibungenParameter.NachJahrQuartalen"/> = true) oder nach Sprache/Dokumentart.
    /// </summary>
    public Task<string> GetAusschreibungenAsync(AusschreibungenParameter parameter)
        => MitDokumentLockAsync(() => GetAusschreibungenCoreAsync(parameter));

    /// <summary>
    /// Erstellt eine Auswertung der effektiven Zuschläge (<see cref="EffektiveZuschlaege"/>) freihändiger
    /// Vergaben der gewählten Projektarten und BBL-Rollen als Excel-Datei, eine Zeile pro Offerte, sortiert
    /// nach massgebendem Datum und Nummer. Freihänder ohne Offerte oder mit widerrufenem Zuschlag erscheinen nicht.
    /// </summary>
    public async Task<string> GetFreihaenderAsync(DateOnly datumVon, DateOnly datumBis, bool freihaendig,
        bool logistik, bool bau, bool aemterDezentral, bool logistikZentral)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        var verfahrensarten = VerfahrensartenListe(offen: false, selektiv: false, freihaendig: freihaendig);
        var projektarten = ProjektartenListe(logistik, bau);
        var (rollen, rollenAnzeige) = RollenListe(aemterDezentral, logistikZentral);

        var daten = await (
            from z in EffektiveZuschlaege(context, datumVon, datumBis)
            let aus = z.Ausschreibung
            where verfahrensarten.Contains(aus.AusVea.VeaNaturalId!) &&
                  projektarten.Contains(aus.AusPra.PraNaturalId!) &&
                  rollen.Contains(aus.AusRia!.RiaNaturalId!)
            orderby z.MassgebendesDatum, aus.AusNummerStr
            select new
            {
                aus.AusJahr,
                aus.AusNummerStr,
                Projektart = aus.AusPra.PraBezeichnung,
                aus.AusBezeichnung,
                RolleBbl = aus.AusRia!.RiaBezeichnung,
                Leistungsart = aus.AusLea!.LeaBezeichnung,
                Ressort = aus.AusRes!.ResKurzbezeichnung,
                Bedarfsstelle = aus.AusAmt!.AmtBezeichnung,
                Bbl = aus.AusSacBbl != null ? aus.AusSacBbl.SacVorname + " " + aus.AusSacBbl.SacNachname : null,
                Zuschlagsempfaenger = z.Offerte.OffAnb.AnbName,
                Zuschlagssumme = z.Offerte.OffZuschlagswertChf,
                aus.AusPublikation
            }).ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Freihänder");
        ExcelTitelzeile(worksheet, 1, $"Publikationsdatum: {datumVon:dd.MM.yyyy} - {datumBis:dd.MM.yyyy}");
        ExcelTitelzeile(worksheet, 2, "Zuschläge Freihänder");
        // Die Verfahrensart ist hier immer «Freihändig», eine Filterzeile dafür sagte nichts aus.
        ExcelTitelzeile(worksheet, 3, $"Projektart(en): {string.Join(", ", projektarten)}");
        ExcelTitelzeile(worksheet, 4, $"Rolle(n) BBL: {string.Join(", ", rollenAnzeige)}");

        const int headerRow = 6;
        string[] spalten =
        [
            "Jahr", "Nummer", "Projektart", "Projektbezeichnung", "Rolle BBL", "Leistungsart",
            "Ressort", "Bedarfsstelle", "BBL", "Zuschlagsempfänger", "Zuschlagssumme",
            "Publikationsdatum des Freihänders"
        ];
        ExcelSpaltentitel(worksheet, headerRow, spalten);

        var row = headerRow + 1;
        foreach (var d in daten)
        {
            worksheet.Cell(row, 1).Value = d.AusJahr;
            worksheet.Cell(row, 2).Value = d.AusNummerStr;
            worksheet.Cell(row, 3).Value = d.Projektart;
            worksheet.Cell(row, 4).Value = d.AusBezeichnung;
            worksheet.Cell(row, 5).Value = d.RolleBbl;
            worksheet.Cell(row, 6).Value = d.Leistungsart;
            worksheet.Cell(row, 7).Value = d.Ressort;
            worksheet.Cell(row, 8).Value = d.Bedarfsstelle;
            worksheet.Cell(row, 9).Value = d.Bbl;
            worksheet.Cell(row, 10).Value = d.Zuschlagsempfaenger;
            if (d.Zuschlagssumme != null)
            {
                worksheet.Cell(row, 11).Value = d.Zuschlagssumme.Value;
            }
            if (d.AusPublikation != null)
            {
                worksheet.Cell(row, 12).Value = d.AusPublikation.Value.ToDateTime(TimeOnly.MinValue);
            }
            row++;
        }

        worksheet.Range(headerRow + 1, 11, row - 1, 11).Style.NumberFormat.Format = "#,##0.00";
        worksheet.Range(headerRow + 1, 12, row - 1, 12).Style.DateFormat.Format = "dd.MM.yyyy";
        worksheet.Range(headerRow, 1, row - 1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        worksheet.Range(headerRow, 11, row - 1, 11).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        TitelUmbrechenUndSpaltenbreitenAnpassen(worksheet, headerRow, spalten, row - 1);

        return await ExcelSpeichernAsync(workbook, $"Freihänder_{datumVon:yyyyMMdd}_{datumBis:yyyyMMdd}.xlsx");
    }

    /// <summary>
    /// Erstellt eine Auswertung der effektiven Zuschläge (<see cref="EffektiveZuschlaege"/>) der gewählten
    /// Verfahrensarten (offen, selektiv, Einladung), Projektarten und BBL-Rollen als Excel-Datei ("Zuschläge
    /// ohne Freihänder"), eine Zeile pro Offerte, sortiert nach massgebendem Datum und Nummer. Die Spalte
    /// Publikationsdatum ist auch beim selektiven Verfahren die Publikation der Ausschreibung.
    /// Die beiden Wochen-Spalten werden als Excel-Formeln eingetragen (ungerundet, Anzeige ohne
    /// Dezimalstellen).
    /// </summary>
    public async Task<string> GetZuschlaegeOhneFreihaenderAsync(DateOnly datumVon, DateOnly datumBis, bool offen, bool selektiv, bool einladung,
        bool logistik, bool bau, bool aemterDezentral, bool logistikZentral)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        var verfahrensarten = VerfahrensartenListe(offen, selektiv, einladung: einladung);
        var projektarten = ProjektartenListe(logistik, bau);
        var (rollen, rollenAnzeige) = RollenListe(aemterDezentral, logistikZentral);

        var daten = await (
            from z in EffektiveZuschlaege(context, datumVon, datumBis)
            let aus = z.Ausschreibung
            where verfahrensarten.Contains(aus.AusVea.VeaNaturalId!) &&
                  projektarten.Contains(aus.AusPra.PraNaturalId!) &&
                  rollen.Contains(aus.AusRia!.RiaNaturalId!)
            orderby z.MassgebendesDatum, aus.AusNummerStr
            select new
            {
                aus.AusJahr,
                aus.AusNummerStr,
                Projektart = aus.AusPra.PraBezeichnung,
                aus.AusBezeichnung,
                RolleBbl = aus.AusRia!.RiaBezeichnung,
                Verfahrensart = aus.AusVea.VeaBezeichnung,
                Leistungsart = aus.AusLea!.LeaBezeichnung,
                Ressort = aus.AusRes!.ResKurzbezeichnung,
                Bedarfsstelle = aus.AusAmt!.AmtBezeichnung,
                Bbl = aus.AusSacBbl != null ? aus.AusSacBbl.SacVorname + " " + aus.AusSacBbl.SacNachname : null,
                Zuschlagsempfaenger = z.Offerte.OffAnb.AnbName,
                Zuschlagssumme = z.Offerte.OffZuschlagswertChf,
                Zuschlagsdatum = z.Zustand.OozDatum,
                Publikation = aus.AusPublikation,
                Eingabefrist = aus.AusOffertenFrist
            }).ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Zuschläge ohne Freihänder");
        ExcelTitelzeile(worksheet, 1, $"Publikationsdatum des Zuschlags: {datumVon:dd.MM.yyyy} - {datumBis:dd.MM.yyyy}");
        ExcelTitelzeile(worksheet, 2, "Zuschläge ohne Freihänder");
        ExcelTitelzeile(worksheet, 3, $"Verfahrensart(en): {string.Join(", ", verfahrensarten)}");
        ExcelTitelzeile(worksheet, 4, $"Projektart(en): {string.Join(", ", projektarten)}");
        ExcelTitelzeile(worksheet, 5, $"Rolle(n) BBL: {string.Join(", ", rollenAnzeige)}");

        const int headerRow = 7;
        string[] spalten =
        [
            "Jahr", "Nummer", "Projektart", "Projektbezeichnung", "Rolle BBL", "Verfahrensart",
            "Leistungsart", "Ressort", "Bedarfsstelle", "BBL", "Zuschlagsempfänger", "Zuschlagssumme",
            "Zuschlagsdatum", "Publikationsdatum", "Eingabefrist Offerten",
            "Eingabefrist Offerten bis Zuschlag in Wochen", "Publikation Ausschreibung bis Zuschlag in Wochen"
        ];
        ExcelSpaltentitel(worksheet, headerRow, spalten);

        var row = headerRow + 1;
        foreach (var d in daten)
        {
            worksheet.Cell(row, 1).Value = d.AusJahr;
            worksheet.Cell(row, 2).Value = d.AusNummerStr;
            worksheet.Cell(row, 3).Value = d.Projektart;
            worksheet.Cell(row, 4).Value = d.AusBezeichnung;
            worksheet.Cell(row, 5).Value = d.RolleBbl;
            worksheet.Cell(row, 6).Value = d.Verfahrensart;
            worksheet.Cell(row, 7).Value = d.Leistungsart;
            worksheet.Cell(row, 8).Value = d.Ressort;
            worksheet.Cell(row, 9).Value = d.Bedarfsstelle;
            worksheet.Cell(row, 10).Value = d.Bbl;
            worksheet.Cell(row, 11).Value = d.Zuschlagsempfaenger;
            if (d.Zuschlagssumme != null)
            {
                worksheet.Cell(row, 12).Value = d.Zuschlagssumme.Value;
            }
            worksheet.Cell(row, 13).Value = d.Zuschlagsdatum.ToDateTime(TimeOnly.MinValue);
            if (d.Publikation != null)
            {
                worksheet.Cell(row, 14).Value = d.Publikation.Value.ToDateTime(TimeOnly.MinValue);
            }
            if (d.Eingabefrist != null)
            {
                worksheet.Cell(row, 15).Value = d.Eingabefrist.Value.ToDateTime(TimeOnly.MinValue);
                worksheet.Cell(row, 16).FormulaA1 = $"(M{row}-O{row})/7";
            }
            if (d.Publikation != null)
            {
                worksheet.Cell(row, 17).FormulaA1 = $"(M{row}-N{row})/7";
            }
            row++;
        }

        worksheet.Range(headerRow + 1, 12, row - 1, 12).Style.NumberFormat.Format = "#,##0.00";
        worksheet.Range(headerRow + 1, 13, row - 1, 15).Style.DateFormat.Format = "dd.MM.yyyy";
        worksheet.Range(headerRow + 1, 16, row - 1, 17).Style.NumberFormat.Format = "0";
        worksheet.Range(headerRow, 1, row - 1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        worksheet.Range(headerRow, 12, row - 1, 12).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        TitelUmbrechenUndSpaltenbreitenAnpassen(worksheet, headerRow, spalten, row - 1);

        return await ExcelSpeichernAsync(workbook, $"Zuschläge_ohne_Freihänder_{datumVon:yyyyMMdd}_{datumBis:yyyyMMdd}.xlsx");
    }

    /// <summary>
    /// Excel «Laufende WTO»: eine Zeile pro Ausschreibung der gewählten Verfahrens-/Projektarten und BBL-Rollen mit Publikation
    /// im Zeitraum (auch beim selektiven Verfahren das Publikationsdatum der Grunddaten), die offene Lose ohne Offerten über «erfasst» hinaus hat.
    /// Das Zuschlagsdatum bleibt leer und wird händisch nachgeführt; die Wochen-Spalten rechnen als Excel-Formeln erst dann.
    /// </summary>
    public async Task<string> GetLaufendeWtoAsync(DateOnly datumVon, DateOnly datumBis, bool offen, bool selektiv, bool einladung,
        bool logistik, bool bau, bool aemterDezentral, bool logistikZentral)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        var verfahrensarten = VerfahrensartenListe(offen, selektiv, einladung: einladung);
        var projektarten = ProjektartenListe(logistik, bau);
        var (rollen, rollenAnzeige) = RollenListe(aemterDezentral, logistikZentral);

        var daten = await (
            from aus in context.TAusschreibung
            join vea in context.TVerfahrensart on aus.AusVeaId equals vea.VeaId
            join pra in context.TProjektart on aus.AusPraId equals pra.PraId
            join ria in context.TRolleInAusschreibung on aus.AusRiaId equals ria.RiaId
            join lea in context.TLeistungsart on aus.AusLeaId equals lea.LeaId
            join amt in context.TAmt on aus.AusAmtId equals amt.AmtId
            join res in context.TRessort on aus.AusResId equals res.ResId into resJoin
            from res in resJoin.DefaultIfEmpty()
            join sac in context.TSachbearbeiter on aus.AusSacBblId equals sac.SacId into sacJoin
            from sac in sacJoin.DefaultIfEmpty()
            where verfahrensarten.Contains(vea.VeaNaturalId!) &&
                  projektarten.Contains(pra.PraNaturalId!) &&
                  rollen.Contains(ria.RiaNaturalId!) &&
                  aus.AusPublikation >= datumVon &&
                  aus.AusPublikation <= datumBis &&
                  context.TLos.Any(los => los.LosAusId == aus.AusId && los.LosZustand == nameof(EnumLosZustand.Offen)) &&
                  // Laufend = keine Offerte ist über "erfasst" hinaus: der letzte Zustand jeder Offerte ist einer der
                  // OffertezustaendeLaufend (Offerten ohne Zustand gelten als laufend).
                  // Nur Lose im Zustand "Offen" zählen; abgebrochene, widerrufene und
                  // abgeschlossene Lose bleiben samt ihren Offerten unberücksichtigt.
                  context.TOfferte
                      .Where(off => off.OffLos.LosAusId == aus.AusId && off.OffLos.LosZustand == nameof(EnumLosZustand.Offen))
                      .All(off => context.TOfferteOffertezustand
                          .Where(ooz => ooz.OozOffId == off.OffId &&
                                        ooz.OozDatum == context.TOfferteOffertezustand
                                            .Where(o => o.OozOffId == off.OffId)
                                            .Max(o => o.OozDatum))
                          .All(ooz => OffertezustaendeLaufend.Contains(ooz.OozOfz.OfzNaturalId!)))
            orderby aus.AusJahr, aus.AusNummerStr
            select new
            {
                aus.AusJahr,
                aus.AusNummerStr,
                Projektart = pra.PraBezeichnung,
                aus.AusBezeichnung,
                RolleBbl = ria.RiaBezeichnung,
                Verfahrensart = vea.VeaBezeichnung,
                Leistungsart = lea.LeaBezeichnung,
                Ressort = res != null ? res.ResKurzbezeichnung : null,
                Bedarfsstelle = amt.AmtBezeichnung,
                Bbl = sac != null ? sac.SacVorname + " " + sac.SacNachname : null,
                Publikation = aus.AusPublikation,
                Eingabefrist = aus.AusOffertenFrist
            }).ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Laufende WTO");
        ExcelTitelzeile(worksheet, 1, $"Publikationsdatum: {datumVon:dd.MM.yyyy} - {datumBis:dd.MM.yyyy}");
        ExcelTitelzeile(worksheet, 2, $"Verfahrensart(en): {string.Join(", ", verfahrensarten)}");
        ExcelTitelzeile(worksheet, 3, $"Projektart(en): {string.Join(", ", projektarten)}");
        ExcelTitelzeile(worksheet, 4, $"Rolle(n) BBL: {string.Join(", ", rollenAnzeige)}");
        ExcelTitelzeile(worksheet, 5, $"Erstellt: {DateTime.Now:dd.MM.yyyy HH:mm}");

        const int headerRow = 7;
        string[] spalten =
        [
            "Jahr", "Nummer", "Projektart", "Projektbezeichnung", "Rolle BBL", "Verfahrensart",
            "Leistungsart", "Ressort", "Bedarfsstelle", "BBL",
            "Zuschlagsdatum", "Publikationsdatum", "Eingabefrist Offerten",
            "Eingabefrist Offerten bis Zuschlag in Wochen", "Publikation Ausschreibung bis Zuschlag in Wochen"
        ];
        ExcelSpaltentitel(worksheet, headerRow, spalten);

        var row = headerRow + 1;
        foreach (var d in daten)
        {
            worksheet.Cell(row, 1).Value = d.AusJahr;
            worksheet.Cell(row, 2).Value = d.AusNummerStr;
            worksheet.Cell(row, 3).Value = d.Projektart;
            worksheet.Cell(row, 4).Value = d.AusBezeichnung;
            worksheet.Cell(row, 5).Value = d.RolleBbl;
            worksheet.Cell(row, 6).Value = d.Verfahrensart;
            worksheet.Cell(row, 7).Value = d.Leistungsart;
            worksheet.Cell(row, 8).Value = d.Ressort;
            worksheet.Cell(row, 9).Value = d.Bedarfsstelle;
            worksheet.Cell(row, 10).Value = d.Bbl;
            // Spalte 11 (Zuschlagsdatum, K) bleibt leer und wird händisch nachgeführt.
            if (d.Publikation != null)
            {
                worksheet.Cell(row, 12).Value = d.Publikation.Value.ToDateTime(TimeOnly.MinValue);
            }
            // Gleiche Berechnung wie in "Zuschläge ohne Freihänder"; solange das
            // Zuschlagsdatum noch nicht nachgeführt ist, bleiben die Zellen leer.
            if (d.Eingabefrist != null)
            {
                worksheet.Cell(row, 13).Value = d.Eingabefrist.Value.ToDateTime(TimeOnly.MinValue);
                worksheet.Cell(row, 14).FormulaA1 = $"IF(K{row}=\"\",\"\",(K{row}-M{row})/7)";
            }
            if (d.Publikation != null)
            {
                worksheet.Cell(row, 15).FormulaA1 = $"IF(K{row}=\"\",\"\",(K{row}-L{row})/7)";
            }
            row++;
        }

        worksheet.Range(headerRow + 1, 11, row - 1, 13).Style.DateFormat.Format = "dd.MM.yyyy";
        worksheet.Range(headerRow + 1, 14, row - 1, 15).Style.NumberFormat.Format = "0";
        worksheet.Range(headerRow, 1, row - 1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        TitelUmbrechenUndSpaltenbreitenAnpassen(worksheet, headerRow, spalten, row - 1);

        return await ExcelSpeichernAsync(workbook, $"Laufende_WTO_{datumVon:yyyyMMdd}_{datumBis:yyyyMMdd}.xlsx");
    }

    /// <summary>
    /// Erstellt das Beschaffungscontrolling-Dashboard (Vorlage Beschaffungscontrolling.dotx)
    /// als PDF für ein Jahr: pro Kategorie (Vorankündigungen, RFI, Ausschreibungen,
    /// Berichtigungen, Abbrüche, Zuschläge, Widerrufe) Total und Quartale je Spaltengruppe
    /// (Bau, Logistik-zentral, Ämter-dezentral). Zuordnung: Publikationsdatum bzw.
    /// Publikationsdatum der Berichtigung, Zuschläge, Abbrüche und Widerrufe wie in der
    /// Zielerreichung (<see cref="ZuschlagPublikationen"/>, <see cref="AbbruchPublikationen"/>,
    /// <see cref="WiderrufPublikationen"/>). Mehrere Ereignisse pro Ausschreibung/Los
    /// zählen einzeln. Beim selektiven Verfahren zählt "zuschlagpraequali" als Zuschlag
    /// 1. Stufe und "zuschlag" als Zuschlag 2. Stufe.
    /// </summary>
    public Task<string> GetBeschaffungscontrollingAsync(int jahr)
        => MitDokumentLockAsync(() => GetBeschaffungscontrollingCoreAsync(jahr));

    /// <summary>
    /// Erstellt den Terminplan einer Ausschreibung als PDF (Vorlage Terminplan.dotx):
    /// Kopfdaten aus der Ausschreibung, Tabelle "Terminplan" mit den Verfahrensschritten.
    /// Die Dauer je Schritt wie in der App (siehe <see cref="ArbeitstageHelper.TerminDauer"/>).
    /// </summary>
    public Task<string> GetTerminplanAsync(Guid ausId)
        => MitDokumentLockAsync(() => GetTerminplanCoreAsync(ausId));

    /// <summary>
    /// Erstellt den Bericht über die Zielerreichung (Vorlage Zielerreichung.dotx) als PDF:
    /// 4 Seiten (Total, Bau, Logistik-zentral, Ämter-dezentral) mit je 9 Abschnitten.
    /// Gezählt wird dieselbe Menge wie im Beschaffungscontrolling, jede Publikation im Zeitraum
    /// ihrer eigenen Publikation (<see cref="ZuschlagPublikationen"/>, <see cref="AbbruchPublikationen"/>,
    /// <see cref="WiderrufPublikationen"/>). Gemessen wird die Bearbeitungsdauer in Arbeitstagen
    /// (exkl. Publikationstag) zwischen Eingereicht und Publikation gegen die Frist aus der
    /// Konfiguration (FristPublikation); Zuschläge, Präqualifikationen (einmal pro Ausschreibung) und Widerrufe mit den
    /// Daten ihres Offertenzustands, Abbrüche mit denen des Loses, Freihänder sowie Widerrufe von RFI und Vorankündigungen
    /// mit denen der Ausschreibung.
    /// </summary>
    public Task<string> GetZielerreichungAsync(DateOnly datumVon, DateOnly datumBis)
        => MitDokumentLockAsync(() => GetZielerreichungCoreAsync(datumVon, datumBis));

    private async Task<string> GetAbbruchWiderrufCoreAsync(int jahrVon, Departement? dep, List<Amt>? aemter, bool excel)
    {
        var jahrBis = jahrVon + 4;
        var selektion = AmtSelektionErmitteln(dep, aemter);
        var amtIdList = selektion.AmtIds;
        var depId = selektion.DepId;

        var datumVon = new DateOnly(jahrVon, 1, 1);
        var datumBis = new DateOnly(jahrBis, 12, 31);

        await using var context = await _dbFactory.CreateDbContextAsync();

        // 1. Teil: Los-Abbrüche, eine Zeile pro Los. «Nach Evaluation» heisst: Das Los hatte Offerten, reine
        // Teilnahmeanträge der Phase 1 zählen nicht.
        var teilnahmeantraege = OfferteHelper.NurTeilnahmeantraege(context);
        var abbrueche = await (
            from abbruch in AbbruchPublikationen(context, datumVon, datumBis)
            let ausschreibung = abbruch.Ausschreibung
            where (depId == null || ausschreibung.AusAmt!.AmtDepId == depId) &&
                  (amtIdList == null || amtIdList.Contains(ausschreibung.AusAmtId!.Value))
            select new
            {
                Jahr = abbruch.MassgebendesDatum.Year,
                ausschreibung.AusId,
                ausschreibung.AusPra.PraNaturalId,
                HatOfferten = abbruch.Los.TOfferte.Any(o => !teilnahmeantraege.Contains(o.OffId))
            }).ToListAsync();

        // Gruppierte Informationen für Los-Zählungen vorbereiten
        var ausIds = abbrueche.Select(x => x.AusId).Distinct().ToList();
        var losInfos = await context.TLos
            .Where(l => ausIds.Contains(l.LosAusId))
            .GroupBy(l => l.LosAusId)
            .Select(g => new
            {
                AusId = g.Key,
                Total = g.Count(),
                Abgebrochen = g.Count(l => l.LosAbbruchdatum != null)
            }).ToDictionaryAsync(x => x.AusId);

        // Pro Los eine Zeile — keine Gruppierung, sonst würden gleichnamige Lose
        // derselben Ausschreibung nur einmal gezählt.
        var abbruchZeilen = abbrueche
            .Select(x =>
            {
                var info = losInfos[x.AusId];
                return new AbbruchWiderruf
                {
                    ProjektArt = x.PraNaturalId,
                    Jahr = x.Jahr,
                    AbbruchGesamt = info.Total == info.Abgebrochen,
                    AbbruchNachEvaluation = x.HatOfferten,
                    Widerrufen = false
                };
            }).ToList();

        // 2. Teil: Widerrufe, eine Zeile pro Zustand
        var widerrufe = await (
            from widerruf in WiderrufPublikationen(context, datumVon, datumBis)
            let ausschreibung = widerruf.Ausschreibung
            where (depId == null || ausschreibung.AusAmt!.AmtDepId == depId) &&
                  (amtIdList == null || amtIdList.Contains(ausschreibung.AusAmtId!.Value))
            select new AbbruchWiderruf
            {
                ProjektArt = ausschreibung.AusPra.PraNaturalId!,
                Jahr = widerruf.MassgebendesDatum.Year,
                AbbruchGesamt = false,
                AbbruchNachEvaluation = false,
                Widerrufen = true
            }).ToListAsync();

        // Gruppiere Resultate zur schnelleren Auswertung
        var anzahlProKombination = abbruchZeilen.Concat(widerrufe)
            .GroupBy(r => (r.Jahr, r.ProjektArt, r.AbbruchGesamt, r.AbbruchNachEvaluation, r.Widerrufen))
            .ToDictionary(g => g.Key, g => g.Count());

        // Je Projektart 5 Zeilen (Abbruch gesamt/einzeln vor und nach Evaluation, Widerruf),
        // je Jahr eine Spalte und in Spalte 5 das Total über die fünf Jahre.
        var anzahlen = new int[10, 6];
        for (var jahr = jahrVon; jahr <= jahrBis; jahr++)
        {
            for (var p = 0; p < Projektarten.Length; p++)
            {
                var projektart = Projektarten[p];
                var rowBase = p * 5;

                anzahlen[rowBase + 0, jahr - jahrVon] = AbbruchWiderrufAnzahl(anzahlProKombination, jahr, projektart, true, false, false);
                anzahlen[rowBase + 1, jahr - jahrVon] = AbbruchWiderrufAnzahl(anzahlProKombination, jahr, projektart, false, false, false);
                anzahlen[rowBase + 2, jahr - jahrVon] = AbbruchWiderrufAnzahl(anzahlProKombination, jahr, projektart, true, true, false);
                anzahlen[rowBase + 3, jahr - jahrVon] = AbbruchWiderrufAnzahl(anzahlProKombination, jahr, projektart, false, true, false);
                anzahlen[rowBase + 4, jahr - jahrVon] = AbbruchWiderrufAnzahl(anzahlProKombination, jahr, projektart, false, false, true);
            }
        }

        for (var row = 0; row < anzahlen.GetLength(0); row++)
        {
            anzahlen[row, 5] = Enumerable.Range(0, 5).Sum(col => anzahlen[row, col]);
        }

        var listeBau = new List<string?[]>
        {
            AbbruchWiderrufZeile(0, anzahlen),
            AbbruchWiderrufZeile(1, anzahlen),
            AbbruchWiderrufTrennzeile(),
            AbbruchWiderrufZeile(2, anzahlen),
            AbbruchWiderrufZeile(3, anzahlen),
            AbbruchWiderrufZeile(4, anzahlen)
        };

        var listeLogistik = new List<string?[]>
        {
            AbbruchWiderrufZeile(5, anzahlen),
            AbbruchWiderrufZeile(6, anzahlen),
            AbbruchWiderrufTrennzeile(),
            AbbruchWiderrufZeile(7, anzahlen),
            AbbruchWiderrufZeile(8, anzahlen),
            AbbruchWiderrufZeile(9, anzahlen)
        };

        var listeTotal = new List<string?[]>
        {
            AbbruchWiderrufSummenZeile(0, 5, anzahlen),
            AbbruchWiderrufSummenZeile(1, 6, anzahlen),
            AbbruchWiderrufTrennzeile(),
            AbbruchWiderrufSummenZeile(2, 7, anzahlen),
            AbbruchWiderrufSummenZeile(3, 8, anzahlen),
            AbbruchWiderrufSummenZeile(4, 9, anzahlen)
        };

        if (excel)
        {
            return await AbbruchWiderrufExcelAsync(jahrVon, selektion, listeBau, listeLogistik, listeTotal);
        }
        return AbbruchWiderrufPdf(jahrVon, selektion, listeBau, listeLogistik, listeTotal);
    }

    private async Task<string> AbbruchWiderrufExcelAsync(int jahrVon, AmtSelektion selektion,
        List<string?[]> listeBau, List<string?[]> listeLogistik, List<string?[]> listeTotal)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Abbrüche und Widerrufe");
        worksheet.Cell(1, 1).Value = "Departement:";
        worksheet.Cell(1, 2).Value = selektion.DepBezeichnung;
        worksheet.Range(1, 2, 1, 7).Merge();
        worksheet.Cell(2, 1).Value = "Ämter:";
        worksheet.Cell(2, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
        worksheet.Cell(2, 2).Value = selektion.AmtBezeichnungen;
        worksheet.Range(2, 2, 2, 7).Merge();
        worksheet.Range(2, 2, 2, 7).Style.Alignment.WrapText = true;
        worksheet.Row(2).Height = Zeilenhoehe(selektion.AmtBezeichnungen);

        AbbruchWiderrufBlockSchreiben(worksheet, 4, jahrVon, "Bau");
        AbbruchWiderrufWerteSchreiben(worksheet, 6, listeBau);
        AbbruchWiderrufBlockSchreiben(worksheet, 13, jahrVon, "Logistik");
        AbbruchWiderrufWerteSchreiben(worksheet, 15, listeLogistik);
        AbbruchWiderrufBlockSchreiben(worksheet, 22, jahrVon, "Bau und Logistik");
        AbbruchWiderrufWerteSchreiben(worksheet, 24, listeTotal);

        worksheet.Column(1).Width = 30;
        worksheet.Range(4, 2, 29, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

        return await ExcelSpeichernAsync(workbook, $"Abbrüche_und_Widerrufe_{jahrVon}_{jahrVon + 4}.xlsx");
    }

    private string AbbruchWiderrufPdf(int jahrVon, AmtSelektion selektion,
        List<string?[]> listeBau, List<string?[]> listeLogistik, List<string?[]> listeTotal)
    {
        var platzhalter = new Dictionary<string, string>
        {
            { "[%Datumvon%]", new DateOnly(jahrVon, 1, 1).ToString("dd.MM.yyyy") },
            { "[%Datumbis%]", new DateOnly(jahrVon + 4, 12, 31).ToString("dd.MM.yyyy") },
            { "[%Departement%]", selektion.DepBezeichnung },
            { "[%Aemter%]", selektion.AmtBezeichnungen },
            { "[%J1%]", jahrVon.ToString() },
            { "[%J2%]", (jahrVon + 1).ToString() },
            { "[%J3%]", (jahrVon + 2).ToString() },
            { "[%J4%]", (jahrVon + 3).ToString() },
            { "[%J5%]", (jahrVon + 4).ToString() },
        };

        VorlageLaden("AbbruchWiderruf.dotx", nameof(GetAbbruchWiderrufAsync));

        _gemBoxDocumentService.InsertValues(platzhalter);
        _gemBoxDocumentService.FillValuesInTable("Bau", 2, 7, listeBau);
        _gemBoxDocumentService.FillValuesInTable("Logistik", 2, 7, listeLogistik);
        _gemBoxDocumentService.FillValuesInTable("BauLogistik", 2, 7, listeTotal);
        return PdfSpeichern($"Abbrüche_und_Widerrufe_{jahrVon}_{jahrVon + 4}.pdf");
    }

    private static int AbbruchWiderrufAnzahl(Dictionary<(int Jahr, string Projektart, bool AbbruchGesamt, bool AbbruchNachEvaluation, bool Widerrufen), int> anzahlProKombination,
        int jahr, string projektart, bool abbruchGesamt, bool abbruchNachEval, bool widerrufen)
    {
        anzahlProKombination.TryGetValue((jahr, projektart, abbruchGesamt, abbruchNachEval, widerrufen), out var anzahl);
        return anzahl;
    }

    /// <summary>Eine Datenzeile (Spalte 0 bleibt leer, damit der Vorlagentext stehen bleibt).</summary>
    private static string?[] AbbruchWiderrufZeile(int zeile, int[,] anzahlen)
    {
        return
        [
            null,
            anzahlen[zeile, 0].FormatNumber(),
            anzahlen[zeile, 1].FormatNumber(),
            anzahlen[zeile, 2].FormatNumber(),
            anzahlen[zeile, 3].FormatNumber(),
            anzahlen[zeile, 4].FormatNumber(),
            anzahlen[zeile, 5].FormatNumber()
        ];
    }

    /// <summary>Eine Datenzeile des Blocks "Bau und Logistik" als Summe der beiden Einzelzeilen.</summary>
    private static string?[] AbbruchWiderrufSummenZeile(int zeileA, int zeileB, int[,] anzahlen)
    {
        return
        [
            null,
            (anzahlen[zeileA, 0] + anzahlen[zeileB, 0]).FormatNumber(),
            (anzahlen[zeileA, 1] + anzahlen[zeileB, 1]).FormatNumber(),
            (anzahlen[zeileA, 2] + anzahlen[zeileB, 2]).FormatNumber(),
            (anzahlen[zeileA, 3] + anzahlen[zeileB, 3]).FormatNumber(),
            (anzahlen[zeileA, 4] + anzahlen[zeileB, 4]).FormatNumber(),
            (anzahlen[zeileA, 5] + anzahlen[zeileB, 5]).FormatNumber()
        ];
    }

    /// <summary>Zwischenzeile ohne Werte (trennt "Abbruch nach Ausschreibung" von "Abbruch aufgrund Evaluation").</summary>
    private static string?[] AbbruchWiderrufTrennzeile() => new string?[7];

    /// <summary>Schreibt Titel- und Beschriftungsspalte eines Blocks (Bau, Logistik, Bau und Logistik).</summary>
    private static void AbbruchWiderrufBlockSchreiben(IXLWorksheet worksheet, int startRow, int jahrVon, string art)
    {
        worksheet.Cell(startRow, 3).Value = jahrVon + 1;
        worksheet.Cell(startRow, 4).Value = jahrVon + 2;
        worksheet.Cell(startRow, 5).Value = jahrVon + 3;
        worksheet.Cell(startRow, 6).Value = jahrVon + 4;
        worksheet.Cell(startRow, 7).Value = "Total";
        worksheet.Cell(startRow, 2).Value = jahrVon;

        var range = worksheet.Range(startRow, 1, startRow, 7);
        range.Style.Fill.BackgroundColor = XLColor.LightGray;
        range.Style.Font.Bold = true;

        worksheet.Cell(startRow, 1).Value = art;
        worksheet.Cell(startRow + 1, 1).Value = "Abbruch nach Ausschreibung";
        worksheet.Cell(startRow + 2, 1).Value = "  -  Gesamtes Verfahren";
        worksheet.Cell(startRow + 3, 1).Value = "  -  Nur einzelne Lose";
        worksheet.Cell(startRow + 4, 1).Value = "Abbruch aufgrund Evaluation";
        worksheet.Cell(startRow + 5, 1).Value = "  -  Gesamtes Verfahren";
        worksheet.Cell(startRow + 6, 1).Value = "  -  Nur einzelne Lose";
        worksheet.Cell(startRow + 7, 1).Value = "Widerruf";
    }

    private static void AbbruchWiderrufWerteSchreiben(IXLWorksheet worksheet, int startRow, List<string?[]> zeilen)
    {
        for (var zeile = 0; zeile < zeilen.Count; zeile++)
        {
            for (var col = 1; col < 7; col++)
            {
                var wert = zeilen[zeile][col];
                if (wert == null)
                {
                    continue;
                }

                ZahlSchreiben(worksheet.Cell(startRow + zeile, col + 1), wert);
            }
        }
    }

    /// <summary>
    /// Schreibt einen bereits formatierten Wert (de-CH, z.B. "1'000") als echte Zahl in die Zelle.
    /// Die Tausendertrennung übernimmt das Zellformat, damit Excel weiterhin damit rechnen kann;
    /// ein nicht lesbarer Wert wird unverändert als Text übernommen.
    /// </summary>
    private static void ZahlSchreiben(IXLCell zelle, string wert)
    {
        if (decimal.TryParse(wert, NumberStyles.Number, CultureInfo.CurrentCulture, out var zahl))
        {
            zelle.Value = zahl;
            zelle.Style.NumberFormat.Format = ExcelZahlenformat;
        }
        else
        {
            zelle.Value = wert;
        }
    }

    private async Task<string> GetZuschlaegeCoreAsync(ZuschlaegeParameter parameter)
    {
        // Die Vorlage wird auch für den Excel-Export geladen: fehlt sie, bricht der Bericht
        // in beiden Varianten ab.
        VorlageLaden("Zuschlaege.dotx", nameof(GetZuschlaegeAsync));

        var datumVon = parameter.DatumVon;
        var datumBis = parameter.DatumBis;
        var verfahrensarten = VerfahrensartenListe(parameter.Offen, parameter.Selektiv, parameter.Freihaendig);
        var projektarten = ProjektartenListe(parameter.Logistik, parameter.Bau);
        var (rollen, rollenAnzeige) = RollenListe(parameter.Meldestelle, parameter.Vergabestelle);
        var cpvNummern = NummernListe(parameter.CpvNummern);
        var bkpNummern = NummernListe(parameter.BkpNummern);
        var selektion = AmtSelektionErmitteln(parameter.Dep, parameter.Aemter);
        var amtIdList = selektion.AmtIds;
        var depId = selektion.DepId;

        await using var context = await _dbFactory.CreateDbContextAsync();

        var losQuery =
            from z in EffektiveZuschlaege(context, datumVon, datumBis)
            let aus = z.Ausschreibung
            where verfahrensarten.Contains(aus.AusVea.VeaNaturalId!) &&
                  projektarten.Contains(aus.AusPra.PraNaturalId!) &&
                  rollen.Contains(aus.AusRia!.RiaNaturalId!) &&
                  z.Los.LosAbbruchdatum == null &&
                  (amtIdList == null || (aus.AusAmtId != null && amtIdList.Contains(aus.AusAmtId!.Value))) &&
                  (depId == null || aus.AusAmt!.AmtDepId == depId)
            select new
            {
                z.Los.LosId,
                KanId = z.Offerte.OffAnb.AnbOrt.OrtKanId ?? Guid.Empty,
                Zuschlagswert = z.Offerte.OffZuschlagswertChf ?? 0
            };

        var losIds = losQuery.Select(l => l.LosId);

        // Eingereicht sind alle Offerten der Lose mit effektivem Zuschlag: Die Zuschlagsofferte zählt
        // dort mit, die Erfolgsquote einer Zeile bleibt damit höchstens 100%. Reine Teilnahmeanträge sind keine Offerten.
        var teilnahmeantraege = OfferteHelper.NurTeilnahmeantraege(context);
        var offerteQuery =
            from off in context.TOfferte
            join anb in context.TAnbieter on off.OffAnbId equals anb.AnbId
            join ort in context.TOrtschaft on anb.AnbOrtId equals ort.OrtId
            where losIds.Contains(off.OffLosId) && !teilnahmeantraege.Contains(off.OffId)
            select new
            {
                KanId = ort.OrtKanId ?? Guid.Empty
            };

        var losList = await losQuery.ToListAsync();
        var offerteList = await offerteQuery.ToListAsync();

        var gruppiert =
            losList
                .GroupBy(l => l.KanId)
                .Select(g => new
                {
                    KanId = g.Key,
                    Anzahl = g.Count(),
                    Betrag = g.Sum(x => x.Zuschlagswert),
                    Offerten = offerteList.Count(o => o.KanId == g.Key)
                }).ToList();

        var kantone = await context.TKanton
            .Include(k => k.KanSpr)
            .ToListAsync();

        // Sprachregion ergänzen; Anbieter ohne Kanton zählen als Ausland und werden zuletzt sortiert.
        var resultat =
            (from g in gruppiert
                join kan in kantone on g.KanId equals kan.KanId into gj
                from kan in gj.DefaultIfEmpty()
                select new
                {
                    Sprachregion = kan == null ? Ausland : kan.KanSpr?.SprNaturalId ?? Ausland,
                    Kanton = kan == null ? Ausland : kan.KanBezeichnung,
                    g.Anzahl,
                    g.Betrag,
                    g.Offerten,
                    Sort = kan == null ? 1 : 0
                }).ToList();

        var totZuschlaege = resultat.Sum(r => r.Anzahl);
        var totBetrag = resultat.Sum(r => r.Betrag);
        var totOfferten = resultat.Sum(r => r.Offerten);

        var zuschlaege = new List<Zuschlaege>();
        if (parameter.Kanton)
        {
            var output = resultat
                .Where(r => r.Anzahl > 0 || r.Betrag > 0 || r.Offerten > 0)
                .OrderBy(r => r.Sort).ThenBy(r => r.Kanton)
                .Select(r => new { r.Kanton, r.Offerten, r.Betrag, r.Anzahl })
                .ToList();

            zuschlaege.AddRange(output.Select(row => new Zuschlaege
            {
                Bezeichnung = row.Kanton,
                Offerten = row.Offerten,
                Betrag = row.Betrag,
                Zuschlag = row.Anzahl,
                TotalOfferten = totOfferten,
                TotalBetrag = totBetrag,
                TotalZuschlaege = totZuschlaege
            }));
        }
        else
        {
            var output = resultat
                .Where(r => r.Anzahl > 0 || r.Betrag > 0 || r.Offerten > 0)
                .GroupBy(r => new { Sprache = r.Sprachregion, r.Sort })
                .OrderBy(g => g.Key.Sort).ThenBy(g => g.Key.Sprache)
                .Select(g => new
                {
                    Sprachregion = g.Key.Sprache,
                    Offerten = g.Sum(x => x.Offerten),
                    Betrag = g.Sum(x => x.Betrag),
                    Anzahl = g.Sum(x => x.Anzahl)
                }).ToList();

            // Bewusst ohne Fallback: ausser DE/FR/IT und "Ausland" darf hier nichts ankommen —
            // ein anderer Wert ist ein Datenfehler in den Stammdaten und soll auffallen.
            zuschlaege.AddRange(output.Select(row => new Zuschlaege
            {
                Bezeichnung = Sprachregionen[row.Sprachregion],
                Offerten = row.Offerten,
                Betrag = row.Betrag,
                Zuschlag = row.Anzahl,
                TotalOfferten = totOfferten,
                TotalBetrag = totBetrag,
                TotalZuschlaege = totZuschlaege
            }));
        }

        var texte = new ZuschlaegeFilterTexte(
            string.Join(", ", verfahrensarten),
            string.Join(", ", projektarten),
            string.Join(", ", rollenAnzeige),
            string.Join(", ", cpvNummern),
            string.Join(", ", bkpNummern),
            selektion.DepBezeichnung,
            selektion.AmtBezeichnungen);

        if (parameter.Excel)
        {
            return await ZuschlaegeExcelAsync(parameter, texte, zuschlaege, totZuschlaege, totBetrag, totOfferten);
        }
        return ZuschlaegePdf(parameter, texte, zuschlaege);
    }

    private async Task<string> ZuschlaegeExcelAsync(ZuschlaegeParameter parameter, ZuschlaegeFilterTexte texte,
        List<Zuschlaege> zuschlaege, int totZuschlaege, decimal totBetrag, int totOfferten)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Zuschläge");
        worksheet.Cell(1, 1).Value = "Verfahrensart(en):";
        worksheet.Range(1, 2, 1, 7).Merge();
        worksheet.Cell(1, 2).Value = texte.Verfahrensarten;
        worksheet.Cell(2, 1).Value = "Projektart(en):";
        worksheet.Range(2, 2, 2, 7).Merge();
        worksheet.Cell(2, 2).Value = texte.Projektarten;
        worksheet.Cell(3, 1).Value = "Rolle BBL:";
        worksheet.Range(3, 2, 3, 7).Merge();
        worksheet.Cell(3, 2).Value = texte.Rollen;
        worksheet.Cell(4, 1).Value = "CPV-Nummern:";
        worksheet.Range(4, 2, 4, 7).Merge();
        worksheet.Cell(4, 2).Value = texte.CpvAnzeige;
        worksheet.Cell(5, 1).Value = "BKP-Nummern:";
        worksheet.Range(5, 2, 5, 7).Merge();
        worksheet.Cell(5, 2).Value = texte.BkpAnzeige;
        worksheet.Cell(6, 1).Value = "Departement:";
        worksheet.Range(6, 2, 6, 7).Merge();
        worksheet.Cell(6, 2).Value = texte.Departement;
        worksheet.Cell(7, 1).Value = "Ämter:";
        worksheet.Cell(7, 2).Value = texte.Aemter;
        worksheet.Range(7, 2, 7, 7).Merge();

        var labelRange = worksheet.Range(1, 1, 7, 1);
        labelRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
        labelRange.Style.Font.Bold = true;
        var valueRange = worksheet.Range(1, 2, 7, 7);
        valueRange.Style.Alignment.WrapText = true;
        valueRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

        // Ohne Einschränkung ("Alle") bleibt die Standardhöhe der Zeile stehen.
        ZeilenhoeheSetzen(worksheet, 4, texte.CpvNummern);
        ZeilenhoeheSetzen(worksheet, 5, texte.BkpNummern);
        ZeilenhoeheSetzen(worksheet, 7, texte.Aemter);

        worksheet.Cell(9, 2).Value = "Anzahl Zuschläge";
        worksheet.Cell(9, 3).Value = "Total Zuschlagswerte CHF exkl. MwSt";
        worksheet.Cell(9, 4).Value = "Anzahl Zuschläge in %";
        worksheet.Cell(9, 5).Value = "Eingereichte Offerten";
        worksheet.Cell(9, 6).Value = "Offerten Prozent";
        worksheet.Cell(9, 7).Value = "Erfolgreiche Offerten Prozent";
        var headerRange = worksheet.Range(9, 1, 9, 7);
        headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
        headerRange.Style.Alignment.WrapText = true;
        worksheet.Row(9).Height = 45;

        worksheet.Column(1).Width = 30;
        worksheet.Column(2).Width = 16;
        worksheet.Column(3).Width = 25;
        worksheet.Column(4).Width = 11;
        worksheet.Column(5).Width = 14;
        worksheet.Column(6).Width = 12;
        worksheet.Column(7).Width = 16;

        worksheet.Range(9, 2, zuschlaege.Count + 9, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

        for (var counter = 10; counter < zuschlaege.Count + 10; counter++)
        {
            var zuschlag = zuschlaege[counter - 10];
            worksheet.Cell(counter, 1).Value = zuschlag.Bezeichnung;
            worksheet.Cell(counter, 2).Value = zuschlag.Zuschlag;
            worksheet.Cell(counter, 3).Value = zuschlag.ExcelBetrag;
            worksheet.Cell(counter, 4).Value = zuschlag.ZuschlagProzent;
            worksheet.Cell(counter, 5).Value = zuschlag.Offerten;
            worksheet.Cell(counter, 6).Value = zuschlag.OffertenProzent;
            worksheet.Cell(counter, 7).Value = zuschlag.ErfolgreichProzent;
        }

        var totalRow = zuschlaege.Count + 10;
        worksheet.Cell(totalRow, 1).Value = "Total";
        worksheet.Cell(totalRow, 2).Value = totZuschlaege;
        worksheet.Cell(totalRow, 3).Value = (double)totBetrag;
        worksheet.Cell(totalRow, 4).Value = 1.00;
        worksheet.Cell(totalRow, 5).Value = totOfferten;
        worksheet.Cell(totalRow, 6).Value = 1.00;

        var totalRange = worksheet.Range(totalRow, 1, totalRow, 7);
        totalRange.Style.Font.Bold = true;
        totalRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        worksheet.Range(10, 3, totalRow, 3).Style.NumberFormat.Format = "#,##0.00";
        worksheet.Range(10, 4, totalRow, 4).Style.NumberFormat.Format = "#,##0.00%";
        worksheet.Range(10, 6, totalRow, 7).Style.NumberFormat.Format = "#,##0.00%";

        if (!parameter.MitBetrag)
        {
            worksheet.Column(3).Delete();
        }

        var dateiname = parameter.Kanton
            ? $"Zuschläge_Kanton_{parameter.DatumVon:yyyyMMdd}_{parameter.DatumBis:yyyyMMdd}.xlsx"
            : $"Zuschläge_Sprachregion_{parameter.DatumVon:yyyyMMdd}_{parameter.DatumBis:yyyyMMdd}.xlsx";
        return await ExcelSpeichernAsync(workbook, dateiname);
    }

    private string ZuschlaegePdf(ZuschlaegeParameter parameter, ZuschlaegeFilterTexte texte, List<Zuschlaege> zuschlaege)
    {
        var mitBetrag = parameter.MitBetrag;
        var liste = new List<string[]>();
        if (zuschlaege.Count > 0)
        {
            var kopfzeile = mitBetrag ? zuschlaege[0].HeaderMitBetrag : zuschlaege[0].HeaderOhneBetrag;
            var totalzeile = mitBetrag ? zuschlaege[0].TotalMitBetrag : zuschlaege[0].TotalOhneBetrag;
            liste.Add(kopfzeile!);
            liste.AddRange(zuschlaege.Select(z => mitBetrag ? z.DetailMitBetrag : z.DetailOhneBetrag));
            liste.Add(totalzeile!);
        }

        var definition = new List<object?[]>
        {
            new object?[] { "H", 0, null },
            new object?[] { "D", 1, null },
            new object?[] { "T", 2, null }
        };

        var titel = parameter.Kanton
            ? $"Zuschläge nach Kanton vom {parameter.DatumVon:dd.MM.yyyy} - {parameter.DatumBis:dd.MM.yyyy}"
            : $"Zuschläge nach Sprachregion vom {parameter.DatumVon:dd.MM.yyyy} - {parameter.DatumBis:dd.MM.yyyy}";

        _gemBoxDocumentService.InsertValues(new Dictionary<string, string>
        {
            { "[%Titel%]", titel },
            { "[%Verfahrensarten%]", texte.Verfahrensarten },
            { "[%Projektarten%]", texte.Projektarten },
            { "[%RolleBBL%]", texte.Rollen },
            { "[%CPV-Nummern%]", texte.CpvAnzeige },
            { "[%BKP-Nummern%]", texte.BkpAnzeige },
            { "[%Departement%]", texte.Departement },
            { "[%Aemter%]", texte.Aemter },
        });

        if (zuschlaege.Count > 0)
        {
            _gemBoxDocumentService.FillTable(mitBetrag ? "MitBetrag" : "OhneBetrag", definition, liste);
        }
        _gemBoxDocumentService.DeleteTable(mitBetrag ? "OhneBetrag" : "MitBetrag");
        return PdfSpeichern($"Zuschläge_{parameter.DatumVon:yyyyMMdd}_{parameter.DatumBis:yyyyMMdd}.pdf");
    }

    private async Task<string> GetAusschreibungenCoreAsync(AusschreibungenParameter parameter)
    {
        var datumVon = parameter.DatumVon;
        var datumBis = parameter.DatumBis;

        var verfahrensarten = VerfahrensartenListe(parameter.Offen, parameter.Selektiv, parameter.Freihaendig,
            vorankuendigung: parameter.Vorankuendigung);
        var projektarten = ProjektartenListe(parameter.Logistik, parameter.Bau);
        var (rollen, rollenAnzeige) = RollenListe(parameter.Meldestelle, parameter.Vergabestelle);
        var selektion = AmtSelektionErmitteln(parameter.Dep, parameter.Aemter);
        var amtIdList = selektion.AmtIds;
        var depId = selektion.DepId;

        List<Guid>? leaIdList = null;
        if (parameter.Leistungsarten is { Count: > 0 })
        {
            leaIdList = parameter.Leistungsarten.Select(l => l.Id).ToList();
        }

        await using var context = await _dbFactory.CreateDbContextAsync();

        var query = context.TAusschreibung
            .Where(aus => aus.AusPublikation >= datumVon && aus.AusPublikation <= datumBis);

        if (leaIdList != null)
        {
            query = query.Where(aus => aus.AusLeaId != null && leaIdList.Contains(aus.AusLeaId.Value));
        }
        if (projektarten.Count > 0)
        {
            query = query.Where(aus => projektarten.Contains(aus.AusPra.PraNaturalId!));
        }
        if (rollen.Count > 0)
        {
            query = query.Where(aus => rollen.Contains(aus.AusRia!.RiaNaturalId!));
        }
        if (verfahrensarten.Count > 0)
        {
            query = query.Where(aus => verfahrensarten.Contains(aus.AusVea.VeaNaturalId!));
        }
        if (amtIdList != null)
        {
            query = query.Where(aus => aus.AusAmtId != null && amtIdList.Contains(aus.AusAmtId.Value));
        }
        else if (depId != null)
        {
            query = query.Where(aus => aus.AusAmt!.AmtDepId == depId);
        }
        if (parameter.Personalverleih)
        {
            query = query.Where(aus => aus.AusPersonalverleih);
        }
        if (parameter.BeschaffungDezentral)
        {
            query = query.Where(aus => aus.AusBeschaffungDezentral);
        }

        // Pro Ausschreibung die für beide Auswertungsmodi benötigten Daten laden
        var ausschreibungen = await query
            .Select(aus => new
            {
                aus.AusId,
                aus.AusPublikation,
                Verfahrensart = aus.AusVea.VeaNaturalId,
                Sprachen = aus.TAusschreibungSprache
                    .Select(s => new { Sprache = s.AspSpr.SprNaturalId, DokArt = s.AspDoa.DoaNaturalId })
                    .ToList()
            })
            .ToListAsync();

        var anzahlTotal = ausschreibungen.Count;

        // Auswertung nach Jahr/Quartal
        var jahrQuartale = ausschreibungen
            .Where(a => a.AusPublikation != null)
            .GroupBy(a => a.AusPublikation!.Value.Year)
            .Select(g => new JahrQuartale(
                g.Key,
                g.Count(a => Quartal(a.AusPublikation!.Value) == 1),
                g.Count(a => Quartal(a.AusPublikation!.Value) == 2),
                g.Count(a => Quartal(a.AusPublikation!.Value) == 3),
                g.Count(a => Quartal(a.AusPublikation!.Value) == 4)))
            .OrderBy(x => x.Jahr)
            .ToList();

        // Auswertung nach Sprache/Dokumentart (Anzahl Ausschreibungen je Sprache und Dokumentart)
        var dokArtPublikation = nameof(EnumDokumentart.Publikation);
        var dokArtUnterlagen = nameof(EnumDokumentart.Unterlagen);
        var dokArtOfferten = nameof(EnumDokumentart.Offerten);

        // Verfahrensarten ohne Unterlagen/Offerten (Vorankündigung/Freihändig erhalten keine Unterlagen/Offerten)
        string[] verfahrenOhneUnterlagen = [VerfahrensartFreihändig, .. VerfahrensartenVorankündigung];

        // 100%-Basis: Publikation = Gesamtanzahl; Unterlagen/Offerte = Anzahl ohne Vorankündigung/Freihändig
        var anzahlMitUnterlagen = ausschreibungen.Count(a => !verfahrenOhneUnterlagen.Contains(a.Verfahrensart));

        static int Prozent(int anzahl, int basis) => basis == 0 ? 0 : (int)Math.Round((decimal)anzahl / basis * 100);

        var sprachAnteile = Sprachen
            .Select(spr => new SprachAnteil(
                spr.Bezeichnung,
                // Publikation: Prozentsatz der Gesamtanzahl (auf Ganzzahl gerundet)
                Prozent(
                    ausschreibungen.Count(a => a.Sprachen.Any(s => s.Sprache == spr.NaturalId && s.DokArt == dokArtPublikation)),
                    anzahlTotal),
                // Unterlagen/Offerte: Prozentsatz der Anzahl ohne Vorankündigung/Freihändig (auf Ganzzahl gerundet)
                Prozent(
                    ausschreibungen.Count(a => !verfahrenOhneUnterlagen.Contains(a.Verfahrensart) && a.Sprachen.Any(s => s.Sprache == spr.NaturalId && s.DokArt == dokArtUnterlagen)),
                    anzahlMitUnterlagen),
                Prozent(
                    ausschreibungen.Count(a => !verfahrenOhneUnterlagen.Contains(a.Verfahrensart) && a.Sprachen.Any(s => s.Sprache == spr.NaturalId && s.DokArt == dokArtOfferten)),
                    anzahlMitUnterlagen)))
            .ToList();

        // Anzeige-Text aus den Selektionskriterien (nicht aus den NaturalIds): die drei
        // Vorankündigungs-Varianten erscheinen als ein Kriterium "Vorankündigung".
        var verfahrensartenAnzeige = VerfahrensartenListe(parameter.Offen, parameter.Selektiv, parameter.Freihaendig);
        if (parameter.Vorankuendigung)
        {
            verfahrensartenAnzeige.Add(VorankuendigungAnzeige);
        }

        var texte = new AusschreibungenFilterTexte(
            parameter.Leistungsarten is { Count: > 0 }
                ? string.Join(", ", parameter.Leistungsarten.Select(l => l.Bezeichnung))
                : AlleAnzeige,
            AnzeigeOderAlle(projektarten),
            AnzeigeOderAlle(verfahrensartenAnzeige),
            AnzeigeOderAlle(rollenAnzeige),
            selektion.DepBezeichnung,
            selektion.AmtBezeichnungen);

        if (parameter.Excel)
        {
            return await AusschreibungenExcelAsync(parameter, texte, jahrQuartale, sprachAnteile, anzahlTotal);
        }
        return AusschreibungenPdf(parameter, texte, jahrQuartale, sprachAnteile, anzahlTotal);
    }

    private async Task<string> AusschreibungenExcelAsync(AusschreibungenParameter parameter, AusschreibungenFilterTexte texte,
        List<JahrQuartale> jahrQuartale, List<SprachAnteil> sprachAnteile, int anzahlTotal)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Ausschreibungen");
        worksheet.Cell(1, 1).Value = "Leistungsart(en):";
        worksheet.Cell(1, 2).Value = texte.Leistungsarten;
        worksheet.Range(1, 2, 1, 6).Merge();
        worksheet.Cell(2, 1).Value = "Projektart(en):";
        worksheet.Cell(2, 2).Value = texte.Projektarten;
        worksheet.Range(2, 2, 2, 6).Merge();
        worksheet.Cell(3, 1).Value = "Verfahrensart(en):";
        worksheet.Cell(3, 2).Value = texte.Verfahrensarten;
        worksheet.Range(3, 2, 3, 6).Merge();
        worksheet.Cell(4, 1).Value = "Rolle BBL:";
        worksheet.Cell(4, 2).Value = texte.Rollen;
        worksheet.Range(4, 2, 4, 6).Merge();
        worksheet.Cell(5, 1).Value = "Departement:";
        worksheet.Cell(5, 2).Value = texte.Departement;
        worksheet.Range(5, 2, 5, 6).Merge();
        worksheet.Cell(6, 1).Value = "Ämter:";
        worksheet.Cell(6, 2).Value = texte.Aemter;
        worksheet.Range(6, 2, 6, 6).Merge();
        worksheet.Range(1, 1, 6, 1).Style.Font.Bold = true;
        worksheet.Column(1).Width = 30;
        worksheet.Column(2).Width = 15;
        worksheet.Column(3).Width = 15;
        worksheet.Column(4).Width = 15;
        worksheet.Column(5).Width = 15;
        worksheet.Column(6).Width = 15;

        var valueRange = worksheet.Range(1, 2, 6, 6);
        valueRange.Style.Alignment.WrapText = true;
        valueRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

        const int startRow = 8;
        if (parameter.NachJahrQuartalen)
        {
            worksheet.Cell(startRow, 1).Value = "Jahr";
            worksheet.Cell(startRow, 2).Value = "1. Quartal";
            worksheet.Cell(startRow, 3).Value = "2. Quartal";
            worksheet.Cell(startRow, 4).Value = "3. Quartal";
            worksheet.Cell(startRow, 5).Value = "4. Quartal";
            worksheet.Cell(startRow, 6).Value = "Total";
            var header = worksheet.Range(startRow, 1, startRow, 6);
            header.Style.Font.Bold = true;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;

            var row = startRow + 1;
            foreach (var j in jahrQuartale)
            {
                worksheet.Cell(row, 1).Value = j.Jahr;
                worksheet.Cell(row, 2).Value = j.Q1;
                worksheet.Cell(row, 3).Value = j.Q2;
                worksheet.Cell(row, 4).Value = j.Q3;
                worksheet.Cell(row, 5).Value = j.Q4;
                worksheet.Cell(row, 6).Value = j.Total;
                row++;
            }
            worksheet.Range(startRow, 2, row - 1, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        }
        else
        {
            worksheet.Cell(startRow - 1, 1).Value = "Anzahl Ausschreibungen:";
            worksheet.Cell(startRow - 1, 2).Value = anzahlTotal;
            worksheet.Cell(startRow - 1, 1).Style.Font.Bold = true;
            worksheet.Cell(startRow, 1).Value = "Sprache";
            worksheet.Cell(startRow, 2).Value = "Sprache Publikation";
            worksheet.Cell(startRow, 3).Value = "Sprache Unterlagen";
            worksheet.Cell(startRow, 4).Value = "Sprache Offerte";
            var header = worksheet.Range(startRow, 1, startRow, 4);
            header.Style.Font.Bold = true;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;

            var row = startRow + 1;
            foreach (var s in sprachAnteile)
            {
                worksheet.Cell(row, 1).Value = s.Bezeichnung;
                worksheet.Cell(row, 2).Value = $"{s.PublikationProzent.FormatNumber()}%";
                worksheet.Cell(row, 3).Value = $"{s.UnterlagenProzent.FormatNumber()}%";
                worksheet.Cell(row, 4).Value = $"{s.OfferteProzent.FormatNumber()}%";
                row++;
            }
            worksheet.Range(startRow, 2, row - 1, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        }

        var dateiname = parameter.NachJahrQuartalen
            ? $"Ausschreibungen_JahrQuartal_{parameter.DatumVon:yyyyMMdd}_{parameter.DatumBis:yyyyMMdd}.xlsx"
            : $"Ausschreibungen_Sprache_{parameter.DatumVon:yyyyMMdd}_{parameter.DatumBis:yyyyMMdd}.xlsx";
        return await ExcelSpeichernAsync(workbook, dateiname);
    }

    private string AusschreibungenPdf(AusschreibungenParameter parameter, AusschreibungenFilterTexte texte,
        List<JahrQuartale> jahrQuartale, List<SprachAnteil> sprachAnteile, int anzahlTotal)
    {
        VorlageLaden("Ausschreibungen.dotx", nameof(GetAusschreibungenAsync));

        var titel = parameter.NachJahrQuartalen
            ? $"Ausschreibungen nach Jahr/Quartal vom {parameter.DatumVon:dd.MM.yyyy} - {parameter.DatumBis:dd.MM.yyyy}"
            : $"Ausschreibungen nach Sprache vom {parameter.DatumVon:dd.MM.yyyy} - {parameter.DatumBis:dd.MM.yyyy}";

        _gemBoxDocumentService.InsertValues(new Dictionary<string, string>
        {
            { "[%Titel%]", titel },
            { "[%Leistungsarten%]", texte.Leistungsarten },
            { "[%Projektarten%]", texte.Projektarten },
            { "[%Verfahrensarten%]", texte.Verfahrensarten },
            { "[%RolleBBL%]", texte.Rollen },
            { "[%Departement%]", texte.Departement },
            { "[%Aemter%]", texte.Aemter },
            { "[%Anzahl%]", parameter.NachJahrQuartalen
                ? string.Empty
                : $"Anzahl Ausschreibungen: {anzahlTotal.FormatNumber()}" }
        });

        var definition = new List<object?[]>
        {
            new object?[] { "H", 0, null },
            new object?[] { "D", 1, null }
        };

        if (parameter.NachJahrQuartalen)
        {
            var liste = new List<string[]>
            {
                new[] { "H", "", "1. Quartal", "2. Quartal", "3. Quartal", "4. Quartal", "Total" }
            };
            liste.AddRange(jahrQuartale.Select(j => new[]
            {
                "D",
                j.Jahr.ToString(),
                j.Q1.FormatNumber(),
                j.Q2.FormatNumber(),
                j.Q3.FormatNumber(),
                j.Q4.FormatNumber(),
                j.Total.FormatNumber()
            }));
            _gemBoxDocumentService.FillTable("JahrUndQuartal", definition, liste);
            _gemBoxDocumentService.DeleteTable("Sprache");
        }
        else
        {
            var liste = new List<string[]>
            {
                new[] { "H", "", "Sprache Publikation", "Sprache Unterlagen", "Sprache Offerte" }
            };
            liste.AddRange(sprachAnteile.Select(s => new[]
            {
                "D",
                s.Bezeichnung,
                $"{s.PublikationProzent.FormatNumber()}%",
                $"{s.UnterlagenProzent.FormatNumber()}%",
                $"{s.OfferteProzent.FormatNumber()}%"
            }));
            _gemBoxDocumentService.FillTable("Sprache", definition, liste);
            _gemBoxDocumentService.DeleteTable("JahrUndQuartal");
        }

        return PdfSpeichern($"Ausschreibungen_{parameter.DatumVon:yyyyMMdd}_{parameter.DatumBis:yyyyMMdd}.pdf");
    }

    private async Task<string> GetBeschaffungscontrollingCoreAsync(int jahr)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        string[] verfahrenPublikation = [.. VerfahrensartenVorankündigung, VerfahrensartRFI, .. VerfahrensartenAusschreibung];

        var publikationen = await ControllingPublikationenAsync(context, jahr, verfahrenPublikation);
        var berichtigungen = await ControllingBerichtigungenAsync(context, jahr, VerfahrensartenAusschreibung);
        var abbrueche = await ControllingAbbruecheAsync(context, jahr);
        var zuschlaege = await ControllingZuschlaegeAsync(context, jahr);
        var widerrufe = await ControllingWiderrufeAsync(context, jahr);

        var vorankuendigungen = publikationen.Where(e => VerfahrensartenVorankündigung.Contains(e.Verfahrensart)).ToList();
        var rfi = publikationen.Where(e => e.Verfahrensart == VerfahrensartRFI).ToList();
        var ausschreibungen = publikationen.Where(e => VerfahrensartenAusschreibung.Contains(e.Verfahrensart)).ToList();

        var gesamtTotal = vorankuendigungen.Count + rfi.Count + ausschreibungen.Count +
                          berichtigungen.Count + abbrueche.Count + zuschlaege.Count + widerrufe.Count;

        // Zeilen in der Reihenfolge der Vorlage (Tabelle "Controlling", ab Zeile 2)
        var werteZeilen = new List<string?[]>();

        void Haupt(List<ControllingEreignis> kategorie) =>
            werteZeilen.Add(ControllingZeile(kategorie,
                FormatProzent(gesamtTotal > 0 ? 100d * kategorie.Count / gesamtTotal : 0),
                kategorie.Count > 0 ? "100%" : "0%"));

        void Sub(List<ControllingEreignis> kategorie, Func<ControllingEreignis, bool> filter)
        {
            var sub = kategorie.Where(filter).ToList();
            werteZeilen.Add(ControllingZeile(sub, null,
                FormatProzent(kategorie.Count > 0 ? 100d * sub.Count / kategorie.Count : 0)));
        }

        void SubVerfahren(List<ControllingEreignis> kategorie)
        {
            Sub(kategorie, e => e.Verfahrensart == VerfahrensartSelektiv);
            Sub(kategorie, e => e.Verfahrensart == VerfahrensartOffen);
            Sub(kategorie, e => e.Verfahrensart == VerfahrensartEinladung);
        }

        Haupt(vorankuendigungen);
        Sub(vorankuendigungen, e => e.Verfahrensart == VerfahrensartVorankündigungSelektiv);
        Sub(vorankuendigungen, e => e.Verfahrensart == VerfahrensartVorankündigungOffen);
        Sub(vorankuendigungen, e => e.Verfahrensart == VerfahrensartVorankündigungUnbekannt);
        Haupt(rfi);
        Haupt(ausschreibungen);
        SubVerfahren(ausschreibungen);
        Haupt(berichtigungen);
        SubVerfahren(berichtigungen);
        Haupt(abbrueche);
        SubVerfahren(abbrueche);
        Haupt(zuschlaege);
        Sub(zuschlaege, e => e.Zustand == OffertezustandZuschlag && e.Verfahrensart == VerfahrensartFreihändig);
        Sub(zuschlaege, e => e.Zustand == OffertezustandZuschlagPraequali);
        Sub(zuschlaege, e => e.Zustand == OffertezustandZuschlag && e.Verfahrensart == VerfahrensartSelektiv);
        Sub(zuschlaege, e => e.Zustand == OffertezustandZuschlag && e.Verfahrensart == VerfahrensartOffen);
        Sub(zuschlaege, e => e.Zustand == OffertezustandZuschlag && e.Verfahrensart == VerfahrensartEinladung);
        Haupt(widerrufe);
        Sub(widerrufe, e => e.Verfahrensart == VerfahrensartFreihändig);
        SubVerfahren(widerrufe);

        // Totalzeile über alle Kategorien
        var alle = vorankuendigungen.Concat(rfi).Concat(ausschreibungen).Concat(berichtigungen)
            .Concat(abbrueche).Concat(zuschlaege).Concat(widerrufe).ToList();
        werteZeilen.Add(ControllingZeile(alle, gesamtTotal > 0 ? "100%" : "0%", null));

        // Quartalsverteilung in Prozent je Spaltengruppe (letzte Zeile)
        var prozentZeile = new string?[19];
        for (var g = 0; g < ControllingGruppen.Length; g++)
        {
            var gruppe = alle.Where(ControllingGruppen[g]).ToList();
            prozentZeile[4 + g * 5] = ProzentEinstellig(gruppe.Count, gruppe.Count);
            for (var q = 1; q <= 4; q++)
            {
                prozentZeile[4 + g * 5 + q] = ProzentEinstellig(gruppe.Count(e => Quartal(e.Datum) == q), gruppe.Count);
            }
        }
        werteZeilen.Add(prozentZeile);

        VorlageLaden("Beschaffungscontrolling.dotx", nameof(GetBeschaffungscontrollingAsync));

        _gemBoxDocumentService.InsertValues(new Dictionary<string, string>
        {
            { "[%DatumVon%]", $"01.01.{jahr}" },
            { "[%DatumBis%]", $"31.12.{jahr}" }
        });

        TabelleFuellen("Controlling", 2, 31, werteZeilen, nameof(GetBeschaffungscontrollingAsync));

        return PdfSpeichern($"Beschaffungscontrolling_{jahr}.pdf");
    }

    /// <summary>Publikations-Ereignisse: Vorankündigungen, RFI und Ausschreibungen.</summary>
    private static async Task<List<ControllingEreignis>> ControllingPublikationenAsync(DoeAContext context, int jahr, string[] verfahrensarten)
    {
        return (await (
            from aus in context.TAusschreibung
            join vea in context.TVerfahrensart on aus.AusVeaId equals vea.VeaId
            join pra in context.TProjektart on aus.AusPraId equals pra.PraId
            join ria in context.TRolleInAusschreibung on aus.AusRiaId equals ria.RiaId into riaJoin
            from ria in riaJoin.DefaultIfEmpty()
            where aus.AusPublikation != null &&
                  aus.AusPublikation!.Value.Year == jahr &&
                  verfahrensarten.Contains(vea.VeaNaturalId!)
            select new
            {
                Vea = vea.VeaNaturalId,
                Datum = aus.AusPublikation!.Value,
                Projektart = pra.PraNaturalId,
                Rolle = ria != null ? ria.RiaNaturalId : null
            }).ToListAsync())
            .Select(x => new ControllingEreignis(x.Vea, x.Datum, x.Projektart, x.Rolle))
            .ToList();
    }

    /// <summary>Berichtigungen (jede Berichtigung zählt einzeln, Datum = Publikation der Berichtigung).</summary>
    private static async Task<List<ControllingEreignis>> ControllingBerichtigungenAsync(DoeAContext context, int jahr, string[] verfahrensarten)
    {
        return (await (
            from ber in context.TBerichtigung
            join aus in context.TAusschreibung on ber.BerAusId equals aus.AusId
            join vea in context.TVerfahrensart on aus.AusVeaId equals vea.VeaId
            join pra in context.TProjektart on aus.AusPraId equals pra.PraId
            join ria in context.TRolleInAusschreibung on aus.AusRiaId equals ria.RiaId into riaJoin
            from ria in riaJoin.DefaultIfEmpty()
            where ber.BerPubliziert != null &&
                  ber.BerPubliziert!.Value.Year == jahr &&
                  verfahrensarten.Contains(vea.VeaNaturalId!)
            select new
            {
                Vea = vea.VeaNaturalId,
                Datum = ber.BerPubliziert!.Value,
                Projektart = pra.PraNaturalId,
                Rolle = ria != null ? ria.RiaNaturalId : null
            }).ToListAsync())
            .Select(x => new ControllingEreignis(x.Vea, x.Datum, x.Projektart, x.Rolle))
            .ToList();
    }

    /// <summary>Abbrüche (<see cref="AbbruchPublikationen"/>), jedes abgebrochene Los zählt einzeln.</summary>
    private static async Task<List<ControllingEreignis>> ControllingAbbruecheAsync(DoeAContext context, int jahr)
    {
        return (await (
            from abbruch in AbbruchPublikationen(context, new DateOnly(jahr, 1, 1), new DateOnly(jahr, 12, 31))
            let aus = abbruch.Ausschreibung
            select new
            {
                Vea = aus.AusVea.VeaNaturalId,
                Datum = abbruch.MassgebendesDatum,
                Projektart = aus.AusPra.PraNaturalId,
                Rolle = aus.AusRia != null ? aus.AusRia.RiaNaturalId : null
            }).ToListAsync())
            .Select(x => new ControllingEreignis(x.Vea, x.Datum, x.Projektart, x.Rolle))
            .ToList();
    }

    /// <summary>
    /// Widerrufe (<see cref="WiderrufPublikationen"/>), jeder Zustand zählt einzeln; dazu die Widerrufe von RFI und
    /// Vorankündigungen, die keine Lose haben (Publikation des Widerrufs in der Ausschreibung).
    /// </summary>
    private static async Task<List<ControllingEreignis>> ControllingWiderrufeAsync(DoeAContext context, int jahr)
    {
        var widerrufe = await (
            from widerruf in WiderrufPublikationen(context, new DateOnly(jahr, 1, 1), new DateOnly(jahr, 12, 31))
            let aus = widerruf.Ausschreibung
            select new
            {
                Vea = aus.AusVea.VeaNaturalId,
                Datum = widerruf.MassgebendesDatum,
                Projektart = aus.AusPra.PraNaturalId,
                Rolle = aus.AusRia != null ? aus.AusRia.RiaNaturalId : null
            }).ToListAsync();

        var widerrufeOhneLose = await (
            from aus in context.TAusschreibung
            where aus.AusPublikationWiderruf != null &&
                  aus.AusPublikationWiderruf!.Value.Year == jahr &&
                  VerfahrensartenWiderrufOhneLose.Contains(aus.AusVea.VeaNaturalId!)
            select new
            {
                Vea = aus.AusVea.VeaNaturalId,
                Datum = aus.AusPublikationWiderruf!.Value,
                Projektart = aus.AusPra.PraNaturalId,
                Rolle = aus.AusRia != null ? aus.AusRia.RiaNaturalId : null
            }).ToListAsync();

        return widerrufe.Concat(widerrufeOhneLose)
            .Select(x => new ControllingEreignis(x.Vea, x.Datum, x.Projektart, x.Rolle))
            .ToList();
    }

    /// <summary>
    /// Zuschläge inkl. 1. Stufe Präqualifikation (<see cref="ZuschlagPublikationen"/>): Jeder Zuschlag zählt einzeln, die
    /// Präqualifikation einmal pro Ausschreibung (ihre Zuschläge haben dieselben Daten, LosService prüft das).
    /// </summary>
    private static async Task<List<ControllingEreignis>> ControllingZuschlaegeAsync(DoeAContext context, int jahr)
    {
        var zuschlaege = await (
            from zuschlag in ZuschlagPublikationen(context, new DateOnly(jahr, 1, 1), new DateOnly(jahr, 12, 31))
            let aus = zuschlag.Ausschreibung
            select new
            {
                aus.AusId,
                Vea = aus.AusVea.VeaNaturalId,
                Datum = zuschlag.MassgebendesDatum,
                Projektart = aus.AusPra.PraNaturalId,
                Rolle = aus.AusRia != null ? aus.AusRia.RiaNaturalId : null,
                Zustand = zuschlag.Zustand.OozOfz.OfzNaturalId
            }).ToListAsync();

        return zuschlaege.Where(x => x.Zustand != OffertezustandZuschlagPraequali)
            .Concat(zuschlaege.Where(x => x.Zustand == OffertezustandZuschlagPraequali).DistinctBy(x => x.AusId))
            .Select(x => new ControllingEreignis(x.Vea, x.Datum, x.Projektart, x.Rolle, x.Zustand))
            .ToList();
    }

    /// <summary>
    /// Eine Datenzeile der Controlling-Tabelle: Prozentspalten, Gesamttotal-Anzahl und
    /// je Spaltengruppe Total und Quartale (Q1 = 1.1.-31.3., Q2 = 1.4.-30.6.,
    /// Q3 = 1.7.-30.9., Q4 = 1.10.-31.12.). Die Beschriftungsspalte bleibt null,
    /// damit FillValuesInTable den Vorlagentext stehen lässt.
    /// </summary>
    private static string?[] ControllingZeile(List<ControllingEreignis> ereignisse, string? gtProzent, string? artProzent)
    {
        var werte = new string?[19];
        werte[1] = gtProzent;
        werte[2] = artProzent;
        werte[3] = ereignisse.Count.FormatNumber();
        for (var g = 0; g < ControllingGruppen.Length; g++)
        {
            var gruppe = ereignisse.Where(ControllingGruppen[g]).ToList();
            werte[4 + g * 5] = gruppe.Count.FormatNumber();
            for (var q = 1; q <= 4; q++)
            {
                werte[4 + g * 5 + q] = gruppe.Count(e => Quartal(e.Datum) == q).FormatNumber();
            }
        }
        return werte;
    }

    private async Task<string> GetTerminplanCoreAsync(Guid ausId)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        var aus = await (
            from a in context.TAusschreibung
            join amt in context.TAmt on a.AusAmtId equals amt.AmtId into amtJoin
            from amt in amtJoin.DefaultIfEmpty()
            join doea in context.TSachbearbeiter on a.AusSacDoeAId equals doea.SacId into doeaJoin
            from doea in doeaJoin.DefaultIfEmpty()
            join rd in context.TSachbearbeiter on a.AusSacRdId equals rd.SacId into rdJoin
            from rd in rdJoin.DefaultIfEmpty()
            join bbl in context.TSachbearbeiter on a.AusSacBblId equals bbl.SacId into bblJoin
            from bbl in bblJoin.DefaultIfEmpty()
            where a.AusId == ausId
            select new
            {
                a.AusNummerStr,
                a.AusBezeichnung,
                Amt = amt.AmtBezeichnung,
                VerantwortlichDoeA = doea == null ? null : doea.SacVorname + " " + doea.SacNachname,
                VerantwortlichRd = rd == null ? null : rd.SacVorname + " " + rd.SacNachname,
                VerantwortlichBbl = bbl == null ? null : bbl.SacVorname + " " + bbl.SacNachname,
                a.AusExtern
            }).FirstOrDefaultAsync();

        if (aus == null)
        {
            _logger.Error(GeneralConstants.MessageTemplate, nameof(AuswertungService), nameof(GetTerminplanAsync), $"Ausschreibung '{ausId}' nicht gefunden");
            throw new InvalidOperationException(GeneralConstants.HandledException);
        }

        // Dieselbe Quelle wie die Terminplanung in der App, damit Reihenfolge und Dauer übereinstimmen.
        var schritte = await _ausschreibungVerfahrensschrittService.Load(ausId);

        VorlageLaden("Terminplan.dotx", nameof(GetTerminplanAsync));

        _gemBoxDocumentService.InsertValues(new Dictionary<string, string>
        {
            { "[%Projektnummer%]", aus.AusNummerStr ?? string.Empty },
            { "[%ProjektTitel%]", aus.AusBezeichnung ?? string.Empty },
            { "[%Aemter%]", aus.Amt ?? string.Empty },
            { "[%VerantwortlichDoeA%]", aus.VerantwortlichDoeA ?? string.Empty },
            { "[%VerantwortlichRD%]", aus.VerantwortlichRd ?? string.Empty },
            { "[%VerantwortlichBBL%]", aus.VerantwortlichBbl ?? string.Empty },
            { "[%VerantwortlichExtern%]", aus.AusExtern ?? string.Empty }
        });

        // Zeile 0 = Kopfzeile (Texte aus der Vorlage übernehmen), Zeile 1 = Vorlage für die Datenzeilen
        var definition = new List<object?[]>
        {
            new object?[] { "H", 0, null },
            new object?[] { "D", 1, null }
        };

        var liste = new List<string[]>
        {
            new string[] { "H", null!, null!, null!, null! }
        };
        liste.AddRange(schritte.Select(s => new[]
        {
            "D",
            s.Verfahrensschritt.Bezeichnung,
            s.DatumVon?.ToString("dd.MM.yyyy") ?? string.Empty,
            s.DatumBis?.ToString("dd.MM.yyyy") ?? string.Empty,
            s.Dauer
        }));

        _gemBoxDocumentService.FillTable("Terminplan", definition, liste);

        return PdfSpeichern($"Terminplan_{aus.AusNummerStr ?? ausId.ToString()}.pdf");
    }

    private async Task<string> GetZielerreichungCoreAsync(DateOnly datumVon, DateOnly datumBis)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        var frist = await GetKonfigurationIntAsync(context, GeneralConstants.KonfigurationFristPublikation);
        var soll = await GetKonfigurationIntAsync(context, "MinimaleZielerreichung");

        // Basis für RFI, Vorankündigungen und Ausschreibungen: alle Ausschreibungen mit Publikation
        // im Selektionsbereich.
        var basis = await (
            from aus in context.TAusschreibung
            join pra in context.TProjektart on aus.AusPraId equals pra.PraId
            join vea in context.TVerfahrensart on aus.AusVeaId equals vea.VeaId
            join ria in context.TRolleInAusschreibung on aus.AusRiaId equals ria.RiaId into riaJoin
            from ria in riaJoin.DefaultIfEmpty()
            where aus.AusPublikation != null &&
                  aus.AusPublikation >= datumVon &&
                  aus.AusPublikation <= datumBis
            select new ZielBasis
            {
                Eingereicht = aus.AusEingereicht,
                Publikation = aus.AusPublikation,
                VerzoegerungDurchAuftraggeber = aus.AusVerzoegerungDurchAuftraggeber,
                Projektart = pra.PraNaturalId,
                Verfahrensart = vea.VeaNaturalId,
                Rolle = ria != null ? ria.RiaNaturalId : null
            }).ToListAsync();

        // Berichtigungen: jede Berichtigung mit eigener Publikation im Selektionsbereich zählt einzeln
        // (unabhängig vom Publikationsdatum der Ausschreibung).
        var berichtigungMessungen = await (
            from ber in context.TBerichtigung
            join aus in context.TAusschreibung on ber.BerAusId equals aus.AusId
            join pra in context.TProjektart on aus.AusPraId equals pra.PraId
            join vea in context.TVerfahrensart on aus.AusVeaId equals vea.VeaId
            join ria in context.TRolleInAusschreibung on aus.AusRiaId equals ria.RiaId into riaJoin
            from ria in riaJoin.DefaultIfEmpty()
            where ber.BerPubliziert != null &&
                  ber.BerPubliziert >= datumVon &&
                  ber.BerPubliziert <= datumBis &&
                  VerfahrensartenAusschreibung.Contains(vea.VeaNaturalId!)
            select new ZielMessung(ber.BerEingereicht, ber.BerPubliziert, ber.BerVerzoegerungDurchAuftraggeber,
                pra.PraNaturalId, ria != null ? ria.RiaNaturalId : null)).ToListAsync();

        // Zuschläge messen mit den Daten ihres Zustands (Eingereicht, Datum = Publikation, Verzögerung). Ausnahme: Freihänder
        // haben ihre Zustell- und Publikationsdaten in der Ausschreibung.
        var zuschlagMessungen = await (
            from zuschlag in ZuschlagPublikationen(context, datumVon, datumBis)
            let ooz = zuschlag.Zustand
            let aus = zuschlag.Ausschreibung
            let freihaendig = aus.AusVea.VeaNaturalId == VerfahrensartFreihändig
            select new
            {
                aus.AusId,
                Praequali = ooz.OozOfz.OfzNaturalId == OffertezustandZuschlagPraequali,
                Messung = new ZielMessung(
                    freihaendig ? aus.AusEingereicht : ooz.OozEingereicht,
                    freihaendig ? aus.AusPublikation : ooz.OozDatum,
                    freihaendig ? aus.AusVerzoegerungDurchAuftraggeber : ooz.OozVerzoegerungDurchAuftraggeber,
                    aus.AusPra.PraNaturalId, aus.AusRia != null ? aus.AusRia.RiaNaturalId : null)
            }).ToListAsync();

        var abbruchMessungen = await (
            from abbruch in AbbruchPublikationen(context, datumVon, datumBis)
            let los = abbruch.Los
            let aus = abbruch.Ausschreibung
            select new ZielMessung(los.LosEingereicht, los.LosAbbruchdatum, los.LosVerzoegerungDurchAuftraggeber,
                aus.AusPra.PraNaturalId, aus.AusRia != null ? aus.AusRia.RiaNaturalId : null)).ToListAsync();

        var widerrufMessungen = await (
            from widerruf in WiderrufPublikationen(context, datumVon, datumBis)
            let ooz = widerruf.Zustand
            let aus = widerruf.Ausschreibung
            select new ZielMessung(ooz.OozEingereicht, ooz.OozDatum, ooz.OozVerzoegerungDurchAuftraggeber,
                aus.AusPra.PraNaturalId, aus.AusRia != null ? aus.AusRia.RiaNaturalId : null)).ToListAsync();

        // Widerrufe von RFI und Vorankündigungen (ohne Lose): Daten in der Ausschreibung, eigene Publikation im Selektionsbereich.
        var widerrufOhneLoseMessungen = await (
            from aus in context.TAusschreibung
            where aus.AusPublikationWiderruf != null &&
                  aus.AusPublikationWiderruf >= datumVon &&
                  aus.AusPublikationWiderruf <= datumBis &&
                  VerfahrensartenWiderrufOhneLose.Contains(aus.AusVea.VeaNaturalId!)
            select new ZielMessung(aus.AusEingereichtWiderruf, aus.AusPublikationWiderruf, aus.AusVerzoegerungDurchAuftraggeberWiderruf,
                aus.AusPra.PraNaturalId, aus.AusRia != null ? aus.AusRia.RiaNaturalId : null)).ToListAsync();

        // Messungen pro Abschnitt (Index = Abschnitts-Nr. in der Vorlage: Tabelle{Seite}.{Abschnitt}).
        ZielMessung MessungAus(ZielBasis aus) =>
            new(aus.Eingereicht, aus.Publikation, aus.VerzoegerungDurchAuftraggeber, aus.Projektart, aus.Rolle);

        List<ZielMessung>[] abschnitte =
        [
            basis.Where(b => b.Verfahrensart == VerfahrensartEinladung).Select(MessungAus).ToList(),
            basis.Where(b => b.Verfahrensart == VerfahrensartRFI).Select(MessungAus).ToList(),
            basis.Where(b => VerfahrensartenVorankündigung.Contains(b.Verfahrensart)).Select(MessungAus).ToList(),
            // Einladungen haben ihren eigenen Abschnitt.
            basis.Where(b => b.Verfahrensart is VerfahrensartOffen or VerfahrensartSelektiv).Select(MessungAus).ToList(),
            berichtigungMessungen,
            zuschlagMessungen.Where(z => !z.Praequali).Select(z => z.Messung).ToList(),
            // Die Präqualifikation zählt pro Ausschreibung: Ihre Zuschläge haben dieselben Daten (LosService prüft das).
            zuschlagMessungen.Where(z => z.Praequali).DistinctBy(z => z.AusId).Select(z => z.Messung).ToList(),
            abbruchMessungen,
            [.. widerrufMessungen, .. widerrufOhneLoseMessungen]
        ];

        VorlageLaden("Zielerreichung.dotx", nameof(GetZielerreichungAsync));

        ZielerreichungTabellenFuellen(abschnitte, frist, soll);

        _gemBoxDocumentService.InsertValues(new Dictionary<string, string>
        {
            { "[%DatumVon%]", datumVon.ToString("dd.MM.yyyy") },
            { "[%DatumBis%]", datumBis.ToString("dd.MM.yyyy") },
            { "[%AnzahlTage%]", frist.FormatNumber() },
            { "[%Soll%]", FormatProzent(soll) }
        });

        return PdfSpeichern($"Zielerreichung_{datumVon:yyyyMMdd}_{datumBis:yyyyMMdd}.pdf");
    }

    /// <summary>
    /// Füllt die 4 Seiten (1 = Total, 2 = Bau, 3 = Logistik-zentral, 4 = Ämter-dezentral)
    /// mit je einer Tabelle pro Abschnitt und hinterlegt die Zielbeurteilung grün bzw. rot.
    /// </summary>
    private void ZielerreichungTabellenFuellen(List<ZielMessung>[] abschnitte, int frist, int soll)
    {
        Func<ZielMessung, bool>[] seitenFilter =
        [
            _ => true,
            m => m.Projektart == ProjektartBau,
            m => m.Projektart == ProjektartLogistik && m.Rolle == RolleInAusschreibungLogistikZentral,
            m => m.Projektart == ProjektartLogistik && m.Rolle == RolleInAusschreibungAemterDezentral
        ];

        for (var seite = 0; seite < seitenFilter.Length; seite++)
        {
            for (var abschnitt = 0; abschnitt < abschnitte.Length; abschnitt++)
            {
                var messungen = abschnitte[abschnitt].Where(seitenFilter[seite]).ToList();
                var tabelle = $"Tabelle{seite + 1}.{abschnitt}";
                var werte = ZielTabelle(messungen, frist, soll);
                TabelleFuellen(tabelle, 0, 3, werte, nameof(GetZielerreichungAsync));

                var beurteilung = werte[ZielBeurteilungZeile][ZielBeurteilungSpalte];
                if (beurteilung is "JA" or "NEIN")
                {
                    _gemBoxDocumentService.SetCellBackground(tabelle, ZielBeurteilungZeile, ZielBeurteilungSpalte,
                        beurteilung == "JA" ? ZielErfuelltFarbe : ZielNichtErfuelltFarbe);
                }
            }
        }
    }

    /// <summary>
    /// Zellenwerte eines Abschnitts (4 Zeilen à 5 Spalten): Anzahl, davon zu lange, davon
    /// Verschulden DöA (zu lange ohne Verzögerung durch Auftraggeber), davon fristgerecht.
    /// Der IST der letzten Zeile ist 100 % minus %-Verschulden (<see cref="ProzentVerschulden"/>),
    /// das Ziel ist erfüllt, wenn er das Soll erreicht. Ohne Treffer bleiben Prozente, Soll und Zielbeurteilung leer.
    /// </summary>
    private static List<string?[]> ZielTabelle(List<ZielMessung> messungen, int frist, int soll)
    {
        var anzahl = messungen.Count;
        // Fehlt ein Datum, ist keine Dauer feststellbar -> gilt als fristgerecht.
        var zuLange = messungen.Where(m => ArbeitstageHelper.PublikationsDauer(m.Eingereicht, m.Publiziert) > frist).ToList();
        var verschulden = zuLange.Count(m => !m.VerzoegerungDurchAuftraggeber);
        var fristgerecht = anzahl - zuLange.Count;
        var prozentVerschulden = ProzentVerschulden(verschulden, anzahl);
        // Auf die angezeigten zwei Stellen gerundet, damit die Zielbeurteilung zum ausgewiesenen IST passt.
        var ist = Math.Round(100 - prozentVerschulden, 2, MidpointRounding.AwayFromZero);

        string? Prozent(double wert) => anzahl > 0 ? FormatProzent(wert) : null;

        return
        [
            [null, anzahl.FormatNumber(), Prozent(100), null, null],
            [null, zuLange.Count.FormatNumber(), Prozent(anzahl > 0 ? 100d * zuLange.Count / anzahl : 0), null, null],
            [null, verschulden.FormatNumber(), Prozent(prozentVerschulden), null, null],
            [
                null, fristgerecht.FormatNumber(), Prozent(ist),
                // Ohne Treffer den [%Soll%]-Platzhalter der Zelle leeren, damit die Spalte leer bleibt.
                anzahl > 0 ? null : string.Empty,
                anzahl > 0 ? (ist >= soll ? "JA" : "NEIN") : null
            ]
        ];
    }

    /// <summary>%-Verschulden DöA eines Abschnitts, bezogen auf alle Publikationen des Abschnitts.</summary>
    private static double ProzentVerschulden(int verschulden, int anzahl) => anzahl > 0 ? 100d * verschulden / anzahl : 0;

    private async Task<int> GetKonfigurationIntAsync(DoeAContext context, string naturalId)
    {
        var wert = await context.TKonfiguration
            .Where(k => k.KonNaturalId == naturalId)
            .Select(k => k.KonWert)
            .FirstOrDefaultAsync();

        if (wert == null || !int.TryParse(wert, out var zahl))
        {
            _logger.Error(GeneralConstants.MessageTemplate, nameof(AuswertungService), nameof(GetKonfigurationIntAsync), $"Konfigurationswert '{naturalId}' fehlt oder ist keine Zahl");
            throw new InvalidOperationException(GeneralConstants.HandledException);
        }
        return zahl;
    }

    private async Task<string> MitDokumentLockAsync(Func<Task<string>> erzeugen)
    {
        await _dokumentLock.WaitAsync();
        try
        {
            return await erzeugen();
        }
        finally
        {
            _dokumentLock.Release();
        }
    }

    /// <summary>Lädt die Word-Vorlage; fehlt sie, wird der Bericht mit einer Fehlermeldung abgebrochen.</summary>
    private void VorlageLaden(string vorlage, string methode)
    {
        if (!_gemBoxDocumentService.Load($"Templates/{vorlage}"))
        {
            _logger.Error(GeneralConstants.MessageTemplate, nameof(AuswertungService), methode, $"Vorlage '{vorlage}' nicht gefunden");
            throw new InvalidOperationException(GeneralConstants.HandledException);
        }
    }

    /// <summary>Füllt eine Tabelle der Vorlage; fehlt sie, wird der Bericht abgebrochen.</summary>
    private void TabelleFuellen(string tabelle, int rowStart, int rowEnd, List<string?[]> werte, string methode)
    {
        if (!_gemBoxDocumentService.FillValuesInTable(tabelle, rowStart, rowEnd, werte))
        {
            _logger.Error(GeneralConstants.MessageTemplate, nameof(AuswertungService), methode, $"Tabelle '{tabelle}' nicht in der Vorlage gefunden");
            throw new InvalidOperationException(GeneralConstants.HandledException);
        }
    }

    /// <summary>Rendert das geladene Dokument als PDF und legt es im Download-Speicher ab.</summary>
    private string PdfSpeichern(string dateiname)
    {
        // Voll qualifiziert, weil ClosedXML ebenfalls einen Typ "SaveOptions" mitbringt.
        var stream = _gemBoxDocumentService.GetStream(GemBox.Document.SaveOptions.PdfDefault);
        return _storageService.StoreFile(stream.ToArray(), dateiname);
    }

    /// <summary>Legt die Arbeitsmappe im Download-Speicher ab.</summary>
    private async Task<string> ExcelSpeichernAsync(XLWorkbook workbook, string dateiname)
    {
        await using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return _storageService.StoreFile(ms.ToArray(), dateiname);
    }

    /// <summary>
    /// Schreibt eine fette Kopfzeile über der Tabelle. Label und Wert stehen in einer Zelle,
    /// damit der Text auch über schmalen Spalten vollständig sichtbar bleibt (er überläuft
    /// in die leeren Nachbarzellen).
    /// </summary>
    private static void ExcelTitelzeile(IXLWorksheet worksheet, int zeile, string text)
    {
        worksheet.Cell(zeile, 1).Value = text;
        worksheet.Cell(zeile, 1).Style.Font.Bold = true;
    }

    /// <summary>Schreibt die Spaltentitel fett auf grauem Hintergrund.</summary>
    private static void ExcelSpaltentitel(IXLWorksheet worksheet, int headerRow, string[] spalten)
    {
        for (var col = 0; col < spalten.Length; col++)
        {
            worksheet.Cell(headerRow, col + 1).Value = spalten[col];
        }

        var headerRange = worksheet.Range(headerRow, 1, headerRow, spalten.Length);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = ExcelTitelHintergrund;
    }

    /// <summary>
    /// Bricht die Spaltentitel um und passt die Spaltenbreiten an den Inhalt der
    /// Datenzeilen an. Mindestbreite ist das längste Wort des Titels, damit beim
    /// Umbruch keine Wörter getrennt werden.
    /// </summary>
    private static void TitelUmbrechenUndSpaltenbreitenAnpassen(IXLWorksheet worksheet, int headerRow, string[] spalten, int letzteZeile)
    {
        worksheet.Range(headerRow, 1, headerRow, spalten.Length).Style.Alignment.WrapText = true;
        if (letzteZeile > headerRow)
        {
            worksheet.Columns(1, spalten.Length).AdjustToContents(headerRow + 1, letzteZeile);
        }
        for (var col = 0; col < spalten.Length; col++)
        {
            // +2: Zellinnenabstand und fette Schrift des Titels
            var minBreite = spalten[col].Split(' ').Max(w => w.Length) + 2;
            var spalte = worksheet.Column(col + 1);
            if (spalte.Width < minBreite)
            {
                spalte.Width = minBreite;
            }
        }
    }

    /// <summary>Geschätzte Höhe einer Kopfzelle mit umbrochenem Text.</summary>
    private static double Zeilenhoehe(string text) =>
        Math.Ceiling((double)text.Length / ExcelZeichenProZeile) * ExcelZeilenhoehe;

    /// <summary>Setzt die Zeilenhöhe für umbrochenen Text; leerer Text lässt die Standardhöhe stehen.</summary>
    private static void ZeilenhoeheSetzen(IXLWorksheet worksheet, int zeile, string text)
    {
        var hoehe = Zeilenhoehe(text);
        if (hoehe > 0)
        {
            worksheet.Row(zeile).Height = hoehe;
        }
    }

    /// <summary>
    /// Effektive Zuschläge mit massgebendem Datum im Bereich datumVon/datumBis, eine Zeile pro Offerte: Der letzte
    /// Offertzustand (jüngstes Datum, dann zuletzt erfasst) ist "zuschlag". Massgebend ist dessen Datum (bei Zuständen
    /// mit Publikation das Publikationsdatum), bei Freihändern die Publikation der Ausschreibung. Verfahrensart filtern
    /// die Aufrufer; ob ein Zuschlag auf einem abgebrochenen Los zählt, entscheidet jede Auswertung selbst.
    /// </summary>
    private static IQueryable<EffektiverZuschlag> EffektiveZuschlaege(DoeAContext context, DateOnly datumVon, DateOnly datumBis) =>
        from ooz in context.TOfferteOffertezustand
        let aus = ooz.OozOff.OffLos.LosAus
        let massgebendesDatum = aus.AusVea.VeaNaturalId == VerfahrensartFreihändig ? aus.AusPublikation : (DateOnly?)ooz.OozDatum
        where ooz.OozOfz.OfzNaturalId == OffertezustandZuschlag &&
              ooz.OozId == context.TOfferteOffertezustand
                  .Where(o => o.OozOffId == ooz.OozOffId)
                  .OrderByDescending(o => o.OozDatum)
                  .ThenByDescending(o => o.OozInsertedDate)
                  .ThenByDescending(o => o.OozId)
                  .Select(o => o.OozId)
                  .First() &&
              massgebendesDatum >= datumVon && massgebendesDatum <= datumBis
        select new EffektiverZuschlag
        {
            Zustand = ooz,
            Offerte = ooz.OozOff,
            Los = ooz.OozOff.OffLos,
            Ausschreibung = aus,
            MassgebendesDatum = massgebendesDatum
        };

    /// <summary>
    /// Abbrüche mit massgebendem Datum im Bereich datumVon/datumBis, eine Zeile pro Los mit Abbruchdatum (dem Datum der
    /// Publikation des Abbruchs). Gemeinsame Zählmenge von 5-Jahresvergleich, Beschaffungscontrolling und Zielerreichung.
    /// </summary>
    private static IQueryable<AbbruchPublikation> AbbruchPublikationen(DoeAContext context, DateOnly datumVon, DateOnly datumBis) =>
        from los in context.TLos
        where los.LosAbbruchdatum != null &&
              VerfahrensartenAusschreibung.Contains(los.LosAus.AusVea.VeaNaturalId!) &&
              los.LosAbbruchdatum >= datumVon && los.LosAbbruchdatum <= datumBis
        select new AbbruchPublikation
        {
            Los = los,
            Ausschreibung = los.LosAus,
            MassgebendesDatum = los.LosAbbruchdatum!.Value
        };

    /// <summary>
    /// Widerrufe mit massgebendem Datum im Bereich datumVon/datumBis, eine Zeile pro Offertzustand "widerrufen", auch
    /// wenn später ein neuer Zuschlag folgt. Massgebend ist sein Datum, das Publikationsdatum des Widerrufs.
    /// Gemeinsame Zählmenge von 5-Jahresvergleich, Beschaffungscontrolling und Zielerreichung.
    /// </summary>
    private static IQueryable<ZustandPublikation> WiderrufPublikationen(DoeAContext context, DateOnly datumVon, DateOnly datumBis) =>
        from ooz in context.TOfferteOffertezustand
        let aus = ooz.OozOff.OffLos.LosAus
        let massgebendesDatum = ooz.OozDatum
        where ooz.OozOfz.OfzNaturalId == OffertezustandWiderrufen &&
              VerfahrensartenZuschlagWiderruf.Contains(aus.AusVea.VeaNaturalId!) &&
              massgebendesDatum >= datumVon && massgebendesDatum <= datumBis
        select new ZustandPublikation
        {
            Zustand = ooz,
            Ausschreibung = aus,
            MassgebendesDatum = massgebendesDatum
        };

    /// <summary>
    /// Zuschläge mit massgebendem Datum im Bereich datumVon/datumBis, eine Zeile pro Offertzustand "zuschlag" oder
    /// "zuschlagpraequali" (auch Freihänder und Zuschläge, die später widerrufen werden). Massgebend ist sein Datum,
    /// das Publikationsdatum des Zuschlags. Gemeinsame Zählmenge von Beschaffungscontrolling und Zielerreichung.
    /// </summary>
    private static IQueryable<ZustandPublikation> ZuschlagPublikationen(DoeAContext context, DateOnly datumVon, DateOnly datumBis) =>
        from ooz in context.TOfferteOffertezustand
        let aus = ooz.OozOff.OffLos.LosAus
        let massgebendesDatum = ooz.OozDatum
        where (ooz.OozOfz.OfzNaturalId == OffertezustandZuschlag || ooz.OozOfz.OfzNaturalId == OffertezustandZuschlagPraequali) &&
              VerfahrensartenZuschlagWiderruf.Contains(aus.AusVea.VeaNaturalId!) &&
              massgebendesDatum >= datumVon && massgebendesDatum <= datumBis
        select new ZustandPublikation
        {
            Zustand = ooz,
            Ausschreibung = aus,
            MassgebendesDatum = massgebendesDatum
        };

    /// <summary>Departement/Ämter der Selektion als Filterwerte und als Anzeigetext.</summary>
    private static AmtSelektion AmtSelektionErmitteln(Departement? dep, List<Amt>? aemter)
    {
        var depBezeichnung = dep?.Bezeichnung ?? AlleAnzeige;
        if (aemter is not { Count: > 0 })
        {
            return new AmtSelektion(dep?.Id, depBezeichnung, null, AlleAnzeige);
        }

        return new AmtSelektion(dep?.Id, depBezeichnung,
            aemter.Select(a => a.Id).ToList(),
            string.Join(", ", aemter.Select(a => a.Bezeichnung)));
    }

    /// <summary>NaturalIds der selektierten Verfahrensarten in der Reihenfolge der Berichtsköpfe.</summary>
    private static List<string> VerfahrensartenListe(bool offen, bool selektiv, bool freihaendig = false,
        bool einladung = false, bool vorankuendigung = false)
    {
        var verfahrensarten = new List<string>();
        if (offen)
        {
            verfahrensarten.Add(VerfahrensartOffen);
        }
        if (selektiv)
        {
            verfahrensarten.Add(VerfahrensartSelektiv);
        }
        if (freihaendig)
        {
            verfahrensarten.Add(VerfahrensartFreihändig);
        }
        if (einladung)
        {
            verfahrensarten.Add(VerfahrensartEinladung);
        }
        if (vorankuendigung)
        {
            verfahrensarten.AddRange(VerfahrensartenVorankündigung);
        }
        return verfahrensarten;
    }

    /// <summary>NaturalIds der selektierten Projektarten.</summary>
    private static List<string> ProjektartenListe(bool logistik, bool bau)
    {
        var projektarten = new List<string>();
        if (logistik)
        {
            projektarten.Add(ProjektartLogistik);
        }
        if (bau)
        {
            projektarten.Add(ProjektartBau);
        }
        return projektarten;
    }

    /// <summary>NaturalIds (für den Filter) und Bezeichnungen (für die Anzeige) der selektierten Rollen BBL.</summary>
    private static (List<string> NaturalIds, List<string> Anzeige) RollenListe(bool aemterDezentral, bool logistikZentral)
    {
        var naturalIds = new List<string>();
        var anzeige = new List<string>();
        if (aemterDezentral)
        {
            naturalIds.Add(RolleInAusschreibungAemterDezentral);
            anzeige.Add(RolleBezeichnungAemterDezentral);
        }
        if (logistikZentral)
        {
            naturalIds.Add(RolleInAusschreibungLogistikZentral);
            anzeige.Add(RolleBezeichnungLogistikZentral);
        }
        return (naturalIds, anzeige);
    }

    /// <summary>Zerlegt eine kommaseparierte Eingabe (CPV-/BKP-Nummern) in die einzelnen Nummern.</summary>
    private static List<string> NummernListe(string eingabe) =>
        eingabe.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(n => n.Trim()).ToList();

    /// <summary>Selektionskriterien als Anzeigetext, "Alle" wenn nichts eingeschränkt wurde.</summary>
    private static string AnzeigeOderAlle(List<string> werte) =>
        werte.Count > 0 ? string.Join(", ", werte) : AlleAnzeige;

    private static int Quartal(DateOnly datum) => (datum.Month - 1) / 3 + 1;

    private static string FormatProzent(double wert) => wert.ToString("0.00", CultureInfo.InvariantCulture) + "%";

    private static string ProzentEinstellig(int anzahl, int basis) =>
        (basis > 0 ? 100d * anzahl / basis : 0).ToString("0.0", CultureInfo.InvariantCulture) + "%";

    /// <summary>Departement/Ämter der Selektion: Filterwerte (null = keine Einschränkung) und Anzeigetexte.</summary>
    private sealed record AmtSelektion(Guid? DepId, string DepBezeichnung, List<Guid>? AmtIds, string AmtBezeichnungen);

    /// <summary>
    /// Ein effektiver Zuschlag aus <see cref="EffektiveZuschlaege"/>. Die Entities bleiben in der Abfrage: Filter und
    /// Spalten der Aufrufer greifen über ihre Navigationen zu (z.B. Anbieter → Ortschaft → Kanton, Ausschreibung → Amt).
    /// </summary>
    private sealed class EffektiverZuschlag
    {
        public TOfferteOffertezustand Zustand { get; init; } = null!;
        public TOfferte Offerte { get; init; } = null!;
        public TLos Los { get; init; } = null!;
        public TAusschreibung Ausschreibung { get; init; } = null!;
        public DateOnly? MassgebendesDatum { get; init; }
    }

    /// <summary>Ein Abbruch aus <see cref="AbbruchPublikationen"/>; die Entities bleiben für die Aufrufer in der Abfrage.</summary>
    private sealed class AbbruchPublikation
    {
        public TLos Los { get; init; } = null!;
        public TAusschreibung Ausschreibung { get; init; } = null!;
        public DateOnly MassgebendesDatum { get; init; }
    }

    /// <summary>
    /// Ein Offertzustand aus <see cref="WiderrufPublikationen"/> bzw. <see cref="ZuschlagPublikationen"/>; die Entities
    /// bleiben für die Aufrufer in der Abfrage.
    /// </summary>
    private sealed class ZustandPublikation
    {
        public TOfferteOffertezustand Zustand { get; init; } = null!;
        public TAusschreibung Ausschreibung { get; init; } = null!;
        public DateOnly MassgebendesDatum { get; init; }
    }

    /// <summary>Die im Kopf des Zuschläge-Berichts ausgewiesenen Selektionskriterien.</summary>
    private sealed record ZuschlaegeFilterTexte(string Verfahrensarten, string Projektarten, string Rollen,
        string CpvNummern, string BkpNummern, string Departement, string Aemter)
    {
        /// <summary>CPV-Nummern oder "Alle", wenn nicht eingeschränkt.</summary>
        public string CpvAnzeige => CpvNummern.Length > 0 ? CpvNummern : AlleAnzeige;

        /// <summary>BKP-Nummern oder "Alle", wenn nicht eingeschränkt.</summary>
        public string BkpAnzeige => BkpNummern.Length > 0 ? BkpNummern : AlleAnzeige;
    }

    /// <summary>Die im Kopf des Ausschreibungen-Berichts ausgewiesenen Selektionskriterien.</summary>
    private sealed record AusschreibungenFilterTexte(string Leistungsarten, string Projektarten,
        string Verfahrensarten, string Rollen, string Departement, string Aemter);

    /// <summary>Anzahl Ausschreibungen eines Jahres, verteilt auf die vier Quartale.</summary>
    private sealed record JahrQuartale(int Jahr, int Q1, int Q2, int Q3, int Q4)
    {
        /// <summary>Anzahl über alle vier Quartale.</summary>
        public int Total => Q1 + Q2 + Q3 + Q4;
    }

    /// <summary>Anteil der Ausschreibungen einer Sprache je Dokumentart in Prozent.</summary>
    private sealed record SprachAnteil(string Bezeichnung, int PublikationProzent, int UnterlagenProzent, int OfferteProzent);

    /// <summary>
    /// Ein Ereignis für das Beschaffungscontrolling: die Verfahrensart der zugehörigen
    /// Ausschreibung, das massgebende Datum (bestimmt das Quartal), Projektart/Rolle für
    /// die Spaltengruppen sowie bei Zuschlägen der Zustand (zuschlag/zuschlagpraequali).
    /// </summary>
    private sealed record ControllingEreignis(string? Verfahrensart, DateOnly Datum, string? Projektart, string? Rolle, string? Zustand = null);

    /// <summary>
    /// Eine Messung für den Zielerreichungs-Bericht: ein publiziertes Ereignis (Ausschreibung,
    /// Berichtigung, Zuschlag/Widerruf pro Offertenzustand, Abbruch pro Los) mit dem
    /// Datumspaar, aus dem die Bearbeitungsdauer gerechnet wird. Projektart/Rolle stammen von
    /// der zugehörigen Ausschreibung (für die Seitenfilter).
    /// </summary>
    private sealed record ZielMessung(
        DateOnly? Eingereicht,
        DateOnly? Publiziert,
        bool VerzoegerungDurchAuftraggeber,
        string? Projektart,
        string? Rolle);

    /// <summary>Basisdaten einer Ausschreibung mit Publikation im Selektionsbereich.</summary>
    private sealed class ZielBasis
    {
        public DateOnly? Eingereicht { get; init; }
        public DateOnly? Publikation { get; init; }
        public bool VerzoegerungDurchAuftraggeber { get; init; }
        public string? Projektart { get; init; }
        public string? Verfahrensart { get; init; }
        public string? Rolle { get; init; }
    }
}
