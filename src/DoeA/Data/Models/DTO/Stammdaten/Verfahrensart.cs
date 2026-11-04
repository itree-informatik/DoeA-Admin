// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using ItreeMud.Models;

namespace DoeA.Data.Models.DTO.Stammdaten;

/// <summary>Verfahrensart (offen, selektiv, freihändig, …) — abgebildet auf dbo.t_verfahrensart (Präfix "Vea").</summary>
public class Verfahrensart
    : Basis
{
    public string Bezeichnung { get; set; } = string.Empty;
    public string Kurzbezeichnung { get; set; } = string.Empty;
    public int Sortierung { get; set; }
}