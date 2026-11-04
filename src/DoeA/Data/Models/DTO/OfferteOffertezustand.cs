// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using ItreeMud.Models;

namespace DoeA.Data.Models.DTO;

/// <summary>
/// Zustandseintrag in der Historie einer Offerte (Zustand + Datum) — abgebildet auf dbo.t_offerteoffertezustand (Präfix "Ooz").
/// Bei Zuständen mit Publikation (Offertezustand.Publikation) ist Datum das Publikationsdatum.
/// </summary>
public class OfferteOffertezustand : Basis
{
    public string Bezeichnung { get; set; } = string.Empty;
    public Guid OffId { get; set; }
    public Guid OfzId { get; set; }
    public DateTime? Datum { get; set; }
    public DateOnly? Eingereicht { get; set; }
    public bool VerzoegerungDurchAuftraggeber { get; set; }
}
