// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Zustandsverlauf einer Offerte: verknüpft Offerte und Offertezustand mit dem Datum des Zustandswechsels.</summary>
[Table("T_OfferteOffertezustand")]
public partial class TOfferteOffertezustand
{
    [Key]
    public Guid OozId { get; set; }

    public Guid OozOffId { get; set; }

    public Guid OozOfzId { get; set; }

    public DateOnly OozDatum { get; set; }

    public DateOnly? OozEingereicht { get; set; }

    public bool OozVerzoegerungDurchAuftraggeber { get; set; }

    [StringLength(255)]
    public string? OozNaturalId { get; set; }

    public bool OozAktiv { get; set; }

    public long OozInsertedDate { get; set; }

    [StringLength(255)]
    public string OozInsertedUser { get; set; } = null!;

    public long OozChangedDate { get; set; }

    [StringLength(255)]
    public string OozChangedUser { get; set; } = null!;

    [ForeignKey("OozOffId")]
    [InverseProperty("TOfferteOffertezustand")]
    public virtual TOfferte OozOff { get; set; } = null!;

    [ForeignKey("OozOfzId")]
    [InverseProperty("TOfferteOffertezustand")]
    public virtual TOffertezustand OozOfz { get; set; } = null!;
}
