// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>n:m-Verknüpfung zwischen Ausschreibung und CPV-Nummer.</summary>
[Table("T_AusschreibungCpvNummer")]
public partial class TAusschreibungCpvNummer
{
    [Key]
    public Guid AcnId { get; set; }

    public Guid AcnAusId { get; set; }

    public Guid AcnCpvId { get; set; }

    [StringLength(255)]
    public string? AcnNaturalId { get; set; }

    public bool AcnAktiv { get; set; }

    public long AcnInsertedDate { get; set; }

    [StringLength(255)]
    public string AcnInsertedUser { get; set; } = null!;

    public long AcnChangedDate { get; set; }

    [StringLength(255)]
    public string AcnChangedUser { get; set; } = null!;

    [ForeignKey("AcnAusId")]
    [InverseProperty("TAusschreibungCpvNummer")]
    public virtual TAusschreibung AcnAus { get; set; } = null!;

    [ForeignKey("AcnCpvId")]
    [InverseProperty("TAusschreibungCpvNummer")]
    public virtual TCpvNummer AcnCpv { get; set; } = null!;
}
