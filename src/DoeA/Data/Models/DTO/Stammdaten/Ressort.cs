// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using ItreeMud.Models;

namespace DoeA.Data.Models.DTO.Stammdaten;

/// <summary>Ressort eines Amts — abgebildet auf dbo.t_ressort (Präfix "Res").</summary>
public class Ressort : Basis
{
    public string Bezeichnung { get; set; } = string.Empty;
    public string Kurzbezeichnung { get; set; } = string.Empty;

    public Guid AmtId { get; set; }
    public Amt? Amt { get; set; }
}