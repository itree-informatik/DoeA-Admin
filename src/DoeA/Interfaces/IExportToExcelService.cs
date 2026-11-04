// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DTO;

namespace DoeA.Interfaces;

/// <summary>Erzeugt Excel-Exporte aus beliebigen DTO-Listen (Spalten werden per Reflection ermittelt).</summary>
public interface IExportToExcelService
{
    /// <summary>
    /// Exportiert die Liste nach Excel und liefert die Download-Id.
    /// <paramref name="trlPraefix"/> setzt (falls gesetzt) eine übersetzte Kopfzeile,
    /// <paramref name="propertyList"/> wählt Spalten aus bzw. schliesst sie mit <paramref name="ignore"/> aus.
    /// </summary>
    Task<string> ExportToExcel<T>(string filename, IList<T>? list, string? trlPraefix = null,
        IList<string>? propertyList = null, bool ignore = false, bool includeId = false,
        IList<string>? sortedList = null, IList<DynamischeExportSpalte<T>>? zusatzSpalten = null);
}
