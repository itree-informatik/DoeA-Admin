// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>n:m-Verknüpfung zwischen Exportgruppe und Ressort (Ressort-Filter des Exports).</summary>
[Table("T_ExportgruppeRessort")]
public partial class TExportgruppeRessort
{
    [Key]
    public Guid EreId { get; set; }

    public Guid EreExpId { get; set; }

    public Guid EreResId { get; set; }

    [StringLength(255)]
    public string? EreNaturalId { get; set; }

    public bool EreAktiv { get; set; }

    public long EreInsertedDate { get; set; }

    [StringLength(255)]
    public string EreInsertedUser { get; set; } = null!;

    public long EreChangedDate { get; set; }

    [StringLength(255)]
    public string EreChangedUser { get; set; } = null!;

    [ForeignKey("EreExpId")]
    [InverseProperty("TExportgruppeRessort")]
    public virtual TExportgruppe EreExp { get; set; } = null!;

    [ForeignKey("EreResId")]
    [InverseProperty("TExportgruppeRessort")]
    public virtual TRessort EreRes { get; set; } = null!;
}
