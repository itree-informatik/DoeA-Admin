// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DbUpdater.Models.DB;

/// <summary>Journal der ausgeführten Skripte (dbo.schemaversions), von DbUp geschrieben und hier nur gelesen.</summary>
[Table("SchemaVersions")]
public class SchemaVersions
{
    [Key]
    public int Id { get; set; }
    public string? ScriptName { get; set; }
    public DateTime Applied { get; set; }

}
