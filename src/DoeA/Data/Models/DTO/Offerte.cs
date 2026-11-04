// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DTO.Stammdaten;
using ItreeMud.Models;

namespace DoeA.Data.Models.DTO;

/// <summary>Offerte eines Anbieters zu einem Los, mit Zustandshistorie — abgebildet auf dbo.t_offerte (Präfix "Off").</summary>
public class Offerte : Basis
{
    public Guid? AnbId { get; set; }
    public Guid LosId { get; set; }
    public Anbieter Anbieter { get; set; } = default!;
    public decimal ZuschlagswertChf { get; set; }
    public int? AnzahlMitarbeiter { get; set; }
    public string Bemerkung { get; set; } = string.Empty;
    public List<OfferteOffertezustand> Offertenzustaende { get; set; } = new();
}
