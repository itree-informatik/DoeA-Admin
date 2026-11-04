// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Land als Stammdatum; wird Ortschaften zugeordnet.</summary>
[Table("T_Land")]
public partial class TLand
{
    [Key]
    public Guid LanId { get; set; }

    [StringLength(255)]
    public string LanBezeichnung { get; set; } = null!;

    [StringLength(255)]
    public string LanKurzbezeichnung { get; set; } = null!;

    [StringLength(255)]
    public string? LanNaturalId { get; set; }

    public bool LanAktiv { get; set; }

    public long LanInsertedDate { get; set; }

    [StringLength(255)]
    public string LanInsertedUser { get; set; } = null!;

    public long LanChangedDate { get; set; }

    [StringLength(255)]
    public string LanChangedUser { get; set; } = null!;

    [InverseProperty("OrtLan")]
    public virtual ICollection<TOrtschaft> TOrtschaft { get; set; } = new List<TOrtschaft>();
}
