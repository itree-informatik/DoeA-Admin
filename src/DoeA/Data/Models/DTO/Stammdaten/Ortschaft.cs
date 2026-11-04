// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using ItreeMud.Models;

namespace DoeA.Data.Models.DTO.Stammdaten;

/// <summary>Ortschaft (PLZ/Ort) mit Land und optionalem Kanton — abgebildet auf dbo.t_ortschaft (Präfix "Ort").</summary>
public class Ortschaft : Basis
{
    public string Ort { get; set; } = string.Empty;
    public string Plz { get; set; } = string.Empty;
    public Guid LanId { get; set; }
    public Guid? KanId { get; set; }

    public Kanton? Kanton { get; set; }
    public Land? Land { get; set; }
}