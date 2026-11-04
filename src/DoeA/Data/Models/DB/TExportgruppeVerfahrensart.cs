// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>n:m-Verknüpfung zwischen Exportgruppe und Verfahrensart (Verfahrensart-Filter des Exports).</summary>
[Table("T_ExportgruppeVerfahrensart")]
public partial class TExportgruppeVerfahrensart
{
    [Key]
    public Guid EvaId { get; set; }

    public Guid EvaExpId { get; set; }

    public Guid EvaVeaId { get; set; }

    [StringLength(255)]
    public string? EvaNaturalId { get; set; }

    public bool EvaAktiv { get; set; }

    public long EvaInsertedDate { get; set; }

    [StringLength(255)]
    public string EvaInsertedUser { get; set; } = null!;

    public long EvaChangedDate { get; set; }

    [StringLength(255)]
    public string EvaChangedUser { get; set; } = null!;

    [ForeignKey("EvaExpId")]
    [InverseProperty("TExportgruppeVerfahrensart")]
    public virtual TExportgruppe EvaExp { get; set; } = null!;

    [ForeignKey("EvaVeaId")]
    [InverseProperty("TExportgruppeVerfahrensart")]
    public virtual TVerfahrensart EvaVea { get; set; } = null!;
}
