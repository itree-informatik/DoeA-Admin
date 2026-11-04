// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using ItreeMud.Models;

namespace DoeA.Data.Models.DTO.Stammdaten;

/// <summary>Zustand einer Offerte (z.B. erfasst, abgelehnt) — abgebildet auf dbo.t_offertezustand (Präfix "Ofz").</summary>
public class Offertezustand : Basis
{
    public string Bezeichnung { get; set; } = string.Empty;
    public string Kurzbezeichnung { get; set; } = string.Empty;

    /// <summary>Beendet die Offerte; fliesst in die Ermittlung des Loszustands ein (siehe LosService).</summary>
    public bool Schlusszustand { get; set; }
    /// <summary>Auf den Zustand folgt eine Publikation; bei den Offerten erscheinen dann Publikation, Anzahl Tage und Verzögerung.</summary>
    public bool Publikation { get; set; }
}