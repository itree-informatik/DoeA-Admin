// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Publikationssprache mit Culture-Code (z.B. «de-CH») als Stammdatum; Achse der Sprachenmatrix und Sprache der Kantone.</summary>
[Table("T_Sprache")]
public partial class TSprache
{
    [Key]
    public Guid SprId { get; set; }

    [StringLength(255)]
    public string SprCulture { get; set; } = null!;

    [StringLength(255)]
    public string SprBezeichnung { get; set; } = null!;

    [StringLength(255)]
    public string SprNaturalId { get; set; } = null!;

    public int SprSortierung { get; set; }

    public bool SprAktiv { get; set; }

    public long SprInsertedDate { get; set; }

    [StringLength(255)]
    public string SprInsertedUser { get; set; } = null!;

    public long SprChangedDate { get; set; }

    [StringLength(255)]
    public string SprChangedUser { get; set; } = null!;

    [InverseProperty("AspSpr")]
    public virtual ICollection<TAusschreibungSprache> TAusschreibungSprache { get; set; } = new List<TAusschreibungSprache>();

    [InverseProperty("KanSpr")]
    public virtual ICollection<TKanton> TKanton { get; set; } = new List<TKanton>();
}
