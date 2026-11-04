// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>n:m-Verknüpfung zwischen Exportgruppe und Amt (Amt-Filter des Exports).</summary>
[Table("T_ExportgruppeAmt")]
public partial class TExportgruppeAmt
{
    [Key]
    public Guid ExaId { get; set; }

    public Guid ExaExpId { get; set; }

    public Guid ExaAmtId { get; set; }

    [StringLength(255)]
    public string? ExaNaturalId { get; set; }

    public bool ExaAktiv { get; set; }

    public long ExaInsertedDate { get; set; }

    [StringLength(255)]
    public string ExaInsertedUser { get; set; } = null!;

    public long ExaChangedDate { get; set; }

    [StringLength(255)]
    public string ExaChangedUser { get; set; } = null!;

    [ForeignKey("ExaAmtId")]
    [InverseProperty("TExportgruppeAmt")]
    public virtual TAmt ExaAmt { get; set; } = null!;

    [ForeignKey("ExaExpId")]
    [InverseProperty("TExportgruppeAmt")]
    public virtual TExportgruppe ExaExp { get; set; } = null!;
}
