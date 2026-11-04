// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using ItreeMud.Models;

namespace DoeA.Data.Models.DTO.Exportgruppen;

/// <summary>
/// Gespeicherte, benannte Filterdefinition über den Ausschreibungsbestand.
/// Die Id-Listen sind die einzige Repräsentation der n:m-Filter (Verknüpfungstabellen
/// T_Exportgruppe*) — sie werden vom <c>ExportgruppeService</c> beim Laden befüllt und
/// beim Speichern gegen die Verknüpfungstabellen abgeglichen.
/// </summary>
public class Exportgruppe : Basis
{
    public string Name { get; set; } = null!;
    public int? Jahr { get; set; }
    public int? AbschlussJahr { get; set; }
    public bool? Personalverleih { get; set; }
    public bool? Abgeschlossen { get; set; }
    public bool? BeschaffungDezentral { get; set; }

    /// <summary>Publikationsdatum-Bereich; von/bis sind einzeln nutzbar («alle ab» bzw. «alle bis»).</summary>
    public DateOnly? PublikationVon { get; set; }
    public DateOnly? PublikationBis { get; set; }

    /// <summary>Sichtbare Ergebnisspalten (kommaseparierte Property-Namen; null = Default), siehe <see cref="ExportgruppeSpalten"/>.</summary>
    public string? Spalten { get; set; }

    public List<Guid> AmtIds { get; set; } = new();
    public List<Guid> KantonIds { get; set; } = new();
    public List<Guid> LeistungsartIds { get; set; } = new();
    public List<Guid> ProjektartIds { get; set; } = new();
    public List<Guid> RessortIds { get; set; } = new();
    public List<Guid> RolleInAusschreibungIds { get; set; } = new();
    public List<Guid> SachbearbeiterDoeAIds { get; set; } = new();
    public List<Guid> SachbearbeiterRdIds { get; set; } = new();
    public List<Guid> SachbearbeiterBblIds { get; set; } = new();
    public List<Guid> VerfahrensartIds { get; set; } = new();
}
