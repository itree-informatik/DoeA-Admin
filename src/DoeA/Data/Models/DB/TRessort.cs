// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Ressort als Stammdatum, einem Amt untergeordnet; wird Ausschreibungen zugeordnet und dient als Exportfilter.</summary>
[Table("T_Ressort")]
public partial class TRessort
{
    [Key]
    public Guid ResId { get; set; }

    public Guid ResAmtId { get; set; }

    [StringLength(255)]
    public string ResBezeichnung { get; set; } = null!;

    [StringLength(255)]
    public string ResKurzbezeichnung { get; set; } = null!;

    [StringLength(255)]
    public string? ResNaturalId { get; set; }

    public bool ResAktiv { get; set; }

    public long ResInsertedDate { get; set; }

    [StringLength(255)]
    public string ResInsertedUser { get; set; } = null!;

    public long ResChangedDate { get; set; }

    [StringLength(255)]
    public string ResChangedUser { get; set; } = null!;

    [ForeignKey("ResAmtId")]
    [InverseProperty("TRessort")]
    public virtual TAmt ResAmt { get; set; } = null!;

    [InverseProperty("AusRes")]
    public virtual ICollection<TAusschreibung> TAusschreibung { get; set; } = new List<TAusschreibung>();

    [InverseProperty("EreRes")]
    public virtual ICollection<TExportgruppeRessort> TExportgruppeRessort { get; set; } = new List<TExportgruppeRessort>();
}
