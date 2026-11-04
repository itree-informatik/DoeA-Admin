// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

namespace DbUpdater.Models;

/// <summary>Aus einer MudBlazor-.resx-Datei gelesener Eintrag; pro Sprache ist nur die zugehörige Spalte gefüllt.</summary>
public class MudBlazorTranslation
{
    public string Key { get; set; } = string.Empty;
    public string Deutsch { get; set; } = string.Empty;
    public string Franzoesisch { get; set; } = string.Empty;
    public string Italienisch { get; set; } = string.Empty;
}
