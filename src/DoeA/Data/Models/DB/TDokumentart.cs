// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Dokumentart einer Ausschreibung als Stammdatum; bildet zusammen mit der Sprache die Sprachenmatrix (TAusschreibungSprache).</summary>
[Table("T_Dokumentart")]
public partial class TDokumentart
{
    [Key]
    public Guid DoaId { get; set; }

    [StringLength(255)]
    public string DoaBezeichnung { get; set; } = null!;

    [StringLength(255)]
    public string? DoaNaturalId { get; set; }

    public int DoaSortierung { get; set; }

    public bool DoaAktiv { get; set; }

    public long DoaInsertedDate { get; set; }

    [StringLength(255)]
    public string DoaInsertedUser { get; set; } = null!;

    public long DoaChangedDate { get; set; }

    [StringLength(255)]
    public string DoaChangedUser { get; set; } = null!;

    [InverseProperty("AspDoa")]
    public virtual ICollection<TAusschreibungSprache> TAusschreibungSprache { get; set; } = new List<TAusschreibungSprache>();
}
