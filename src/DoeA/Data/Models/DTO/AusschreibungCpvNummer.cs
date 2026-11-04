// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DTO.Stammdaten;
using ItreeMud.Models;

namespace DoeA.Data.Models.DTO;

/// <summary>Zuordnung Ausschreibung ↔ CPV-Nummer (n:m) — abgebildet auf dbo.t_ausschreibungcpvnummer (Präfix "Acn").</summary>
public class AusschreibungCpvNummer : Basis
{
    public Guid CpvId { get; set; }
    public CpvNummer CpvNummer { get; set; } = new();
    public Guid AusId { get; set; }
}
