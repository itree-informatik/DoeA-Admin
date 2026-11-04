// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using ItreeMud.Models;

namespace DoeA.Data.Models.DTO.Stammdaten;

/// <summary>Amt (Dienststelle) mit Departements-Zuordnung — abgebildet auf dbo.t_amt (Präfix "Amt").</summary>
public class Amt : Basis
{
    public string Bezeichnung { get; set; } = string.Empty;
    public string Kurzbezeichnung { get; set; } = string.Empty;
    public string Dienststellennummer { get; set; } = string.Empty;
    public Departement? Departement { get; set; }
    public Guid DepId { get; set; }

}