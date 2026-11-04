// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Logeintrag, den Serilog über den PostgreSQL-Sink schreibt; DoeA liest die Tabelle nur für die Log-Ansicht in den Einstellungen.</summary>
[Table("T_Log")]
public partial class TLog
{
    [Key]
    public int Id { get; set; }

    public string? Message { get; set; }

    public string? MessageTemplate { get; set; }

    public string? Level { get; set; }

    // Npgsql mappt DateTime sonst auf 'timestamp with time zone' (nur UTC erlaubt);
    // die Spalte ist 'timestamp' ohne Zeitzone und wird von Serilog mit Lokalzeit geschrieben.
    [Column(TypeName = "timestamp without time zone")]
    public DateTime? TimeStamp { get; set; }

    public string? Exception { get; set; }

    public string? Properties { get; set; }
}
