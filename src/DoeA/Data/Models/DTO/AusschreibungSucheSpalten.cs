// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

namespace DoeA.Data.Models.DTO;

/// <summary>
/// Katalog der Ergebnisspalten der Ausschreibungs-Suche — Single Source of Truth für Reihenfolge,
/// Grid-Titel, Popup-Beschriftung und Default-Sichtbarkeit. Die Sichtbarkeit wird nicht in der DB
/// gespeichert, sondern pro Circuit im <c>AusschreibungSucheState</c> gehalten.
/// </summary>
public static class AusschreibungSucheSpalten
{
    /// <summary>Alle Spalten in Anzeigereihenfolge; Default = Standardansicht der Übersicht (Art, Jahr, Nummer, Simap-Nr, Dienststellennummer, Bezeichnung, Rolle, Sachbearbeiter, Verfahrensart, Publiziert), alles Weitere ist zuschaltbar.</summary>
    public static readonly IReadOnlyList<SpaltenDefinition> AlleSpalten = new List<SpaltenDefinition>
    {
        new(nameof(AusschreibungSuche.Art), "AUSSCHREIBUNGEN_ART", true),
        new(nameof(AusschreibungSuche.Jahr), "AUSSCHREIBUNGEN_JAHR", true),
        new(nameof(AusschreibungSuche.NummerStr), "AUSSCHREIBUNGEN_NUMMER", true),
        new(nameof(AusschreibungSuche.SimapNr), "AUSSCHREIBUNGEN_SIMAPNR", true),
        new(nameof(AusschreibungSuche.Dienststellennummer), "AUSSCHREIBUNGEN_DIENSTSTELLENNUMMER", true),
        new(nameof(AusschreibungSuche.Bezeichnung), "AUSSCHREIBUNGEN_BEZEICHNUNG", true),
        new(nameof(AusschreibungSuche.Rolle), "AUSSCHREIBUNGEN_ROLLE", true),
        new(nameof(AusschreibungSuche.Sachbearbeiter), "AUSSCHREIBUNGEN_SACHBEARBEITER", true),
        new(nameof(AusschreibungSuche.Rechtsdienst), "AUSSCHREIBUNGEN_SACHBEARBEITER_RD", false),
        new(nameof(AusschreibungSuche.Bbl), "AUSSCHREIBUNGEN_SACHBEARBEITER_BBL", false),
        new(nameof(AusschreibungSuche.Extern), "AUSSCHREIBUNGEN_EXTERN", false),
        new(nameof(AusschreibungSuche.Verfahrensart), "AUSSCHREIBUNGEN_VERFAHRENSART", true),
        new(nameof(AusschreibungSuche.Leistungsart), "AUSSCHREIBUNGEN_LEISTUNGSART", false),
        new(nameof(AusschreibungSuche.Personalverleih), "AUSSCHREIBUNGEN_PERSONALVERLEIH", false),
        new(nameof(AusschreibungSuche.Publiziert), "AUSSCHREIBUNGEN_PUBLIZIERT", true),
        new(nameof(AusschreibungSuche.Abgeschlossen), "AUSSCHREIBUNGEN_ABGESCHLOSSEN", false),
        new(nameof(AusschreibungSuche.Abgebrochen), "AUSSCHREIBUNGEN_ABGEBROCHEN", false),
        new(nameof(AusschreibungSuche.Widerrufen), "AUSSCHREIBUNGEN_WIDERRUFEN", false),
        new(nameof(AusschreibungSuche.RfiNummer), "AUSSCHREIBUNGEN_RFI", false),
        new(nameof(AusschreibungSuche.VorankuendigungNummer), "AUSSCHREIBUNGEN_VORANKUENDIGUNG", false),
    };

    /// <summary>Property-Namen der standardmässig sichtbaren Spalten (neue Set-Instanz).</summary>
    public static HashSet<string> DefaultSpalten()
    {
        return AlleSpalten.Where(s => s.Default).Select(s => s.Property).ToHashSet();
    }

    /// <summary>Property-Namen des kompakten Spaltensatzes der Auswahlfenster Hauptprojekt/Vorankündigung/RFI (neue Set-Instanz).</summary>
    public static HashSet<string> AuswahlfensterSpalten()
    {
        return
        [
            nameof(AusschreibungSuche.Jahr),
            nameof(AusschreibungSuche.NummerStr),
            nameof(AusschreibungSuche.SimapNr),
            nameof(AusschreibungSuche.Bezeichnung),
            nameof(AusschreibungSuche.Rolle),
            nameof(AusschreibungSuche.Sachbearbeiter),
            nameof(AusschreibungSuche.Verfahrensart),
        ];
    }
}
