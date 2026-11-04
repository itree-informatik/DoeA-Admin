// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Bundesamt als Stammdatum; Ämter gehören zu einem Departement, gliedern sich in Ressorts und werden Ausschreibungen zugeordnet.</summary>
[Table("T_Amt")]
public partial class TAmt
{
    [Key]
    public Guid AmtId { get; set; }

    public Guid AmtDepId { get; set; }

    [StringLength(255)]
    public string AmtBezeichnung { get; set; } = null!;

    [StringLength(255)]
    public string AmtKurzbezeichnung { get; set; } = null!;

    [StringLength(255)]
    public string AmtDienststellennummer { get; set; } = null!;

    [StringLength(255)]
    public string? AmtNaturalId { get; set; }

    public bool AmtAktiv { get; set; }

    public long AmtInsertedDate { get; set; }

    [StringLength(255)]
    public string AmtInsertedUser { get; set; } = null!;

    public long AmtChangedDate { get; set; }

    [StringLength(255)]
    public string AmtChangedUser { get; set; } = null!;

    [ForeignKey("AmtDepId")]
    [InverseProperty("TAmt")]
    public virtual TDepartement AmtDep { get; set; } = null!;

    [InverseProperty("AusAmt")]
    public virtual ICollection<TAusschreibung> TAusschreibung { get; set; } = new List<TAusschreibung>();

    [InverseProperty("ExaAmt")]
    public virtual ICollection<TExportgruppeAmt> TExportgruppeAmt { get; set; } = new List<TExportgruppeAmt>();

    [InverseProperty("ResAmt")]
    public virtual ICollection<TRessort> TRessort { get; set; } = new List<TRessort>();
}
