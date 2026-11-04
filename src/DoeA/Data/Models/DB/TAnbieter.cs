// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Anbieter (Firma mit Adresse und Ortschaft) als Stammdatum; reicht Offerten auf Lose ein.</summary>
[Table("T_Anbieter")]
public partial class TAnbieter
{
    [Key]
    public Guid AnbId { get; set; }

    public Guid AnbOrtId { get; set; }

    [StringLength(255)]
    public string AnbName { get; set; } = null!;

    [StringLength(255)]
    public string AnbAdresse { get; set; } = null!;

    [StringLength(255)]
    public string? AnbAdresse2 { get; set; }

    [StringLength(255)]
    public string? AnbAdresse3 { get; set; }

    [StringLength(255)]
    public string? AnbNaturalId { get; set; }

    public bool AnbAktiv { get; set; }

    public long AnbInsertedDate { get; set; }

    [StringLength(255)]
    public string AnbInsertedUser { get; set; } = null!;

    public long AnbChangedDate { get; set; }

    [StringLength(255)]
    public string AnbChangedUser { get; set; } = null!;

    [ForeignKey("AnbOrtId")]
    [InverseProperty("TAnbieter")]
    public virtual TOrtschaft AnbOrt { get; set; } = null!;

    [InverseProperty("OffAnb")]
    public virtual ICollection<TOfferte> TOfferte { get; set; } = new List<TOfferte>();
}
