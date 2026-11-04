// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using ItreeMud.Models;

namespace DoeA.Data.Models.DTO.Stammdaten;

/// <summary>Leistungsart, einer Projektart zugeordnet — abgebildet auf dbo.t_leistungsart (Präfix "Lea").</summary>
public class Leistungsart : Basis
{
    public string Bezeichnung { get; set; } = string.Empty;
    public string Kurzbezeichnung { get; set; } = string.Empty;
    public Guid PraId { get; set; }

    public Projektart? Projektart { get; set; }
}