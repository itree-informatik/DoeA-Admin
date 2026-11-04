// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Los einer Ausschreibung mit Zustand (offen, abgebrochen, abgeschlossen, widerrufen) und Abbruchdatum; Träger der Offerten und BKP-Nummern.</summary>
[Table("T_Los")]
public partial class TLos
{
    [Key]
    public Guid LosId { get; set; }

    public Guid LosAusId { get; set; }

    public DateOnly? LosAbbruchdatum { get; set; }

    public DateOnly? LosEingereicht { get; set; }

    public bool LosVerzoegerungDurchAuftraggeber { get; set; }

    [StringLength(255)]
    public string LosBezeichnung { get; set; } = null!;

    [StringLength(255)]
    public string LosZustand { get; set; } = null!;

    [StringLength(255)]
    public string? LosNaturalId { get; set; }

    public bool LosAktiv { get; set; }

    public long LosInsertedDate { get; set; }

    [StringLength(255)]
    public string LosInsertedUser { get; set; } = null!;

    public long LosChangedDate { get; set; }

    [StringLength(255)]
    public string LosChangedUser { get; set; } = null!;

    [ForeignKey("LosAusId")]
    [InverseProperty("TLos")]
    public virtual TAusschreibung LosAus { get; set; } = null!;

    [InverseProperty("LbnLos")]
    public virtual ICollection<TLosBkpNummer> TLosBkpNummer { get; set; } = new List<TLosBkpNummer>();

    [InverseProperty("OffLos")]
    public virtual ICollection<TOfferte> TOfferte { get; set; } = new List<TOfferte>();
}
