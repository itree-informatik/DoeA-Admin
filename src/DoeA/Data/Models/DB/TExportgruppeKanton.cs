// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>n:m-Verknüpfung zwischen Exportgruppe und Kanton (Kanton-Filter des Exports).</summary>
[Table("T_ExportgruppeKanton")]
public partial class TExportgruppeKanton
{
    [Key]
    public Guid EkaId { get; set; }

    public Guid EkaExpId { get; set; }

    public Guid EkaKanId { get; set; }

    [StringLength(255)]
    public string? EkaNaturalId { get; set; }

    public bool EkaAktiv { get; set; }

    public long EkaInsertedDate { get; set; }

    [StringLength(255)]
    public string EkaInsertedUser { get; set; } = null!;

    public long EkaChangedDate { get; set; }

    [StringLength(255)]
    public string EkaChangedUser { get; set; } = null!;

    [ForeignKey("EkaExpId")]
    [InverseProperty("TExportgruppeKanton")]
    public virtual TExportgruppe EkaExp { get; set; } = null!;

    [ForeignKey("EkaKanId")]
    [InverseProperty("TExportgruppeKanton")]
    public virtual TKanton EkaKan { get; set; } = null!;
}
