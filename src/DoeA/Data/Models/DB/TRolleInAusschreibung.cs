// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Rolle des BBL in einer Ausschreibung (z.B. Meldestelle, Vergabestelle) als Stammdatum; wird Ausschreibungen zugeordnet und dient als Exportfilter.</summary>
[Table("T_RolleInAusschreibung")]
public partial class TRolleInAusschreibung
{
    [Key]
    public Guid RiaId { get; set; }

    [StringLength(1000)]
    public string RiaBezeichnung { get; set; } = null!;

    [StringLength(255)]
    public string RiaKurzbezeichnung { get; set; } = null!;

    [StringLength(255)]
    public string? RiaNaturalId { get; set; }

    public bool RiaAktiv { get; set; }

    public long RiaInsertedDate { get; set; }

    [StringLength(255)]
    public string RiaInsertedUser { get; set; } = null!;

    public long RiaChangedDate { get; set; }

    [StringLength(255)]
    public string RiaChangedUser { get; set; } = null!;

    [InverseProperty("AusRia")]
    public virtual ICollection<TAusschreibung> TAusschreibung { get; set; } = new List<TAusschreibung>();

    [InverseProperty("ExrRia")]
    public virtual ICollection<TExportgruppeRolleInAusschreibung> TExportgruppeRolleInAusschreibung { get; set; } = new List<TExportgruppeRolleInAusschreibung>();
}
