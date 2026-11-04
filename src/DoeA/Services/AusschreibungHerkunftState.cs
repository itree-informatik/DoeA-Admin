// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

namespace DoeA.Services;

/// <summary>
/// Merkt sich, woher die Ausschreibung zuletzt geöffnet wurde, damit «Zurück zur Suche» dorthin führt.
/// Scoped registriert -> gilt für den Blazor-Circuit, nach Reload/Neuverbindung führt der Button zur Suche.
/// </summary>
public class AusschreibungHerkunftState
{
    /// <summary>Ziel von «Zurück zur Suche»; null führt zur Ausschreibungssuche.</summary>
    public string? RuecksprungUrl { get; set; }
}
