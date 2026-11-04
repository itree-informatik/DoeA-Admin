// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>n:m-Verknüpfung zwischen Exportgruppe und Sachbearbeiter (Sachbearbeiter-Filter des Exports); die Rolle (DoeA/RD/BBL, wie <c>EnumOrganisationszugehoerigkeit</c>) bestimmt, auf welches Sachbearbeiter-Feld der Ausschreibung der Filter wirkt.</summary>
[Table("T_ExportgruppeSachbearbeiter")]
public partial class TExportgruppeSachbearbeiter
{
    [Key]
    public Guid ExsId { get; set; }

    public Guid ExsExpId { get; set; }

    public Guid ExsSacId { get; set; }

    [StringLength(10)]
    public string ExsRolle { get; set; } = null!;

    [StringLength(255)]
    public string? ExsNaturalId { get; set; }

    public bool ExsAktiv { get; set; }

    public long ExsInsertedDate { get; set; }

    [StringLength(255)]
    public string ExsInsertedUser { get; set; } = null!;

    public long ExsChangedDate { get; set; }

    [StringLength(255)]
    public string ExsChangedUser { get; set; } = null!;

    [ForeignKey("ExsExpId")]
    [InverseProperty("TExportgruppeSachbearbeiter")]
    public virtual TExportgruppe ExsExp { get; set; } = null!;

    [ForeignKey("ExsSacId")]
    [InverseProperty("TExportgruppeSachbearbeiter")]
    public virtual TSachbearbeiter ExsSac { get; set; } = null!;
}
