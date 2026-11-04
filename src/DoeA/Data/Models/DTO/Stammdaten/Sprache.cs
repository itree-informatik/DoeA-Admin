// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using ItreeMud.Models;

namespace DoeA.Data.Models.DTO.Stammdaten;

/// <summary>Publikationssprache mit Culture-Code (z.B. «de-CH») — abgebildet auf dbo.t_sprache (Präfix "Spr").</summary>
public class Sprache : Basis
{
    public string Culture { get; set; } = string.Empty;
    public string Bezeichnung { get; set; } = string.Empty;

    /// <summary>Anzeige-Reihenfolge; <c>BaseService</c> sortiert automatisch danach.</summary>
    public int Sortierung { get; set; }
}