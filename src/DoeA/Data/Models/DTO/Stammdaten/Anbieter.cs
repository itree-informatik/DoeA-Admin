// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using ItreeMud.Models;

namespace DoeA.Data.Models.DTO.Stammdaten;

/// <summary>Anbieter (Firma mit Adresse und Ortschaft), der Offerten einreicht — abgebildet auf dbo.t_anbieter (Präfix "Anb").</summary>
public class Anbieter : Basis
{
    public string Name { get; set; } = string.Empty;
    public string Adresse { get; set; } = string.Empty;
    public string? Adresse2 { get; set; }
    public string? Adresse3 { get; set; }
    public Guid OrtId { get; set; }

    public Ortschaft? Ortschaft { get; set; }
}
