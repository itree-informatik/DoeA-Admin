// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Berichtigung einer publizierten Ausschreibung mit Einreichungs- und Publikationsdatum sowie Verzögerungs-Kennzeichen für die Zielerreichung.</summary>
[Table("T_Berichtigung")]
public partial class TBerichtigung
{
    [Key]
    public Guid BerId { get; set; }

    public Guid BerAusId { get; set; }

    public DateOnly? BerEingereicht { get; set; }

    public DateOnly? BerPubliziert { get; set; }

    public bool BerVerzoegerungDurchAuftraggeber { get; set; }

    [StringLength(4000)]
    public string? BerBemerkungen { get; set; }

    [StringLength(255)]
    public string? BerNaturalId { get; set; }

    public bool BerAktiv { get; set; }

    public long BerInsertedDate { get; set; }

    [StringLength(255)]
    public string BerInsertedUser { get; set; } = null!;

    public long BerChangedDate { get; set; }

    [StringLength(255)]
    public string BerChangedUser { get; set; } = null!;

    [ForeignKey("BerAusId")]
    [InverseProperty("TBerichtigung")]
    public virtual TAusschreibung BerAus { get; set; } = null!;
}
