// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Organisationseinheit der Sachbearbeiter (RD, DoeA, BBL) als Stammdatum; im Code über EnumOrganisationszugehoerigkeit verankert, weil die Ausschreibung je Einheit einen eigenen Sachbearbeiter führt.</summary>
[Table("T_Organisationszugehoerigkeit")]
public partial class TOrganisationszugehoerigkeit
{
    [Key]
    public Guid OrgId { get; set; }

    [StringLength(255)]
    public string OrgBezeichnung { get; set; } = null!;

    [StringLength(255)]
    public string OrgKurzbezeichnung { get; set; } = null!;

    [StringLength(255)]
    public string? OrgNaturalId { get; set; }

    public bool OrgAktiv { get; set; }

    public long OrgInsertedDate { get; set; }

    [StringLength(255)]
    public string OrgInsertedUser { get; set; } = null!;

    public long OrgChangedDate { get; set; }

    [StringLength(255)]
    public string OrgChangedUser { get; set; } = null!;

    [InverseProperty("SacOrg")]
    public virtual ICollection<TSachbearbeiter> TSachbearbeiter { get; set; } = new List<TSachbearbeiter>();
}
