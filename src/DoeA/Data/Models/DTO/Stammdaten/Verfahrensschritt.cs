// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using ItreeMud.Models;

namespace DoeA.Data.Models.DTO.Stammdaten;

/// <summary>Verfahrensschritt der Terminplanung — abgebildet auf dbo.t_verfahrensschritt (Präfix "Ves").</summary>
public class Verfahrensschritt : Basis
{
    public string Bezeichnung { get; set; } = string.Empty;
    public int Sortierung { get; set; }

    /// <summary>Die Dauer des Termins zählt nur Arbeitstage (ohne Wochenenden und Feiertage des Kantons Bern).</summary>
    public bool NurWerktage { get; set; }

    /// <summary>Schritt gehört zum offenen Verfahren.</summary>
    public bool Offen { get; set; }

    /// <summary>Schritt gehört zum selektiven Verfahren.</summary>
    public bool Selektiv { get; set; }
}