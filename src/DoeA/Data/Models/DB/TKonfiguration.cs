// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Konfigurationseintrag mit typisiertem Wert (z.B. Publikationsfrist und minimale Zielerreichung für die Auswertungen); der Code spricht ihn über die NaturalId an.</summary>
[Table("T_Konfiguration")]
public partial class TKonfiguration
{
    [Key]
    public Guid KonId { get; set; }

    [StringLength(255)]
    public string KonBezeichnung { get; set; } = null!;

    public string KonWert { get; set; } = null!;

    [StringLength(50)]
    public string KonTyp { get; set; } = null!;

    [StringLength(1000)]
    public string KonBeschreibung { get; set; } = null!;

    [StringLength(255)]
    public string KonNaturalId { get; set; } = null!;

    public bool KonAktiv { get; set; }

    public long KonInsertedDate { get; set; }

    [StringLength(255)]
    public string KonInsertedUser { get; set; } = null!;

    public long KonChangedDate { get; set; }

    [StringLength(255)]
    public string KonChangedUser { get; set; } = null!;
}
