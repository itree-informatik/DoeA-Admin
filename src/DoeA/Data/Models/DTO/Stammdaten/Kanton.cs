// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using ItreeMud.Models;

namespace DoeA.Data.Models.DTO.Stammdaten;

/// <summary>Kanton mit zugeordneter Sprache — abgebildet auf dbo.t_kanton (Präfix "Kan").</summary>
public class Kanton : Basis
{
    public string Bezeichnung { get; set; } = string.Empty;
    public string Kurzbezeichnung { get; set; } = string.Empty;
    public Guid SprId { get; set; }
    public Sprache? Sprache { get; set; }
}