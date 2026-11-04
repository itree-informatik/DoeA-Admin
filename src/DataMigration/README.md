# DataMigration

Einmalige Datenmigration **SQL Server → PostgreSQL** für DoeA.

Liest per `Microsoft.Data.SqlClient` aus der SQL-Server-Quelle und schreibt per Npgsql
(Binary `COPY`) in die PostgreSQL-Zieldatenbank. Läuft plattformübergreifend
(Entwicklung lokal auf Windows, produktive Migration auf Linux).

## Voraussetzungen

- Die Zieldatenbank (PostgreSQL) ist bereits per **DbUpdater** aufgesetzt (Schema `dbo`,
  Tabellen, Funktionen, Seed-Daten aus `Rel0000`).
- An der Quelldatenbank (SQL Server) sind **keine** Änderungen nötig (nur Lesezugriff);
  die `datetime → bigint`-Ticks-Umrechnung passiert in C#.

## Was es tut

1. Leert die Zieltabellen in FK-sicherer Reihenfolge (`Migrations.DeleteOrder`).
2. Kopiert 27 Tabellen in FK-sicherer Reihenfolge (`Migrations.All`).
3. Alles in **einer Transaktion** auf der Zielseite (Commit nur bei vollständigem Erfolg).

Transformationen (siehe `Migrations.cs`):
- `datetime → .NET-Ticks` in C# (`DateTime.Ticks`, Spaltentyp `Ticks(...)`)
- Tabellen-Renames `T_DokArt → t_dokumentart`, `T_Rolle → t_rolleinausschreibung`
- Spalten-Renames `AusRolId → ausriaid`, `VesOnlyWorkingdays → vesnurwerktage`
- Sprache: `SprNaturalId` (DE/FR/IT/EN) → Culture `de-CH/fr-CH/it-CH/en-US`
- `datetime → date`-Casts, `coalesce(AusKap3Voeb, 0)`

## Konfiguration

Reihenfolge (spätere überschreiben frühere):
`appsettings.json` → User-Secrets → Umgebungsvariablen → Kommandozeilen-Args.

Benötigt:
- `ConnectionStrings:Source` — SQL Server
- `ConnectionStrings:Target` — PostgreSQL
- optional `CommandTimeoutSeconds` (Default 600)

### Entwicklung (Windows) — User-Secrets

```powershell
dotnet user-secrets set "ConnectionStrings:Source" "Server=localhost;Database=DoeA_Source;Trusted_Connection=True;TrustServerCertificate=True;"
dotnet user-secrets set "ConnectionStrings:Target" "Host=localhost;Port=5432;Database=doea;Username=postgres;Password=dev"
dotnet run
```

### Produktiv (Linux) — Umgebungsvariablen

```bash
export ConnectionStrings__Source="Server=sqlsrv;Database=DoeA;User Id=...;Password=...;TrustServerCertificate=True;"
export ConnectionStrings__Target="Host=pg;Port=5432;Database=doea;Username=...;Password=..."
dotnet DataMigration.dll
```

Oder per Args:
```bash
dotnet DataMigration.dll --ConnectionStrings:Source="..." --ConnectionStrings:Target="..."
```

> **Hinweis:** `appsettings.json` ist bewusst leer; Connection-Strings gehören in User Secrets oder Umgebungsvariablen und werden nicht committet.
