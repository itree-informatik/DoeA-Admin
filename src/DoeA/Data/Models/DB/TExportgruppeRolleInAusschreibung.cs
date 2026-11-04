// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>n:m-Verknüpfung zwischen Exportgruppe und Rolle in Ausschreibung (Rollen-Filter des Exports).</summary>
[Table("T_ExportgruppeRolleInAusschreibung")]
public partial class TExportgruppeRolleInAusschreibung
{
    [Key]
    public Guid ExrId { get; set; }

    public Guid ExrExpId { get; set; }

    public Guid ExrRiaId { get; set; }

    [StringLength(255)]
    public string? ExrNaturalId { get; set; }

    public bool ExrAktiv { get; set; }

    public long ExrInsertedDate { get; set; }

    [StringLength(255)]
    public string ExrInsertedUser { get; set; } = null!;

    public long ExrChangedDate { get; set; }

    [StringLength(255)]
    public string ExrChangedUser { get; set; } = null!;

    [ForeignKey("ExrExpId")]
    [InverseProperty("TExportgruppeRolleInAusschreibung")]
    public virtual TExportgruppe ExrExp { get; set; } = null!;

    [ForeignKey("ExrRiaId")]
    [InverseProperty("TExportgruppeRolleInAusschreibung")]
    public virtual TRolleInAusschreibung ExrRia { get; set; } = null!;
}
