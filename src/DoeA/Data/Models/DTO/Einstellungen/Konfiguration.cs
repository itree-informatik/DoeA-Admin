// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using ItreeMud.Models;

namespace DoeA.Data.Models.DTO.Einstellungen;

/// <summary>Konfigurationseintrag mit typisiertem Wert (Typ steuert den Editor der KonfigurationSeite) — abgebildet auf dbo.t_konfiguration (Präfix "Kon").</summary>
public class Konfiguration : Basis
{
    public string Bezeichnung { get; set; } = string.Empty;
    public string Wert { get; set; } = string.Empty;
    public string Typ { get; set; } = string.Empty;
    public string Beschreibung { get; set; } = string.Empty;

    public bool WertBool { get; set; }
    public int WertInt { get; set; }
    public string? WertDate { get; set; }
    public decimal WertDecimal { get; set; }
    public bool WertIsInt => Typ == "int";
    public bool WertIsBool => Typ == "bool";
    public bool WertIsDate => Typ == "date";
    public bool WertIsDecimal => Typ == "decimal";
    public bool WertIsJson => Typ == "json";
}
