// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>BKP-Nummer (Baukostenplan) als Stammdatum; wird Losen zugeordnet.</summary>
[Table("T_BkpNummer")]
[Index("BkpNummerStr", Name = "UQ_T_BkpNummer_BkpNummerStr", IsUnique = true)]
public partial class TBkpNummer
{
    [Key]
    public Guid BkpId { get; set; }

    [StringLength(255)]
    public string BkpNummerStr { get; set; } = null!;

    [StringLength(255)]
    public string BkpBezeichnung { get; set; } = null!;

    public int BkpNummer { get; set; }

    [StringLength(255)]
    public string? BkpNaturalId { get; set; }

    public bool BkpAktiv { get; set; }

    public long BkpInsertedDate { get; set; }

    [StringLength(255)]
    public string BkpInsertedUser { get; set; } = null!;

    public long BkpChangedDate { get; set; }

    [StringLength(255)]
    public string BkpChangedUser { get; set; } = null!;

    [InverseProperty("LbnBkp")]
    public virtual ICollection<TLosBkpNummer> TLosBkpNummer { get; set; } = new List<TLosBkpNummer>();
}
