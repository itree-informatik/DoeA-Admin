// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using ItreeMud.Services;

namespace DoeA.Data.Helper;

/// <summary>Bindet die ItreeMud-Komponenten an die datenbankbasierten Übersetzungen des TranslationHelper.</summary>
public class DoeATranslationProvider : ITranslationProvider
{
    /// <summary>Übersetzt den Schlüssel in die Sprache des aktuellen Circuits.</summary>
    public string Translate(string key) => TranslationHelper.Translate(key);
}
