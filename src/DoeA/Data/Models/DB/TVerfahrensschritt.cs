// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Verfahrensschritt der Terminplanung als Stammdatum (Sortierung, nur Werktage, gilt für offenes bzw. selektives Verfahren); die Termine je Ausschreibung liegen in TAusschreibungVerfahrensschritt.</summary>
[Table("T_Verfahrensschritt")]
public partial class TVerfahrensschritt
{
    [Key]
    public Guid VesId { get; set; }

    [StringLength(255)]
    public string VesBezeichnung { get; set; } = null!;

    public int VesSortierung { get; set; }

    public bool VesNurWerktage { get; set; }

    public bool VesOffen { get; set; }

    public bool VesSelektiv { get; set; }

    [StringLength(255)]
    public string? VesNaturalId { get; set; }

    public bool VesAktiv { get; set; }

    public long VesInsertedDate { get; set; }

    [StringLength(255)]
    public string VesInsertedUser { get; set; } = null!;

    public long VesChangedDate { get; set; }

    [StringLength(255)]
    public string VesChangedUser { get; set; } = null!;

    [InverseProperty("AvsVes")]
    public virtual ICollection<TAusschreibungVerfahrensschritt> TAusschreibungVerfahrensschritt { get; set; } = new List<TAusschreibungVerfahrensschritt>();
}
