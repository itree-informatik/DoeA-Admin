// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Constants;
using DoeA.Data.Models.DTO;
using DoeA.Interfaces;
using ItreeMud.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using ILogger = Serilog.ILogger;

namespace DoeA.Services;

/// <inheritdoc cref="IErrorHandlerService"/>
public class ErrorHandlerService : IErrorHandlerService
{
    /// <summary>Präfix, an dem bereits übersetzbare Fehlermeldungen erkannt werden.</summary>
    private const string FehlerPraefix = "ERROR_";

    private readonly ILogger _logger;

    public ErrorHandlerService(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>Übersetzungsschlüssel der aufgetretenen Fehlermeldung; von der UI abonniert.</summary>
    public event Action<string>? OnError;

    /// <summary>Führt eine Operation ohne Ergebnismodell aus; liefert false, wenn sie mit einer gemeldeten Exception scheitert.</summary>
    public async Task<bool> HandleAsync(Func<Task> operation)
    {
        try
        {
            await operation();
            return true;
        }
        catch (Exception ex)
        {
            HandleException(ex);
            return false;
        }
    }

    /// <summary>Führt die Operation aus und liefert im Fehlerfall eine leere Instanz statt einer Exception.</summary>
    public async Task<TModel> HandleAsync<TModel>(Func<Task<ServiceResult<TModel>>> operation) where TModel : Basis
    {
        try
        {
            var result = await operation();
            return result.Item ?? Activator.CreateInstance<TModel>();
        }
        catch (Exception ex)
        {
            HandleException(ex);
            return Activator.CreateInstance<TModel>();
        }
    }

    /// <summary>Führt die Operation aus und liefert im Fehlerfall eine leere Liste statt einer Exception.</summary>
    public async Task<List<TModel>?> HandleListAsync<TModel>(Func<Task<ServiceResult<TModel>>> operation)
        where TModel : Basis
    {
        try
        {
            var result = await operation();
            return result.Items ?? new List<TModel>();
        }
        catch (Exception ex)
        {
            HandleException(ex);
            return new List<TModel>();
        }
    }

    private void HandleException(Exception ex)
    {
        var (className, methodName) = ExceptionHelper.GetExceptionOrigin(ex);
        _logger.Error(GeneralConstants.MessageTemplate, className, methodName, ex.Message);

        // Unique-Constraint-Verletzung (z.B. doppelte BKP/CPV-Nummer) verständlich melden.
        if (ex is DbUpdateException
            {
                InnerException: PostgresException { SqlState: PostgresErrorCodes.UniqueViolation }
            })
        {
            OnError?.Invoke(GeneralConstants.DuplicateKeyError);
            return;
        }

        if (ex.Message.StartsWith(FehlerPraefix))
        {
            OnError?.Invoke(ExceptionHelper.GenerateExceptionMessage(ex.Message));
            return;
        }

        OnError?.Invoke(GeneralConstants.UnhandledException);
    }
}
