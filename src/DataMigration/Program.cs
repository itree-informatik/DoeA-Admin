// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.Diagnostics;
using DataMigration;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Npgsql;

// Konfiguration: appsettings.json -> User-Secrets (Dev/Windows) -> Umgebungsvariablen (Prod/Linux) -> Args.
// Connection-Strings produktiv z. B. via Env: ConnectionStrings__Source / ConnectionStrings__Target
var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true)
    .AddUserSecrets(typeof(Program).Assembly, optional: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

var sourceCs = config.GetConnectionString("Source");
var targetCs = config.GetConnectionString("Target");
var commandTimeout = int.TryParse(config["CommandTimeoutSeconds"], out var ct) ? ct : 600;

if (string.IsNullOrWhiteSpace(sourceCs) || string.IsNullOrWhiteSpace(targetCs))
{
    Console.Error.WriteLine("Fehlende Connection-Strings. 'ConnectionStrings:Source' (SQL Server) und " +
                            "'ConnectionStrings:Target' (PostgreSQL) müssen gesetzt sein " +
                            "(appsettings.json, User-Secrets oder Umgebungsvariablen).");
    return 1;
}

var totalWatch = Stopwatch.StartNew();

try
{
    await using var source = new SqlConnection(sourceCs);
    await using var target = new NpgsqlConnection(targetCs);
    await source.OpenAsync();
    await target.OpenAsync();

    Console.WriteLine($"Quelle (SQL Server): {source.DataSource} / {source.Database}");
    Console.WriteLine($"Ziel   (PostgreSQL): {target.Host}:{target.Port} / {target.Database}");
    Console.WriteLine();

    // Alles in einer Transaktion: entweder vollständig oder gar nicht.
    await using var tx = await target.BeginTransactionAsync();

    // 1) Zieltabellen leeren (FK-sichere Reihenfolge)
    Console.WriteLine("Leere Zieltabellen ...");
    foreach (var table in Migrations.DeleteOrder)
    {
        await using var delete = new NpgsqlCommand($"delete from {table}", target, tx);
        delete.CommandTimeout = commandTimeout;
        var deleted = await delete.ExecuteNonQueryAsync();
        Console.WriteLine($"  - {table}: {deleted} gelöscht");
    }
    Console.WriteLine();

    // 2) Daten kopieren (FK-sichere Insert-Reihenfolge)
    Console.WriteLine("Kopiere Daten ...");
    long grandTotal = 0;
    foreach (var migration in Migrations.All)
    {
        var rows = await CopyTableAsync(source, target, tx, migration, commandTimeout);
        grandTotal += rows;
        Console.WriteLine($"  - {migration.TargetTable}: {rows} kopiert");
    }

    // 3) Zusätzliche Verfahrensarten einfügen, die es im Altsystem nicht gibt. Die Audit-Felder
    //    (Inserted/Changed) werden aus einer bestehenden Verfahrensart übernommen.
    //    Die Vorankündigungs-Varianten sortieren direkt hinter "Vorankündigung offen" (Sortierung 30;
    //    Umbenennung von Bezeichnung und NaturalId erfolgt im Select der Migration).
    Console.WriteLine();
    Console.WriteLine("Füge zusätzliche Verfahrensarten 'RFI', 'Einladung', 'Vorankündigung selektiv/unbekannt' ein ...");
    var neueVerfahrensarten = new (string Bezeichnung, string Kurzbezeichnung, int Sortierung, string NaturalId)[]
    {
        ("Vorankündigung selektiv", "VS", 31, "VorankündigungSelektiv"),
        ("Vorankündigung unbekannt", "VU", 32, "VorankündigungUnbekannt"),
        ("RFI", "R", 40, "RFI"),
        ("Einladung", "E", 50, "Einladung"),
    };
    foreach (var vea in neueVerfahrensarten)
    {
        await using var insert = new NpgsqlCommand(
            @"insert into dbo.t_verfahrensart
                  (veaid, veabezeichnung, veakurzbezeichnung, veasortierung, veanaturalid, veaaktiv,
                   veainserteddate, veainserteduser, veachangeddate, veachangeduser)
              select @veaid, @bezeichnung, @kurzbezeichnung, @sortierung, @naturalid, true,
                     veainserteddate, veainserteduser, veachangeddate, veachangeduser
              from dbo.t_verfahrensart
              order by veaid
              limit 1",
            target, tx);
        insert.CommandTimeout = commandTimeout;
        insert.Parameters.AddWithValue("veaid", Guid.NewGuid());
        insert.Parameters.AddWithValue("bezeichnung", vea.Bezeichnung);
        insert.Parameters.AddWithValue("kurzbezeichnung", vea.Kurzbezeichnung);
        insert.Parameters.AddWithValue("sortierung", vea.Sortierung);
        insert.Parameters.AddWithValue("naturalid", vea.NaturalId);
        var inserted = await insert.ExecuteNonQueryAsync();
        Console.WriteLine($"  - dbo.t_verfahrensart '{vea.Bezeichnung}': {inserted} eingefügt");
    }

    // 4) Zusätzliche Offertezustände einfügen, die es im Altsystem nicht gibt (nur bei Verfahrensart
    //    Selektiv wählbar, siehe VerfahrensartRegeln.IstOffertezustandWaehlbar). Audit-Felder analog Schritt 3.
    Console.WriteLine();
    Console.WriteLine("Füge zusätzliche Offertezustände 'Teilnahmeantrag erfasst/abgelehnt' und 'Zuschlag/Widerruf Präqualifikation' ein ...");
    var neueOffertezustaende = new (string Bezeichnung, string Kurzbezeichnung, bool Schlusszustand, bool Publikation, string NaturalId)[]
    {
        // Phase 1 des selektiven Verfahrens: Ein erfasster Teilnahmeantrag läuft weiter, ein abgelehnter ist abschliessend.
        ("Teilnahmeantrag erfasst", "TA erfasst", false, false, "teilnahmeantragerfasst"),
        ("Teilnahmeantrag abgelehnt", "TA abgelehnt", true, false, "teilnahmeantragabgelehnt"),
        // Zuschlag Präquali ist Zwischenzustand (die Offerte läuft in Phase 2 weiter und erhält
        // später einen anderen Schlusszustand); nur der Präquali-Widerruf ist abschliessend. Auf beide folgt eine Publikation.
        ("Zuschlag Präqualifikation", "Zuschlag PQ", false, true, "zuschlagpraequali"),
        ("Widerruf Präqualifikation", "Widerruf PQ", true, true, "widerrufpraequali"),
    };
    foreach (var ofz in neueOffertezustaende)
    {
        await using var insert = new NpgsqlCommand(
            @"insert into dbo.t_offertezustand
                  (ofzid, ofzbezeichnung, ofzkurzbezeichnung, ofzschlusszustand, ofzpublikation, ofznaturalid, ofzaktiv,
                   ofzinserteddate, ofzinserteduser, ofzchangeddate, ofzchangeduser)
              select @ofzid, @bezeichnung, @kurzbezeichnung, @schlusszustand, @publikation, @naturalid, true,
                     ofzinserteddate, ofzinserteduser, ofzchangeddate, ofzchangeduser
              from dbo.t_offertezustand
              order by ofzid
              limit 1",
            target, tx);
        insert.CommandTimeout = commandTimeout;
        insert.Parameters.AddWithValue("ofzid", Guid.NewGuid());
        insert.Parameters.AddWithValue("bezeichnung", ofz.Bezeichnung);
        insert.Parameters.AddWithValue("kurzbezeichnung", ofz.Kurzbezeichnung);
        insert.Parameters.AddWithValue("schlusszustand", ofz.Schlusszustand);
        insert.Parameters.AddWithValue("publikation", ofz.Publikation);
        insert.Parameters.AddWithValue("naturalid", ofz.NaturalId);
        var inserted = await insert.ExecuteNonQueryAsync();
        Console.WriteLine($"  - dbo.t_offertezustand '{ofz.Bezeichnung}': {inserted} eingefügt");
    }

    await tx.CommitAsync();

    totalWatch.Stop();
    Console.WriteLine();
    Console.WriteLine($"Fertig. {grandTotal} Zeilen in {Migrations.All.Length} Tabellen " +
                      $"in {totalWatch.Elapsed.TotalSeconds:F1}s migriert.");
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine();
    Console.Error.WriteLine("Migration fehlgeschlagen, Transaktion wird zurückgerollt:");
    Console.Error.WriteLine(ex);
    return 1;
}

// Liest die Quelltabelle (SQL Server) und schreibt sie via Binary-COPY in die Zieltabelle (PostgreSQL).
static async Task<long> CopyTableAsync(
    SqlConnection source, NpgsqlConnection target, NpgsqlTransaction tx,
    TableMigration migration, int commandTimeout)
{
    await using var read = new SqlCommand(migration.SourceSelect, source) { CommandTimeout = commandTimeout };
    await using var reader = await read.ExecuteReaderAsync();

    if (reader.FieldCount != migration.Columns.Count)
        throw new InvalidOperationException(
            $"{migration.TargetTable}: Quelle liefert {reader.FieldCount} Spalten, " +
            $"Ziel erwartet {migration.Columns.Count}.");

    var columnList = string.Join(", ", migration.Columns.Select(c => c.Name));
    var copyCommand = $"copy {migration.TargetTable} ({columnList}) from stdin (format binary)";

    await using var writer = await target.BeginBinaryImportAsync(copyCommand);

    long rows = 0;
    while (await reader.ReadAsync())
    {
        await writer.StartRowAsync();
        for (var i = 0; i < migration.Columns.Count; i++)
        {
            var col = migration.Columns[i];
            if (await reader.IsDBNullAsync(i))
            {
                await writer.WriteNullAsync();
            }
            else
            {
                var value = reader.GetValue(i);
                if (col.Convert != null)
                    value = col.Convert(value);
                await writer.WriteAsync(value, col.Type);
            }
        }
        rows++;
    }

    await writer.CompleteAsync();
    return rows;
}
