// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.Diagnostics.CodeAnalysis;
using ItreeMud.Services;

namespace DoeA.Data.Helper;

/// <summary>UI-Sprachen der Applikation (Codes des LanguageService) und Abbildung des eIAM-Claims "language".</summary>
public static class SprachHelper
{
    /// <summary>Rückfallsprache; entspricht dem Startwert von LanguageService.CurrentLanguage.</summary>
    public const string Standard = "de";

    // Nur Sprachen mit eigener Spalte in t_translation; der ItreeMud-Standard enthält zusätzlich Englisch.
    private static readonly string[] SprachenMitUebersetzung = ["de", "fr", "it"];

    /// <summary>
    /// Schränkt die ItreeMud-Standardsprachen beim Start auf die übersetzten ein — ItreeLanguagePicker und IstUnterstuetzt
    /// arbeiten danach auf derselben Liste, eine Auswahl ohne Übersetzung ist nicht mehr möglich.
    /// </summary>
    public static void KonfiguriereSprachen() =>
        LanguageService.SupportedLanguages = LanguageService.DefaultLanguages.Where(l => SprachenMitUebersetzung.Contains(l.Code)).ToList();

    /// <summary>Bildet den eIAM-Claim (z.B. "DE" oder "de-CH") auf einen unterstützten Sprachcode ab; Unbekanntes fällt auf Deutsch zurück.</summary>
    public static string AusClaim(string? language)
    {
        var code = language?.Trim().ToLowerInvariant();
        if (code is { Length: > 2 })
        {
            code = code[..2];
        }

        return IstUnterstuetzt(code) ? code : Standard;
    }

    /// <summary>true für einen exakten Code aus LanguageService.SupportedLanguages (kleingeschrieben, ohne Region).</summary>
    public static bool IstUnterstuetzt([NotNullWhen(true)] string? sprache) => LanguageService.IsSupported(sprache);
}
