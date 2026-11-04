// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Verfahrensart (offen, selektiv, freihändig, …) als Stammdatum; wird Ausschreibungen zugeordnet und bestimmt in den Auswertungen die massgebliche Publikation (bei selektiv die Präqualifikation).</summary>
[Table("T_Verfahrensart")]
public partial class TVerfahrensart
{
    [Key]
    public Guid VeaId { get; set; }

    [StringLength(255)]
    public string VeaBezeichnung { get; set; } = null!;

    [StringLength(255)]
    public string VeaKurzbezeichnung { get; set; } = null!;

    public int VeaSortierung { get; set; }

    [StringLength(255)]
    public string? VeaNaturalId { get; set; }

    public bool VeaAktiv { get; set; }

    public long VeaInsertedDate { get; set; }

    [StringLength(255)]
    public string VeaInsertedUser { get; set; } = null!;

    public long VeaChangedDate { get; set; }

    [StringLength(255)]
    public string VeaChangedUser { get; set; } = null!;

    [InverseProperty("AusVea")]
    public virtual ICollection<TAusschreibung> TAusschreibung { get; set; } = new List<TAusschreibung>();

    [InverseProperty("EvaVea")]
    public virtual ICollection<TExportgruppeVerfahrensart> TExportgruppeVerfahrensart { get; set; } = new List<TExportgruppeVerfahrensart>();
}
