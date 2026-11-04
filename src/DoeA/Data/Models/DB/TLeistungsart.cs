// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Leistungsart als Stammdatum, einer Projektart untergeordnet; wird Ausschreibungen zugeordnet und dient als Exportfilter.</summary>
[Table("T_Leistungsart")]
public partial class TLeistungsart
{
    [Key]
    public Guid LeaId { get; set; }

    public Guid LeaPraId { get; set; }

    [StringLength(255)]
    public string LeaBezeichnung { get; set; } = null!;

    [StringLength(255)]
    public string LeaKurzbezeichnung { get; set; } = null!;

    [StringLength(255)]
    public string? LeaNaturalId { get; set; }

    public bool LeaAktiv { get; set; }

    public long LeaInsertedDate { get; set; }

    [StringLength(255)]
    public string LeaInsertedUser { get; set; } = null!;

    public long LeaChangedDate { get; set; }

    [StringLength(255)]
    public string LeaChangedUser { get; set; } = null!;

    [ForeignKey("LeaPraId")]
    [InverseProperty("TLeistungsart")]
    public virtual TProjektart LeaPra { get; set; } = null!;

    [InverseProperty("AusLea")]
    public virtual ICollection<TAusschreibung> TAusschreibung { get; set; } = new List<TAusschreibung>();

    [InverseProperty("ExlLea")]
    public virtual ICollection<TExportgruppeLeistungsart> TExportgruppeLeistungsart { get; set; } = new List<TExportgruppeLeistungsart>();
}
