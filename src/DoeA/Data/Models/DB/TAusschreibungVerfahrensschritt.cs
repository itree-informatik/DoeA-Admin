// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Terminplanung einer Ausschreibung: verknüpft Ausschreibung und Verfahrensschritt mit dem Zeitraum von/bis.</summary>
[Table("T_AusschreibungVerfahrensschritt")]
public partial class TAusschreibungVerfahrensschritt
{
    [Key]
    public Guid AvsId { get; set; }

    public Guid AvsAusId { get; set; }

    public Guid AvsVesId { get; set; }

    public DateOnly? AvsDatumVon { get; set; }

    public DateOnly? AvsDatumBis { get; set; }

    [StringLength(255)]
    public string? AvsNaturalId { get; set; }

    public bool AvsAktiv { get; set; }

    public long AvsInsertedDate { get; set; }

    [StringLength(255)]
    public string AvsInsertedUser { get; set; } = null!;

    public long AvsChangedDate { get; set; }

    [StringLength(255)]
    public string AvsChangedUser { get; set; } = null!;

    [ForeignKey("AvsAusId")]
    [InverseProperty("TAusschreibungVerfahrensschritt")]
    public virtual TAusschreibung AvsAus { get; set; } = null!;

    [ForeignKey("AvsVesId")]
    [InverseProperty("TAusschreibungVerfahrensschritt")]
    public virtual TVerfahrensschritt AvsVes { get; set; } = null!;
}
