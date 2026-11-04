// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.Globalization;

namespace DoeA.Data.Helper;

/// <summary>Alphabetische Reihenfolge der Auswahllisten nach dem angezeigten Text, unabhängig von der Kollation der Datenbank.</summary>
public static class SortierHelper
{
    // de-CH: Umlaute wie ihre Grundbuchstaben, Gross-/Kleinschreibung zählt nicht.
    private static readonly StringComparer Alphabet = StringComparer.Create(CultureInfo.GetCultureInfo("de-CH"), CompareOptions.IgnoreCase);

    /// <summary>Liefert die Einträge alphabetisch nach <paramref name="text"/> als neue Liste; <c>null</c> ergibt eine leere Liste.</summary>
    public static List<T> Alphabetisch<T>(this IEnumerable<T>? eintraege, Func<T, string> text) =>
        (eintraege ?? []).OrderBy(text, Alphabet).ToList();
}
