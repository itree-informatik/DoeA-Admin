// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using FluentValidation;

namespace DoeA.Interfaces;

/// <summary>
/// Querschnittsfunktionen für Services: Validierung inkl. Audit-Feldern und Änderungs-/Concurrency-Prüfung.
/// </summary>
public interface IBaseFunctionService
{
    /// <summary>Setzt Id, Aktiv-Flag und Audit-Felder per Reflection und validiert anschliessend.</summary>
    Task Validate<T>(AbstractValidator<T> validator, T model, bool isnew, long? date = null) where T : class;

    /// <summary>Validiert das Modell und wirft bei Fehlern eine Exception mit allen Meldungen.</summary>
    Task Validate<T>(AbstractValidator<T> validator, T model) where T : class;

    /// <summary>
    /// Vergleicht das Modell mit dem DB-Stand und liefert <c>OK</c>, <c>NOTHING_CHANGED</c>,
    /// <c>ERROR_NOT_FOUND</c> oder <c>ERROR_CONCURRENCY_EXCEPTION</c>.
    /// <paramref name="prefix"/> ist das 3-Buchstaben-Spaltenpräfix der Entity.
    /// </summary>
    Task<string> CheckChangesAndConcurrency<TDbEntity>(object model, string prefix, string? ignoreProperties = null) where TDbEntity : class;

    /// <summary>Wie <see cref="CheckChangesAndConcurrency{TDbEntity}"/>, meldet aber alles ausser «geändert» als Exception.</summary>
    Task CheckChangesAndConcurrencyVoid<TDbEntity>(object model, string prefix, string? ignoreProperties = null) where TDbEntity : class;
}
