// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

namespace DoeA.Data.Mapper;

/// <summary>Konvertierungen, die alle Mapper über <c>[UseStaticMapper]</c> teilen.</summary>
public static class TextKonvertierung
{
    /// <summary>
    /// Schreibt einen leeren DTO-Text als NULL in eine nullable Spalte zurück. Die DTO-Basis
    /// initialisiert Texte mit <c>string.Empty</c>, und Mapperly überspringt beim Laden eine
    /// NULL-Quelle — ohne diese Umkehr würde jedes NULL beim ersten Speichern zu '' werden.
    /// </summary>
    public static string? LeerZuNull(string wert) => string.IsNullOrEmpty(wert) ? null : wert;
}
