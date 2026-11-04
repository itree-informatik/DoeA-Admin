// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

namespace DoeA.Interfaces.Einstellungen;

/// <summary>Zugriff auf die datenbankbasierten Übersetzungen (Tabelle T_Translation).</summary>
public interface ITranslationService
{
    /// <summary>Speichert die Übersetzungen eines Eintrags; liefert false, wenn zur Id kein Eintrag existiert.</summary>
    Task<bool> UpdateTranslation(Guid id, string deutsch, string? franzoesisch, string? italienisch);

    /// <summary>Übersetzt einen Schlüssel (SCREAMING_SNAKE_CASE) in der aktuellen Sprache.</summary>
    string Translate(string key);
}
