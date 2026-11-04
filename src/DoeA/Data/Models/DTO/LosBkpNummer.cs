// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DTO.Stammdaten;
using ItreeMud.Models;

namespace DoeA.Data.Models.DTO;

/// <summary>Zuordnung Los ↔ BKP-Nummer (n:m) — abgebildet auf dbo.t_losbkpnummer (Präfix "Lbn").</summary>
public class LosBkpNummer : Basis
{
    public Guid BkpId { get; set; }
    public BkpNummer BkpNummer { get; set; } = new();
    public Guid LosId { get; set; }
}
