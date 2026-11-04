// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Release-Stand der Applikation (Version, Datum), dem die Übersetzungen zugeordnet werden; die DbUp-Skripte legen die Einträge an.</summary>
[Table("T_Release")]
public partial class TRelease
{
    [Key]
    public Guid RelId { get; set; }

    [StringLength(255)]
    public string RelVersion { get; set; } = null!;

    public DateOnly RelDatum { get; set; }

    [InverseProperty("TrlRel")]
    public virtual ICollection<TTranslation> TTranslation { get; set; } = new List<TTranslation>();
}
