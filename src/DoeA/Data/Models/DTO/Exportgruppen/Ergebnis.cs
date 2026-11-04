// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using ItreeMud.Models;

namespace DoeA.Data.Models.DTO.Exportgruppen;

/// <summary>
/// Flache Ergebniszeile der Filterausführung einer Exportgruppe — eine Zeile pro Ausschreibung
/// (<c>Id</c> = AusId). Wird ausschliesslich per LINQ-Projektion befüllt (kein Mapper).
/// Achtung Namensgebung: Der Klassenname ist Teil der Übersetzungsschlüssel des Excel-Exports
/// (<c>EXPORTGRUPPE_ERGEBNIS_{PROPERTY}</c>, siehe ExportToExcelService) — bei Umbenennung
/// müssen die Translation-Keys mitziehen.
/// </summary>
public class Ergebnis : Basis
{
    public int Jahr { get; set; }
    public string NummerStr { get; set; } = string.Empty;
    public string Bezeichnung { get; set; } = string.Empty;
    public string? SimapNr { get; set; }
    public string? Amt { get; set; }
    public string? Ressort { get; set; }
    public string? Leistungsart { get; set; }
    public string Projektart { get; set; } = string.Empty;
    public string Verfahrensart { get; set; } = string.Empty;
    public string? RolleInAusschreibung { get; set; }
    /// <summary>Kantone der Offerten-Anbieter (Kurzbezeichnungen, distinct, kommasepariert) — wird nach der Projektion befüllt.</summary>
    public string? Kanton { get; set; }
    public string? SachbearbeiterDoeA { get; set; }
    public string? SachbearbeiterRd { get; set; }
    public string? SachbearbeiterBbl { get; set; }
    public DateOnly? Publikation { get; set; }
    public DateOnly? OffertenFrist { get; set; }
    public bool Abgeschlossen { get; set; }
    public string? Extern { get; set; }
    public bool Personalverleih { get; set; }
    public bool BeschaffungDezentral { get; set; }
}
