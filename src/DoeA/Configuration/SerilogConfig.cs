// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using Microsoft.Extensions.Logging.Console;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;
using NpgsqlTypes;

namespace DoeA.Configuration;

/// <summary>Serilog-Logger aus der Sektion «Serilog» der Konfiguration, mit PostgreSQL-Sink in T_Log und Konsole.</summary>
public static class SerilogConfig
{
    /// <summary>Zeitstempel jeder Konsolenmeldung; mit Offset, damit die doppelte Stunde bei der Umstellung auf Winterzeit eindeutig bleibt.</summary>
    private const string KonsolenZeitstempel = "yyyy-MM-dd HH:mm:ss.fff zzz";

    /// <summary>Erstellt den Logger, registriert ihn samt LoggingLevelSwitch als Singleton und versieht die Konsolenausgabe mit Zeitstempel.</summary>
    public static void AddSerilogServices(
        IServiceCollection service,
        IConfiguration configSettings)
    {
        // Meldungen des Frameworks (ASP.NET Core, Blazor-Circuits, EF Core) schreibt nicht Serilog, sondern der
        // Konsolen-Logger von .NET, und der lässt den Zeitstempel standardmässig weg. AddSimpleConsole statt nur
        // Configure<SimpleConsoleFormatterOptions>: ohne FormatterName überschreibt der Logger diese Optionen.
        service.AddLogging(logging => logging.AddSimpleConsole(o => o.TimestampFormat = KonsolenZeitstempel + " "));

        // Ohne Serilog-Sektion bleibt der Standard-Logger aktiv statt mit fehlenden Schlüsseln abzubrechen.
        if (configSettings.GetChildren().Any(x => x.Key == "Serilog"))
        {
            var connectionString = configSettings.GetConnectionString("APP");
            var table = configSettings["Serilog:Table"];
            var levelSwitch = new LoggingLevelSwitch(LogLevel(configSettings["Serilog:MinimumLevel"]!));

            IDictionary<string, ColumnWriterBase> columnWriters = new Dictionary<string, ColumnWriterBase>
            {
                { "Message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
                { "MessageTemplate", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
                { "Level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
                { "TimeStamp", new TimestampColumnWriter(NpgsqlDbType.Timestamp) },
                { "Exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
                { "Properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb) }
            };

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(levelSwitch)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.PostgreSQL(
                    connectionString: connectionString,
                    tableName: table,
                    columnOptions: columnWriters,
                    // Ohne Schemanamen schreibt der Sink unqualifiziertes «T_Log» und PostgreSQL sucht die
                    // Tabelle über den search_path, der dbo nicht enthält — jeder Schreibversuch scheitert
                    // still mit «relation t_log does not exist».
                    schemaName: DoeAContext.Schema,
                    // T_Log kommt aus den DbUp-Skripten; der Sink darf die Tabelle nicht selbst anlegen.
                    needAutoCreateTable: false
                )
                .WriteTo.Console(outputTemplate: $"{{Timestamp:{KonsolenZeitstempel}}} [{{Level:u3}}] {{Message:lj}}{{NewLine}}{{Exception}}")
                .CreateLogger();

            Log.Information("Application started");
            service.AddSingleton(Log.Logger);
            service.AddSingleton(levelSwitch);
        }
    }

    /// <summary>Wandelt den konfigurierten Level-Namen in ein LogEventLevel um; Unbekanntes wird zu Warning.</summary>
    public static LogEventLevel LogLevel(string logLevelString)
    {
        return logLevelString switch
        {
            "Verbose" => LogEventLevel.Verbose,
            "Debug" => LogEventLevel.Debug,
            "Information" => LogEventLevel.Information,
            "Warning" => LogEventLevel.Warning,
            "Error" => LogEventLevel.Error,
            "Fatal" => LogEventLevel.Fatal,
            _ => LogEventLevel.Warning
        };
    }
}
