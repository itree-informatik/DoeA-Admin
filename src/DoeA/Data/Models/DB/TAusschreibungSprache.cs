// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Sprachenmatrix einer Ausschreibung: verknüpft Ausschreibung, Dokumentart und Sprache, um festzuhalten, welche Dokumentart in welcher Sprache publiziert wird.</summary>
[Table("T_AusschreibungSprache")]
public partial class TAusschreibungSprache
{
    [Key]
    public Guid AspId { get; set; }

    public Guid AspSprId { get; set; }

    public Guid AspAusId { get; set; }

    public Guid AspDoaId { get; set; }

    [StringLength(255)]
    public string? AspNaturalId { get; set; }

    public bool AspAktiv { get; set; }

    public long AspInsertedDate { get; set; }

    [StringLength(255)]
    public string AspInsertedUser { get; set; } = null!;

    public long AspChangedDate { get; set; }

    [StringLength(255)]
    public string AspChangedUser { get; set; } = null!;

    [ForeignKey("AspAusId")]
    [InverseProperty("TAusschreibungSprache")]
    public virtual TAusschreibung AspAus { get; set; } = null!;

    [ForeignKey("AspDoaId")]
    [InverseProperty("TAusschreibungSprache")]
    public virtual TDokumentart AspDoa { get; set; } = null!;

    [ForeignKey("AspSprId")]
    [InverseProperty("TAusschreibungSprache")]
    public virtual TSprache AspSpr { get; set; } = null!;
}
