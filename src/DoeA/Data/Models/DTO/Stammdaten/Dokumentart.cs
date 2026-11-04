// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using ItreeMud.Models;

namespace DoeA.Data.Models.DTO.Stammdaten;

/// <summary>Dokumentart einer Ausschreibung — abgebildet auf dbo.t_dokumentart (Präfix "Doa").</summary>
public class Dokumentart : Basis
{
    public string Bezeichnung { get; set; } = string.Empty;

    /// <summary>Anzeige-Reihenfolge; <c>BaseService</c> sortiert automatisch danach.</summary>
    public int Sortierung { get; set; }
}