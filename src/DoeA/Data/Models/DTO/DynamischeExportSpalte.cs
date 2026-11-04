// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

namespace DoeA.Data.Models.DTO;

/// <summary>
/// Zusätzliche, zur Laufzeit bestimmte Spalte für den generischen Excel-Export
/// (<see cref="Interfaces.IExportToExcelService"/>): der Titel kommt fertig übersetzt vom
/// Aufrufer (keine Translation-Key-Auflösung), der Wert wird pro Zeile geliefert und als Text
/// geschrieben, ganze Zahlen (<c>int</c>) als Zahl. Wird z.B. für die variable Anzahl
/// Berichtigungs-Spalten im Exportgruppen-Gesamtexport verwendet.
/// </summary>
public class DynamischeExportSpalte<T>
{
    public required string Titel { get; init; }
    public required Func<T, object?> Wert { get; init; }
}
