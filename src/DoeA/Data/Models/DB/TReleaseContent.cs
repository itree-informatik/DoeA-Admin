// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Release-Notes-Eintrag (Typ, Titel, Text) zu einem Release; im Schema vorhanden, in der Applikation derzeit ohne Verwendung.</summary>
[Table("T_ReleaseContent")]
public partial class TReleaseContent
{
    [Key]
    public Guid RecId { get; set; }

    public Guid RecRelId { get; set; }

    [StringLength(255)]
    public string RecTyp { get; set; } = null!;

    [StringLength(255)]
    public string RecTitel { get; set; } = null!;

    [StringLength(4000)]
    public string RecText { get; set; } = null!;
}
