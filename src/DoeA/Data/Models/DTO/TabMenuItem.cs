// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

namespace DoeA.Data.Models.DTO;

/// <summary>Tab-Eintrag einer Seite mit Tab-Navigation (z.B. Tabs der Ausschreibung).</summary>
public class TabMenuItem
{
    public string TitleKey { get; set; } = string.Empty;
    public string? Url { get; set; }
    public int Sort { get; set; }
    public string? Title { get; set; }
    public string? SiteTitle { get; set; }
}
