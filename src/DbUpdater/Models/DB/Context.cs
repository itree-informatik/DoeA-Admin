// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DbUpdater.Models.DB;

/// <summary>EF-Context des DbUpdaters für dbo.schemaversions und t_translation; Objektnamen werden wie in DoeA kleingeschrieben.</summary>
public class Context : DbContext
{
    public Context() { }

    public Context(DbContextOptions<Context> options) : base(options) { }

    public virtual DbSet<SchemaVersions> SchemaVersion { get; set; } = null!;
    public virtual DbSet<Translation> Translation { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder opt)
    {
        opt.UseNpgsql(Helper.ConnectionString, opts => opts.CommandTimeout(Helper.CommandTimeout));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("dbo");

        modelBuilder.Entity<SchemaVersions>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK_SchemaVersions_Id");

            // Npgsql mappt DateTime sonst auf 'timestamp with time zone' (nur UTC erlaubt).
            // Spalte ist 'timestamp' (ohne Zeitzone) -> passt zu DateTime.Now (Kind=Local).
            entity.Property(e => e.Applied).HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<Translation>(entity =>
        {
            entity.HasKey(e => e.TrlId).HasName("PK_Translation");

            entity.Property(e => e.TrlId).ValueGeneratedNever();
        });

        // PostgreSQL ist case-sensitiv (Npgsql quotet Bezeichner). Die DDL-Skripte legen alle
        // Objekte kleingeschrieben an, daher hier alle Tabellen-/Spalten-/Key-Namen kleinschreiben,
        // damit EF dieselben Objekte trifft (z. B. dbo."T_Translation" -> dbo.t_translation).
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.GetTableName()?.ToLowerInvariant());
            entity.SetSchema(entity.GetSchema()?.ToLowerInvariant());

            var storeObject = StoreObjectIdentifier.Table(entity.GetTableName()!, entity.GetSchema());
            foreach (var property in entity.GetProperties())
            {
                var column = property.GetColumnName(storeObject);
                if (column != null)
                    property.SetColumnName(column.ToLowerInvariant());
            }

            foreach (var key in entity.GetKeys())
                key.SetName(key.GetName()?.ToLowerInvariant());

            foreach (var fk in entity.GetForeignKeys())
                fk.SetConstraintName(fk.GetConstraintName()?.ToLowerInvariant());

            foreach (var index in entity.GetIndexes())
                index.SetDatabaseName(index.GetDatabaseName()?.ToLowerInvariant());
        }
    }
}
