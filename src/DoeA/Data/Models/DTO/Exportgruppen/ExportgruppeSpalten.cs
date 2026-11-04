// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

namespace DoeA.Data.Models.DTO.Exportgruppen;

/// <summary>
/// Katalog der Ergebnisspalten einer Exportgruppe — Single Source of Truth für Reihenfolge,
/// Grid-Titel, Popup-Beschriftung, Excel-Spaltenliste und Default-Sichtbarkeit.
/// Die Sichtbarkeit selbst wird pro Exportgruppe gespeichert (<see cref="Exportgruppe.Spalten"/>,
/// kommaseparierte Property-Namen; null = Default-Spaltensatz).
/// </summary>
public static class ExportgruppeSpalten
{
    /// <summary>Alle Spalten in Anzeigereihenfolge; Default = kompakter Spaltensatz ohne horizontales Scrollen, alles Weitere ist zuschaltbar.</summary>
    public static readonly IReadOnlyList<SpaltenDefinition> AlleSpalten = new List<SpaltenDefinition>
    {
        new(nameof(Ergebnis.Jahr), "EXPORTGRUPPE_ERGEBNIS_JAHR", true),
        new(nameof(Ergebnis.NummerStr), "EXPORTGRUPPE_ERGEBNIS_NUMMERSTR", true),
        new(nameof(Ergebnis.Bezeichnung), "EXPORTGRUPPE_ERGEBNIS_BEZEICHNUNG", true),
        new(nameof(Ergebnis.SimapNr), "EXPORTGRUPPE_ERGEBNIS_SIMAPNR", false),
        new(nameof(Ergebnis.Amt), "EXPORTGRUPPE_ERGEBNIS_AMT", true),
        new(nameof(Ergebnis.Ressort), "EXPORTGRUPPE_ERGEBNIS_RESSORT", false),
        new(nameof(Ergebnis.Leistungsart), "EXPORTGRUPPE_ERGEBNIS_LEISTUNGSART", false),
        new(nameof(Ergebnis.Projektart), "EXPORTGRUPPE_ERGEBNIS_PROJEKTART", true),
        new(nameof(Ergebnis.Verfahrensart), "EXPORTGRUPPE_ERGEBNIS_VERFAHRENSART", true),
        new(nameof(Ergebnis.RolleInAusschreibung), "EXPORTGRUPPE_ERGEBNIS_ROLLEINAUSSCHREIBUNG", false),
        new(nameof(Ergebnis.Kanton), "EXPORTGRUPPE_ERGEBNIS_KANTON", false),
        new(nameof(Ergebnis.SachbearbeiterDoeA), "EXPORTGRUPPE_ERGEBNIS_SACHBEARBEITERDOEA", false),
        new(nameof(Ergebnis.SachbearbeiterRd), "EXPORTGRUPPE_ERGEBNIS_SACHBEARBEITERRD", false),
        new(nameof(Ergebnis.SachbearbeiterBbl), "EXPORTGRUPPE_ERGEBNIS_SACHBEARBEITERBBL", false),
        new(nameof(Ergebnis.Publikation), "EXPORTGRUPPE_ERGEBNIS_PUBLIKATION", true),
        new(nameof(Ergebnis.OffertenFrist), "EXPORTGRUPPE_ERGEBNIS_OFFERTENFRIST", false),
        new(nameof(Ergebnis.Abgeschlossen), "EXPORTGRUPPE_ERGEBNIS_ABGESCHLOSSEN", false),
        new(nameof(Ergebnis.Extern), "EXPORTGRUPPE_ERGEBNIS_EXTERN", false),
        new(nameof(Ergebnis.Personalverleih), "EXPORTGRUPPE_ERGEBNIS_PERSONALVERLEIH", false),
        new(nameof(Ergebnis.BeschaffungDezentral), "EXPORTGRUPPE_ERGEBNIS_BESCHAFFUNGDEZENTRAL", false),
    };

    /// <summary>Liest die gespeicherte Sichtbarkeit; null = Default-Spaltensatz, unbekannte Namen werden ignoriert.</summary>
    public static HashSet<string> AusCsv(string? csv)
    {
        if (csv == null)
        {
            return AlleSpalten.Where(s => s.Default).Select(s => s.Property).ToHashSet();
        }

        var namen = csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToHashSet();
        return AlleSpalten.Select(s => s.Property).Where(namen.Contains).ToHashSet();
    }

    /// <summary>Serialisiert die sichtbaren Spalten in Katalogreihenfolge für die Speicherung.</summary>
    public static string AlsCsv(IReadOnlySet<string> sichtbare)
    {
        return string.Join(",", AlleSpalten.Select(s => s.Property).Where(sichtbare.Contains));
    }
}
