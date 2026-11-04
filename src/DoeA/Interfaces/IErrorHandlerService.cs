// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DTO;
using ItreeMud.Models;

namespace DoeA.Interfaces;

/// <summary>
/// Führt Service-Operationen aus, protokolliert Exceptions und meldet eine übersetzbare
/// Fehlermeldung über <see cref="OnError"/> an die UI, statt sie durchzureichen.
/// </summary>
public interface IErrorHandlerService
{
    /// <summary>Übersetzungsschlüssel der aufgetretenen Fehlermeldung; von der UI abonniert.</summary>
    event Action<string>? OnError;

    /// <summary>Führt eine Operation ohne Ergebnismodell aus; liefert false, wenn sie mit einer gemeldeten Exception scheitert.</summary>
    Task<bool> HandleAsync(Func<Task> operation);

    /// <summary>Führt die Operation aus und liefert im Fehlerfall eine leere Instanz statt einer Exception.</summary>
    Task<TModel> HandleAsync<TModel>(Func<Task<ServiceResult<TModel>>> operation) where TModel : Basis;

    /// <summary>Führt die Operation aus und liefert im Fehlerfall eine leere Liste statt einer Exception.</summary>
    Task<List<TModel>?> HandleListAsync<TModel>(Func<Task<ServiceResult<TModel>>> operation)
        where TModel : Basis;
}
