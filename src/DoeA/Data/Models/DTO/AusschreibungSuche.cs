// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using ItreeMud.Models;

namespace DoeA.Data.Models.DTO;

/// <summary>Zeile der Ausschreibungs-Suchliste — rein lesende Projektion über mehrere Tabellen, befüllt vom AusschreibungService.</summary>
public class AusschreibungSuche : Basis
{
    public string Art { get; set; } = string.Empty;
    public string Bezeichnung { get; set; } = string.Empty;
    public int Jahr { get; set; }
    public int Nummer { get; set; }
    public string NummerStr { get; set; } = string.Empty;
    public string Dienststellennummer { get; set; } = string.Empty;
    public string Rolle { get; set; } = string.Empty;
    public string Sachbearbeiter { get; set; } = string.Empty;
    public string Verfahrensart { get; set; } = string.Empty;
    public string Leistungsart { get; set; } = string.Empty;
    public bool Personalverleih { get; set; }
    public DateOnly? Publiziert { get; set; }
    public string? PubliziertJahr => Publiziert?.Year.ToString();
    public string? PubliziertMonat => Publiziert?.ToString("MM");
    public string? PubliziertTag => Publiziert?.ToString("dd");
    public bool Abgeschlossen { get; set; }
    public bool Abgebrochen { get; set; }
    public bool Widerrufen { get; set; }
    public string Ressort { get; internal set; } = string.Empty;
    public string? SimapNr { get; internal set; }
    public int? Abschlussjahr { get; internal set; }
    public string? Standort { get; internal set; }
    public bool? BeschaffungDezentral { get; internal set; }
    public string Rechtsdienst { get; internal set; } = string.Empty;
    public string Bbl { get; internal set; } = string.Empty;
    public string? Extern { get; internal set; }
    public DateOnly? EingabefristOfferte { get; internal set; }
    public string? VorankuendigungNummer { get; internal set; }
    public string? RfiNummer { get; internal set; }
}
