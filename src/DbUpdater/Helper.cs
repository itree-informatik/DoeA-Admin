// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DbUp;
using DbUp.Helpers;
using DbUpdater.Models.DB;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DbUpdater;

/// <summary>Einstellungen des DbUpdater-Laufs (Verbindung, Skriptpfad, Timeout) und die Ausführung einzelner Skripte über DbUp.</summary>
public static class Helper
{
    /// <summary>Timeout in Sekunden pro Skript und für den EF-Context.</summary>
    public static int CommandTimeout { get; set; }
    /// <summary>Verbindung zur Zieldatenbank aus CLI-Argument, User Secrets oder Umgebungsvariable.</summary>
    public static string? ConnectionString { get; set; }
    /// <summary>Wurzelordner der Skripte (RelNNNN-Unterordner); wird aus dem Dateipfad entfernt, um den Journalnamen zu bilden.</summary>
    public static string? ScriptPath { get; set; }
    /// <summary>Gemeinsamer EF-Context, um dbo.schemaversions vor jedem Skript zu lesen.</summary>
    public static Context DbContext = new();
    /// <summary>Konsolenfarbe beim Start, um sie nach farbigen Meldungen zurückzusetzen.</summary>
    public static ConsoleColor OriginalColor = Console.ForegroundColor;

    /// <summary>
    /// Führt ein SQL-Skript via DbUp aus. Bei <paramref name="writeToSchemaVersions"/> journalt DbUp den
    /// Eintrag in dbo.schemaversions innerhalb derselben Transaktion wie das Skript — ein Absturz zwischen
    /// Skript und Protokolleintrag kann so keinen halb applizierten Zustand hinterlassen.
    /// </summary>
    /// <param name="file">Pfad inklusive Dateiname</param>
    /// <param name="writeToSchemaVersions">false führt das Skript ohne Protokollierung aus (läuft dann bei jedem Lauf erneut).</param>
    public static async Task ExecuteScript(string file, bool writeToSchemaVersions = true)
    {
        try
        {
            // TrimStart statt Index-Zugriff: Ob nach dem Entfernen des ScriptPath ein führender Slash
            // übrig bleibt, hängt davon ab, ob der Pfad mit Schluss-Slash konfiguriert wurde — ein fixes
            // [1..] schnitt sonst das erste Zeichen des Ordnernamens ab ("el0000." statt "Rel0000.").
            var entry = file.Replace(ScriptPath!, string.Empty).Replace("/", ".").TrimStart('.');
            var row = DbContext.SchemaVersion.FirstOrDefault(l => l.ScriptName == entry);
            if (row != null)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($@"Already executed: '{file}'");
                Console.ForegroundColor = OriginalColor;
                return;
            }


            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($@"Execute file: '{file}'");
            Console.ForegroundColor = OriginalColor;

            // Der Skriptname wird explizit gesetzt (RelNNNN.Dateiname), damit die Journal-Einträge
            // zum bisherigen Format in dbo.schemaversions kompatibel bleiben.
            var builder = DeployChanges.To
                .PostgresqlDatabase(DbContext.Database.GetConnectionString())
                .WithScript(entry, await File.ReadAllTextAsync(file, Encoding.UTF8))
                .WithTransactionPerScript()
                .WithExecutionTimeout(TimeSpan.FromSeconds(CommandTimeout));

            builder = writeToSchemaVersions
                ? builder.JournalToPostgresqlTable("dbo", "schemaversions")
                : builder.JournalTo(new NullJournal());

            var databaseUpgradeResult = builder.Build().PerformUpgrade();
            if (!databaseUpgradeResult.Successful)
            {
                throw new Exception("Error in script");
            }
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($@"{Environment.NewLine}{e}");
            Console.ForegroundColor = OriginalColor;
            throw;
        }
    }

}