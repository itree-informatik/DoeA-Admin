// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Projektart (z.B. Bau, Logistik) als Stammdatum; gliedert die Leistungsarten, wird Ausschreibungen zugeordnet und dient als Exportfilter.</summary>
[Table("T_Projektart")]
public partial class TProjektart
{
    [Key]
    public Guid PraId { get; set; }

    [StringLength(255)]
    public string PraBezeichnung { get; set; } = null!;

    [StringLength(255)]
    public string PraKurzbezeichnung { get; set; } = null!;

    [StringLength(255)]
    public string? PraNaturalId { get; set; }

    public bool PraAktiv { get; set; }

    public long PraInsertedDate { get; set; }

    [StringLength(255)]
    public string PraInsertedUser { get; set; } = null!;

    public long PraChangedDate { get; set; }

    [StringLength(255)]
    public string PraChangedUser { get; set; } = null!;

    [InverseProperty("AusPra")]
    public virtual ICollection<TAusschreibung> TAusschreibung { get; set; } = new List<TAusschreibung>();

    [InverseProperty("EprPra")]
    public virtual ICollection<TExportgruppeProjektart> TExportgruppeProjektart { get; set; } = new List<TExportgruppeProjektart>();

    [InverseProperty("LeaPra")]
    public virtual ICollection<TLeistungsart> TLeistungsart { get; set; } = new List<TLeistungsart>();
}
