// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

namespace DoeA.Data.Models.DTO;

/// <summary>
/// Markiert eine ganzzahlige DTO-Property, die der Excel-Export (<see cref="Interfaces.IExportToExcelService"/>)
/// als Zahl statt als Text schreibt, damit Excel damit rechnen kann (z.B. Durchschnitt der Anzahl Tage).
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ExportAlsZahlAttribute : Attribute
{
}
