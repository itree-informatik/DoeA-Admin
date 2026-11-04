// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Sachbearbeiter mit Organisationszugehörigkeit (RD, DoeA, BBL) als Stammdatum; wird Ausschreibungen je Organisationseinheit zugeordnet und dient als Exportfilter.</summary>
[Table("T_Sachbearbeiter")]
public partial class TSachbearbeiter
{
    [Key]
    public Guid SacId { get; set; }

    public Guid SacOrgId { get; set; }

    [StringLength(255)]
    public string SacNachname { get; set; } = null!;

    [StringLength(255)]
    public string SacVorname { get; set; } = null!;

    [StringLength(255)]
    public string SacKurzbezeichnung { get; set; } = null!;

    [StringLength(255)]
    public string? SacNaturalId { get; set; }

    public bool SacAktiv { get; set; }

    public long SacInsertedDate { get; set; }

    [StringLength(255)]
    public string SacInsertedUser { get; set; } = null!;

    public long SacChangedDate { get; set; }

    [StringLength(255)]
    public string SacChangedUser { get; set; } = null!;

    [ForeignKey("SacOrgId")]
    [InverseProperty("TSachbearbeiter")]
    public virtual TOrganisationszugehoerigkeit SacOrg { get; set; } = null!;

    [InverseProperty("AusSacBbl")]
    public virtual ICollection<TAusschreibung> TAusschreibungAusSacBbl { get; set; } = new List<TAusschreibung>();

    [InverseProperty("AusSacDoeA")]
    public virtual ICollection<TAusschreibung> TAusschreibungAusSacDoeA { get; set; } = new List<TAusschreibung>();

    [InverseProperty("AusSacRd")]
    public virtual ICollection<TAusschreibung> TAusschreibungAusSacRd { get; set; } = new List<TAusschreibung>();

    [InverseProperty("ExsSac")]
    public virtual ICollection<TExportgruppeSachbearbeiter> TExportgruppeSachbearbeiter { get; set; } = new List<TExportgruppeSachbearbeiter>();
}
