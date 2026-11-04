// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Übersetzungsschlüssel mit Texten in Deutsch, Französisch und Italienisch, einem Release zugeordnet; wird beim Start einmalig vom TranslationHelper geladen.</summary>
[Table("T_Translation")]
[Index("TrlKey", Name = "UQ_T_Translation_TrlKey", IsUnique = true)]
public partial class TTranslation
{
    [Key]
    public Guid TrlId { get; set; }

    [StringLength(255)]
    public string TrlKey { get; set; } = null!;

    [StringLength(4000)]
    public string TrlDeutsch { get; set; } = null!;

    [StringLength(4000)]
    public string? TrlFranzoesisch { get; set; }

    [StringLength(4000)]
    public string? TrlItalienisch { get; set; }

    public Guid TrlRelId { get; set; }

    [ForeignKey("TrlRelId")]
    [InverseProperty("TTranslation")]
    public virtual TRelease TrlRel { get; set; } = null!;
}
