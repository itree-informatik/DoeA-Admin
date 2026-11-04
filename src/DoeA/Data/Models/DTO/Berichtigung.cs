// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using ItreeMud.Models;

namespace DoeA.Data.Models.DTO;

/// <summary>Berichtigung einer Ausschreibung (1:n) — abgebildet auf dbo.t_berichtigung (Präfix "Ber").</summary>
public class Berichtigung : Basis
{
    public Guid AusId { get; set; }
    public DateOnly? Eingereicht { get; set; }
    public DateOnly? Publiziert { get; set; }
    public bool VerzoegerungDurchAuftraggeber { get; set; }
    public string? Bemerkungen { get; set; }
}
