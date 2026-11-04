// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using ItreeMud.Models;

namespace DoeA.Data.Models.DTO.Stammdaten;

/// <summary>CPV-Nummer (Common Procurement Vocabulary) — abgebildet auf dbo.t_cpvnummer (Präfix "Cpv").</summary>
public class CpvNummer : Basis
{
    public string NummerStr { get; set; } = string.Empty;
    public string Bezeichnung { get; set; } = string.Empty;
    public int Nummer { get; set; }
}