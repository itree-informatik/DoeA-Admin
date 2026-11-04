// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

namespace DoeA.Data.Models.DTO.Einstellungen;

/// <summary>Filter der Log-Ansicht: Level, Zeitraum, Suchtext und Zeilenlimit.</summary>
public class LogSearchParameter
{
    /// <summary>Obergrenze der gelieferten Zeilen; ohne Standardwert käme bei 0 eine leere Liste zurück.</summary>
    public int MaxRows { get; set; } = 1000;
    public string LogLevel { get; set; } = string.Empty;
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public string SearchText { get; set; } = string.Empty;
}
