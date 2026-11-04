// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Zustand einer Offerte (z.B. erfasst, abgelehnt) als Stammdatum; das Kennzeichen Schlusszustand markiert Endzustände, die beim Abschliessen eines Loses gesetzt werden.</summary>
[Table("T_Offertezustand")]
public partial class TOffertezustand
{
    [Key]
    public Guid OfzId { get; set; }

    [StringLength(255)]
    public string OfzBezeichnung { get; set; } = null!;

    [StringLength(255)]
    public string OfzKurzbezeichnung { get; set; } = null!;

    public bool OfzSchlusszustand { get; set; }

    public bool OfzPublikation { get; set; }

    [StringLength(255)]
    public string? OfzNaturalId { get; set; }

    public bool OfzAktiv { get; set; }

    public long OfzInsertedDate { get; set; }

    [StringLength(255)]
    public string OfzInsertedUser { get; set; } = null!;

    public long OfzChangedDate { get; set; }

    [StringLength(255)]
    public string OfzChangedUser { get; set; } = null!;

    [InverseProperty("OozOfz")]
    public virtual ICollection<TOfferteOffertezustand> TOfferteOffertezustand { get; set; } = new List<TOfferteOffertezustand>();
}
