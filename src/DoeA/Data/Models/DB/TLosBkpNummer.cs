// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>n:m-Verknüpfung zwischen Los und BKP-Nummer.</summary>
[Table("T_LosBkpNummer")]
public partial class TLosBkpNummer
{
    [Key]
    public Guid LbnId { get; set; }

    public Guid LbnLosId { get; set; }

    public Guid LbnBkpId { get; set; }

    [StringLength(255)]
    public string? LbnNaturalId { get; set; }

    public bool LbnAktiv { get; set; }

    public long LbnInsertedDate { get; set; }

    [StringLength(255)]
    public string LbnInsertedUser { get; set; } = null!;

    public long LbnChangedDate { get; set; }

    [StringLength(255)]
    public string LbnChangedUser { get; set; } = null!;

    [ForeignKey("LbnBkpId")]
    [InverseProperty("TLosBkpNummer")]
    public virtual TBkpNummer LbnBkp { get; set; } = null!;

    [ForeignKey("LbnLosId")]
    [InverseProperty("TLosBkpNummer")]
    public virtual TLos LbnLos { get; set; } = null!;
}
