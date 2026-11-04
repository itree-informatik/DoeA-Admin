// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>n:m-Verknüpfung zwischen Exportgruppe und Leistungsart (Leistungsart-Filter des Exports).</summary>
[Table("T_ExportgruppeLeistungsart")]
public partial class TExportgruppeLeistungsart
{
    [Key]
    public Guid ExlId { get; set; }

    public Guid ExlExpId { get; set; }

    public Guid ExlLeaId { get; set; }

    [StringLength(255)]
    public string? ExlNaturalId { get; set; }

    public bool ExlAktiv { get; set; }

    public long ExlInsertedDate { get; set; }

    [StringLength(255)]
    public string ExlInsertedUser { get; set; } = null!;

    public long ExlChangedDate { get; set; }

    [StringLength(255)]
    public string ExlChangedUser { get; set; } = null!;

    [ForeignKey("ExlExpId")]
    [InverseProperty("TExportgruppeLeistungsart")]
    public virtual TExportgruppe ExlExp { get; set; } = null!;

    [ForeignKey("ExlLeaId")]
    [InverseProperty("TExportgruppeLeistungsart")]
    public virtual TLeistungsart ExlLea { get; set; } = null!;
}
