// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using ItreeMud.Components;

namespace DoeA.Data.Helper;

/// <summary>Gemeinsames Theme für MainLayout und AnmeldungLayout, damit beide Rahmen dieselben Farben zeigen.</summary>
public static class DoeATheme
{
    /// <summary>
    /// ItreeMud-Theme mit den Rottönen des Web-CD Bund (swiss.github.io/styleguide: $venetian-red als Primärrot, $red als Akzentrot)
    /// und Noto Sans als Schrift (ItreeMud-Standard wäre Roboto). Farben nur hier anpassen, nicht in app.css: der
    /// ThemeProvider rendert sein :root-Style nach app.css und leitet die Varianten (-rgb, -hover, -text, Darken) aus der Palette ab.
    /// </summary>
    public static readonly ItreeTheme Standard = ErstelleTheme();

    private static ItreeTheme ErstelleTheme()
    {
        var theme = ItreeTheme.ErstelleKundenTheme("#dc0018", "#ffffff", schriften: ["Noto Sans", "Helvetica", "Arial", "sans-serif"]);
        theme.PaletteLight.PrimaryLighten = "#f7001d";
        return theme;
    }
}
