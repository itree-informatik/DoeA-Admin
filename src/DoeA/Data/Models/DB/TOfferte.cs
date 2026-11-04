// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Offerte eines Anbieters auf ein Los mit Zuschlagswert und Mitarbeiterzahl; ihren Zustandsverlauf hält TOfferteOffertezustand.</summary>
[Table("T_Offerte")]
public partial class TOfferte
{
    [Key]
    public Guid OffId { get; set; }

    public Guid OffLosId { get; set; }

    public Guid OffAnbId { get; set; }

    [StringLength(4000)]
    public string? OffBemerkung { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? OffZuschlagswertChf { get; set; }

    public int? OffAnzahlMitarbeiter { get; set; }

    [StringLength(255)]
    public string? OffNaturalId { get; set; }

    public bool OffAktiv { get; set; }

    public long OffInsertedDate { get; set; }

    [StringLength(255)]
    public string OffInsertedUser { get; set; } = null!;

    public long OffChangedDate { get; set; }

    [StringLength(255)]
    public string OffChangedUser { get; set; } = null!;

    [ForeignKey("OffAnbId")]
    [InverseProperty("TOfferte")]
    public virtual TAnbieter OffAnb { get; set; } = null!;

    [ForeignKey("OffLosId")]
    [InverseProperty("TOfferte")]
    public virtual TLos OffLos { get; set; } = null!;

    [InverseProperty("OozOff")]
    public virtual ICollection<TOfferteOffertezustand> TOfferteOffertezustand { get; set; } = new List<TOfferteOffertezustand>();
}
