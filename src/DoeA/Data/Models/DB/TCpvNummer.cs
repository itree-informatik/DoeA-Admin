// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>CPV-Nummer (Common Procurement Vocabulary) als Stammdatum; wird Ausschreibungen zugeordnet.</summary>
[Table("T_CpvNummer")]
public partial class TCpvNummer
{
    [Key]
    public Guid CpvId { get; set; }

    [StringLength(255)]
    public string CpvNummerStr { get; set; } = null!;

    [StringLength(255)]
    public string CpvBezeichnung { get; set; } = null!;

    public int CpvNummer { get; set; }

    [StringLength(255)]
    public string? CpvNaturalId { get; set; }

    public bool CpvAktiv { get; set; }

    public long CpvInsertedDate { get; set; }

    [StringLength(255)]
    public string CpvInsertedUser { get; set; } = null!;

    public long CpvChangedDate { get; set; }

    [StringLength(255)]
    public string CpvChangedUser { get; set; } = null!;

    [InverseProperty("AcnCpv")]
    public virtual ICollection<TAusschreibungCpvNummer> TAusschreibungCpvNummer { get; set; } = new List<TAusschreibungCpvNummer>();
}
