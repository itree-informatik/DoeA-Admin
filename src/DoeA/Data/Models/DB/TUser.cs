// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Benutzer aus dem eIAM: UsrExtId ist der Claim "sub", UsrAktiv steuert die Autorisierung in DoeA.</summary>
[Table("T_User")]
public partial class TUser
{
    [Key]
    public Guid UsrId { get; set; }

    [StringLength(255)]
    public string UsrExtId { get; set; } = null!;

    [StringLength(255)]
    public string UsrVorname { get; set; } = null!;

    [StringLength(255)]
    public string UsrNachname { get; set; } = null!;

    [StringLength(255)]
    public string UsrEMail { get; set; } = null!;

    [StringLength(10)]
    public string UsrSprache { get; set; } = null!;

    /// <summary>Letzte erfolgreiche Anmeldung als DateTime.Ticks; null, solange der Benutzer nie freigeschaltet war.</summary>
    public long? UsrLastLogin { get; set; }

    public bool UsrAktiv { get; set; }
}
