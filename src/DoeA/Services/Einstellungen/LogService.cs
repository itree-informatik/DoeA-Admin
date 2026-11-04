// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Mapper.Einstellungen;
using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO.Einstellungen;
using DoeA.Interfaces;
using DoeA.Interfaces.Einstellungen;
using Microsoft.EntityFrameworkCore;
using Serilog.Core;
using Serilog.Events;

namespace DoeA.Services.Einstellungen;

/// <summary>Liest die von Serilog geschriebenen Logeinträge und steuert das Mindest-Loglevel zur Laufzeit.</summary>
public class LogService : ILogService
{
    // Auswahl im Levelfilter, bei der nicht auf ein einzelnes Level eingeschränkt wird.
    private const string AlleLevel = "All";

    private const string ExportDateiname = "ExportLog.xlsx";

    // Konfigurationseintrag mit der Aufbewahrungsdauer der Logeinträge in Tagen.
    private const string KonfigAnzahlTageLog = "AnzahlTageLog";

    // Fallback, falls der Konfigurationseintrag fehlt oder keine gültige Zahl enthält.
    private const int StandardAufbewahrungTage = 30;

    private readonly IDbContextFactory<DoeAContext> _dbFactory;
    private readonly IExportToExcelService _exportToExcelService;
    private readonly LogMapper _logMapper;
    private readonly LoggingLevelSwitch _logLevelSwitcher;

    public LogService(IDbContextFactory<DoeAContext> dbFactory, LogMapper logMapper, IExportToExcelService exportToExcelService, LoggingLevelSwitch logLevelSwitcher)
    {
        _dbFactory = dbFactory;
        _exportToExcelService = exportToExcelService;
        _logLevelSwitcher = logLevelSwitcher;
        _logMapper = logMapper;
    }

    /// <summary>Liefert die neuesten Logeinträge zu den Suchkriterien.</summary>
    public async Task<IList<Log>> Search(LogSearchParameter parameter)
    {
        await using var dbContext = await _dbFactory.CreateDbContextAsync();
        var query = dbContext.TLog.AsNoTracking();

        if (parameter.From != null)
        {
            query = query.Where(x => x.TimeStamp >= parameter.From.Value);
        }

        if (parameter.To != null)
        {
            query = query.Where(x => x.TimeStamp <= parameter.To.Value);
        }

        if (!string.IsNullOrWhiteSpace(parameter.SearchText))
        {
            // Contains ist unter PostgreSQL case-sensitiv; ToLower macht die Suche unabhängig von der
            // Gross-/Kleinschreibung (kein ILike, weil % und _ sonst als Platzhalter wirken).
            var suchtext = parameter.SearchText.ToLower();
            query = query.Where(x => (x.Message != null && x.Message.ToLower().Contains(suchtext)) ||
                                     (x.Exception != null && x.Exception.ToLower().Contains(suchtext)));
        }

        if (parameter.LogLevel != AlleLevel)
        {
            query = query.Where(x => x.Level == parameter.LogLevel);
        }

        // Absteigend nach Id, damit bei der Begrenzung auf MaxRows die jüngsten Einträge übrig bleiben.
        var logs = await query
            .OrderByDescending(x => x.Id)
            .Take(parameter.MaxRows)
            .ToListAsync();
        return logs.Select(_logMapper.ZuModel).ToList();
    }

    /// <summary>Exportiert die Treffer der Suchkriterien nach Excel und liefert die Id des bereitgestellten Downloads.</summary>
    public async Task<string> Export(LogSearchParameter parameter)
    {
        var logs = await Search(parameter);
        return await _exportToExcelService.ExportToExcel(ExportDateiname, logs, string.Empty);
    }

    /// <summary>
    /// Löscht Logeinträge, die älter sind als die konfigurierte Aufbewahrungsdauer
    /// (<c>AnzahlTageLog</c>), und liefert die Anzahl der entfernten Zeilen.
    /// </summary>
    public async Task<int> Delete()
    {
        await using var dbContext = await _dbFactory.CreateDbContextAsync();

        var aufbewahrungTage = await LeseAufbewahrungstage(dbContext);
        var grenze = DateTime.Now.AddDays(-aufbewahrungTage);

        // Einträge ohne Zeitstempel bleiben stehen: ihr Alter ist nicht bestimmbar.
        return await dbContext.TLog
            .Where(x => x.TimeStamp != null && x.TimeStamp < grenze)
            .ExecuteDeleteAsync();
    }

    /// <summary>Liest die Aufbewahrungsdauer aus der Konfiguration; fällt auf den Standardwert zurück.</summary>
    private static async Task<int> LeseAufbewahrungstage(DoeAContext dbContext)
    {
        var wert = await dbContext.TKonfiguration
            .AsNoTracking()
            .Where(k => k.KonNaturalId == KonfigAnzahlTageLog)
            .Select(k => k.KonWert)
            .FirstOrDefaultAsync();

        // Ein fehlender oder unbrauchbarer Wert darf die Bereinigung nicht verhindern.
        return int.TryParse(wert, out var tage) && tage > 0 ? tage : StandardAufbewahrungTage;
    }

    /// <summary>Liefert das aktuelle Mindest-Loglevel als Zahlenwert des Serilog-LogEventLevel.</summary>
    public int GetLogLevel()
    {
        return (int)_logLevelSwitcher.MinimumLevel;
    }

    /// <summary>Setzt das Mindest-Loglevel zur Laufzeit; der LoggingLevelSwitch wirkt sofort auf alle Sinks.</summary>
    public void SetLogLevel(int logLevel)
    {
        _logLevelSwitcher.MinimumLevel = (LogEventLevel)logLevel;
    }
}
