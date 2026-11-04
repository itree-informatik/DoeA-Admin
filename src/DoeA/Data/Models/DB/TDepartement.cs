// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Departement als Stammdatum; oberste Stufe der Organisationshierarchie über den Ämtern.</summary>
[Table("T_Departement")]
public partial class TDepartement
{
    [Key]
    public Guid DepId { get; set; }

    [StringLength(255)]
    public string DepBezeichnung { get; set; } = null!;

    [StringLength(255)]
    public string DepKurzbezeichnung { get; set; } = null!;

    [StringLength(255)]
    public string? DepNaturalId { get; set; }

    public bool DepAktiv { get; set; }

    public long DepInsertedDate { get; set; }

    [StringLength(255)]
    public string DepInsertedUser { get; set; } = null!;

    public long DepChangedDate { get; set; }

    [StringLength(255)]
    public string DepChangedUser { get; set; } = null!;

    [InverseProperty("AmtDep")]
    public virtual ICollection<TAmt> TAmt { get; set; } = new List<TAmt>();
}
