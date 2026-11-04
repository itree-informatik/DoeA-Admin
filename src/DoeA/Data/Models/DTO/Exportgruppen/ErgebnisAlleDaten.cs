// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.Reflection;
using ItreeMud.Models;

namespace DoeA.Data.Models.DTO.Exportgruppen;

/// <summary>Eine Berichtigung für die dynamischen Berichtigungs-Spalten des Exports.</summary>
public record BerichtigungExport(DateOnly? Eingereicht, DateOnly? Publiziert, bool VerzoegerungDurchAuftraggeber, string? Bemerkungen)
{
    public int? AnzahlTage => ArbeitstageHelper.PublikationsDauer(Eingereicht, Publiziert);
}

/// <summary>Exportzeile «Alle Daten» einer Exportgruppe, eine Zeile pro Offertzustand (Left Joins Ausschreibung → Los → Offerte → Offertzustand → Anbieter); der Klassenname ist Teil der Übersetzungsschlüssel EXPORTGRUPPE_ERGEBNISALLEDATEN_{PROPERTY}.</summary>
public class ErgebnisAlleDaten : Basis
{
    // Schlüssel für die nachgelagerte Befüllung der Aggregat-Spalten (CPV/BKP/Sprachmatrix/
    // Offerten-Zählung) und des aktuellen Offertzustands — internal, damit sie nicht als Exportspalten erscheinen.
    internal Guid AusId { get; set; }
    internal Guid? LosId { get; set; }
    internal Guid? OffId { get; set; }
    internal long? OozInsertedDate { get; set; }
    internal Guid? OozId { get; set; }

    /// <summary>
    /// Alle Berichtigungen der Ausschreibung in Erfassungsreihenfolge — werden im Export als
    /// dynamische Spalten («Berichtigung 1, 2, …») zuhinterst angehängt (internal, damit die
    /// Liste nicht selbst als Exportspalte erscheint).
    /// </summary>
    internal List<BerichtigungExport> Berichtigungen { get; set; } = [];

    public int Jahr { get; set; }
    public string NummerStr { get; set; } = string.Empty;
    public string Bezeichnung { get; set; } = string.Empty;
    public string? SimapNr { get; set; }
    public string? Dienststellennummer { get; set; }
    public string? Amt { get; set; }
    public string? Ressort { get; set; }
    public string? Leistungsart { get; set; }
    public string Projektart { get; set; } = string.Empty;
    public string Verfahrensart { get; set; } = string.Empty;
    public string? RolleInAusschreibung { get; set; }
    /// <summary>Nummer der übergeordneten Ausschreibung (AusAusId).</summary>
    public string? Hauptprojekt { get; set; }
    /// <summary>Nummer der referenzierten Vorankündigung.</summary>
    public string? Vorankuendigung { get; set; }
    /// <summary>Nummer der referenzierten RFI.</summary>
    public string? Rfi { get; set; }
    /// <summary>Alle CPV-Nummern der Ausschreibung, konkateniert.</summary>
    public string? CpvNummern { get; set; }
    public string? SachbearbeiterDoeA { get; set; }
    public string? SachbearbeiterRd { get; set; }
    public string? SachbearbeiterBbl { get; set; }
    public string? Extern { get; set; }
    public bool Personalverleih { get; set; }
    public bool BeschaffungDezentral { get; set; }
    public DateOnly? Eingereicht { get; set; }

    // Sprachmatrix (Sprache × Dokumentart): «X» wenn vorhanden, sonst leer.
    public string? PublikationDe { get; set; }
    public string? PublikationFr { get; set; }
    public string? PublikationIt { get; set; }
    public string? PublikationEn { get; set; }
    public string? UnterlagenDe { get; set; }
    public string? UnterlagenFr { get; set; }
    public string? UnterlagenIt { get; set; }
    public string? UnterlagenEn { get; set; }
    public string? OffertenDe { get; set; }
    public string? OffertenFr { get; set; }
    public string? OffertenIt { get; set; }
    public string? OffertenEn { get; set; }

    public DateOnly? Publikation { get; set; }
    public bool VerzoegerungDurchAuftraggeber { get; set; }
    // Anzahl Tage bis zur Publikation wie in der Maske (nicht nach SQL übersetzbar, deshalb berechnet).
    [ExportAlsZahl]
    public int? AnzahlTage => ArbeitstageHelper.PublikationsDauer(Eingereicht, Publikation);
    // Widerruf einer RFI oder Vorankündigung (ohne Lose, deshalb in der Ausschreibung).
    public DateOnly? EingereichtWiderruf { get; set; }
    public DateOnly? PublikationWiderruf { get; set; }
    public bool VerzoegerungDurchAuftraggeberWiderruf { get; set; }
    [ExportAlsZahl]
    public int? AnzahlTageWiderruf => ArbeitstageHelper.PublikationsDauer(EingereichtWiderruf, PublikationWiderruf);
    public DateOnly? EingabefristTeilnahme { get; set; }
    public int? PflichtenhefterPraequali { get; set; }
    public int? AnzahlTeilnehmer { get; set; }
    public DateOnly? VersandPh2 { get; set; }
    public int? Pflichtenhefter { get; set; }
    public DateOnly? OffertenFrist { get; set; }
    public int? AbschlussJahr { get; set; }
    public bool Abgeschlossen { get; set; }
    public string? Standort { get; set; }
    public string? Bemerkung { get; set; }

    public string? LosBezeichnung { get; set; }
    public string? LosZustand { get; set; }
    public DateOnly? LosEingereicht { get; set; }
    public DateOnly? LosAbbruchdatum { get; set; }
    public bool? LosVerzoegerungDurchAuftraggeber { get; set; }
    [ExportAlsZahl]
    public int? LosAnzahlTage => ArbeitstageHelper.PublikationsDauer(LosEingereicht, LosAbbruchdatum);
    /// <summary>Alle BKP-Nummern des Loses, konkateniert.</summary>
    public string? BkpNummern { get; set; }

    public decimal? OfferteZuschlagswertChf { get; set; }
    [ExportAlsZahl]
    public int? OfferteAnzahlMitarbeitende { get; set; }
    public string? OfferteBemerkung { get; set; }
    /// <summary>Offertzustand dieser Zeile.</summary>
    public string? OfferteZustand { get; set; }
    public DateOnly? OfferteZustandDatum { get; set; }
    public DateOnly? OfferteZustandEingereicht { get; set; }
    public bool? OfferteZustandVerzoegerungDurchAuftraggeber { get; set; }
    // Bei Zuständen mit Publikation ist das Zustandsdatum die Publikation; ohne Publikation fehlt «Eingereicht».
    [ExportAlsZahl]
    public int? OfferteZustandAnzahlTage => ArbeitstageHelper.PublikationsDauer(OfferteZustandEingereicht, OfferteZustandDatum);
    public bool? OfferteZustandAktuell { get; set; }

    public string? AnbieterName { get; set; }
    public string? AnbieterAdresse { get; set; }
    public string? AnbieterAdresse2 { get; set; }
    public string? AnbieterAdresse3 { get; set; }
    public string? AnbieterPlz { get; set; }
    public string? AnbieterOrt { get; set; }
    public string? AnbieterKanton { get; set; }
    public string? AnbieterLand { get; set; }
    /// <summary>Sprache des Anbieter-Kantons.</summary>
    public string? AnbieterKantonSprache { get; set; }
    public bool? AnbieterAktiv { get; set; }

    // Offerten des Loses nach Sprachregion des Anbieter-Kantons bzw. Ausland —
    // pro Zeile desselben Loses bewusst redundant.
    public int? AnzahlOffertenDeutsch { get; set; }
    public int? AnzahlOffertenFranzoesisch { get; set; }
    public int? AnzahlOffertenItalienisch { get; set; }
    public int? AnzahlOffertenAusland { get; set; }

    /// <summary>
    /// Spalten (und Reihenfolge) des Excel-Exports: die hier deklarierten Properties in
    /// Deklarationsreihenfolge plus das geerbte Aktiv (fachlich das Aktiv-Flag der Ausschreibung,
    /// vor den Los-Spalten einsortiert) — die übrigen Basis-Metadaten bleiben aussen vor.
    /// </summary>
    public static List<string> ExportSpalten()
    {
        var spalten = typeof(ErgebnisAlleDaten)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(p => p.Name)
            .ToList();
        return spalten;
    }
}
