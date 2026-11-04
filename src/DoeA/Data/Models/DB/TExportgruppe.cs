// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Gespeicherte Exportgruppe: benannte Filterkombination (Jahr, Publikationszeitraum, Kennzeichen, Spaltenauswahl) für den Datenexport von Ausschreibungen; die Filterlisten liegen in den TExportgruppe*-Verknüpfungen.</summary>
[Table("T_Exportgruppe")]
public partial class TExportgruppe
{
    [Key]
    public Guid ExpId { get; set; }

    [StringLength(1000)]
    public string ExpName { get; set; } = null!;

    public int? ExpJahr { get; set; }

    public int? ExpAbschlussJahr { get; set; }

    public bool? ExpPersonalverleih { get; set; }

    public bool? ExpAbgeschlossen { get; set; }

    public bool? ExpBeschaffungDezentral { get; set; }

    public DateOnly? ExpPublikationVon { get; set; }

    public DateOnly? ExpPublikationBis { get; set; }

    public string? ExpSpalten { get; set; }

    [StringLength(255)]
    public string? ExpNaturalId { get; set; }

    public bool ExpAktiv { get; set; }

    public long ExpInsertedDate { get; set; }

    [StringLength(255)]
    public string ExpInsertedUser { get; set; } = null!;

    public long ExpChangedDate { get; set; }

    [StringLength(255)]
    public string ExpChangedUser { get; set; } = null!;

    [InverseProperty("ExaExp")]
    public virtual ICollection<TExportgruppeAmt> TExportgruppeAmt { get; set; } = new List<TExportgruppeAmt>();

    [InverseProperty("ExlExp")]
    public virtual ICollection<TExportgruppeLeistungsart> TExportgruppeLeistungsart { get; set; } = new List<TExportgruppeLeistungsart>();

    [InverseProperty("EkaExp")]
    public virtual ICollection<TExportgruppeKanton> TExportgruppeKanton { get; set; } = new List<TExportgruppeKanton>();

    [InverseProperty("EprExp")]
    public virtual ICollection<TExportgruppeProjektart> TExportgruppeProjektart { get; set; } = new List<TExportgruppeProjektart>();

    [InverseProperty("EreExp")]
    public virtual ICollection<TExportgruppeRessort> TExportgruppeRessort { get; set; } = new List<TExportgruppeRessort>();

    [InverseProperty("ExrExp")]
    public virtual ICollection<TExportgruppeRolleInAusschreibung> TExportgruppeRolleInAusschreibung { get; set; } = new List<TExportgruppeRolleInAusschreibung>();

    [InverseProperty("ExsExp")]
    public virtual ICollection<TExportgruppeSachbearbeiter> TExportgruppeSachbearbeiter { get; set; } = new List<TExportgruppeSachbearbeiter>();

    [InverseProperty("EvaExp")]
    public virtual ICollection<TExportgruppeVerfahrensart> TExportgruppeVerfahrensart { get; set; } = new List<TExportgruppeVerfahrensart>();
}
