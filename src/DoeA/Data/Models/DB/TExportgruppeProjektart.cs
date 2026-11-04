// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>n:m-Verknüpfung zwischen Exportgruppe und Projektart (Projektart-Filter des Exports).</summary>
[Table("T_ExportgruppeProjektart")]
public partial class TExportgruppeProjektart
{
    [Key]
    public Guid EprId { get; set; }

    public Guid EprExpId { get; set; }

    public Guid EprPraId { get; set; }

    [StringLength(255)]
    public string? EprNaturalId { get; set; }

    public bool EprAktiv { get; set; }

    public long EprInsertedDate { get; set; }

    [StringLength(255)]
    public string EprInsertedUser { get; set; } = null!;

    public long EprChangedDate { get; set; }

    [StringLength(255)]
    public string EprChangedUser { get; set; } = null!;

    [ForeignKey("EprExpId")]
    [InverseProperty("TExportgruppeProjektart")]
    public virtual TExportgruppe EprExp { get; set; } = null!;

    [ForeignKey("EprPraId")]
    [InverseProperty("TExportgruppeProjektart")]
    public virtual TProjektart EprPra { get; set; } = null!;
}
