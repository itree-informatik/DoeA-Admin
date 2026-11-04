// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Kanton als Stammdatum mit zugeordneter Sprache; wird Ortschaften zugeordnet und dient als Exportfilter.</summary>
[Table("T_Kanton")]
public partial class TKanton
{
    [Key]
    public Guid KanId { get; set; }

    public Guid KanSprId { get; set; }

    [StringLength(255)]
    public string KanBezeichnung { get; set; } = null!;

    [StringLength(255)]
    public string KanKurzbezeichnung { get; set; } = null!;

    [StringLength(255)]
    public string? KanNaturalId { get; set; }

    public bool KanAktiv { get; set; }

    public long KanInsertedDate { get; set; }

    [StringLength(255)]
    public string KanInsertedUser { get; set; } = null!;

    public long KanChangedDate { get; set; }

    [StringLength(255)]
    public string KanChangedUser { get; set; } = null!;

    [ForeignKey("KanSprId")]
    [InverseProperty("TKanton")]
    public virtual TSprache KanSpr { get; set; } = null!;

    [InverseProperty("EkaKan")]
    public virtual ICollection<TExportgruppeKanton> TExportgruppeKanton { get; set; } = new List<TExportgruppeKanton>();

    [InverseProperty("OrtKan")]
    public virtual ICollection<TOrtschaft> TOrtschaft { get; set; } = new List<TOrtschaft>();
}
