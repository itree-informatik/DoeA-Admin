// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Ortschaft (PLZ/Ort) mit Land und optionalem Kanton als Stammdatum; Adressbestandteil der Anbieter.</summary>
[Table("T_Ortschaft")]
public partial class TOrtschaft
{
    [Key]
    public Guid OrtId { get; set; }

    public Guid OrtLanId { get; set; }

    public Guid? OrtKanId { get; set; }

    [StringLength(255)]
    public string OrtOrt { get; set; } = null!;

    [StringLength(255)]
    public string OrtPlz { get; set; } = null!;

    [StringLength(255)]
    public string? OrtNaturalId { get; set; }

    public bool OrtAktiv { get; set; }

    public long OrtInsertedDate { get; set; }

    [StringLength(255)]
    public string OrtInsertedUser { get; set; } = null!;

    public long OrtChangedDate { get; set; }

    [StringLength(255)]
    public string OrtChangedUser { get; set; } = null!;

    [ForeignKey("OrtKanId")]
    [InverseProperty("TOrtschaft")]
    public virtual TKanton? OrtKan { get; set; }

    [ForeignKey("OrtLanId")]
    [InverseProperty("TOrtschaft")]
    public virtual TLand OrtLan { get; set; } = null!;

    [InverseProperty("AnbOrt")]
    public virtual ICollection<TAnbieter> TAnbieter { get; set; } = new List<TAnbieter>();
}
