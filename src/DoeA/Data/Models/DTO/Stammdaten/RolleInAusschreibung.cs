// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using ItreeMud.Models;

namespace DoeA.Data.Models.DTO.Stammdaten;

/// <summary>Rolle des BBL in einer Ausschreibung (z.B. Meldestelle, Vergabestelle) — abgebildet auf dbo.t_rolleinausschreibung (Präfix "Ria").</summary>
public class RolleInAusschreibung : Basis
{
    public string Bezeichnung { get; set; } = string.Empty;
    public string Kurzbezeichnung { get; set; } = string.Empty;

}