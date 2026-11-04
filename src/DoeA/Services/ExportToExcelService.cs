// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using ClosedXML.Excel;
using DoeA.Data.Models.DTO;
using DoeA.Interfaces;
using ItreeMud.Extensions;
using System.Reflection;

namespace DoeA.Services;

/// <inheritdoc cref="IExportToExcelService"/>
public class ExportToExcelService : IExportToExcelService
{
    private const string BezeichnungProperty = "Bezeichnung";
    private const string ChangedDateProperty = "ChangedDate";
    private const string IdProperty = "Id";
    private const string InsertedDateProperty = "InsertedDate";
    private const string NullableTypName = "Nullable`1";
    private const string TranslatedTextProperty = "TranslatedText";

    /// <summary>Teil des Namespaces der DTOs – daran werden verschachtelte Modelle erkannt.</summary>
    private const string ModelNamespaceTeil = "Models";

    /// <summary>Trennt im Spaltenschlüssel die Property vom Unter-Property, z.B. «Amt;Bezeichnung».</summary>
    private const char UnterPropertyTrenner = ';';

    private const string FormatDatum = "dd.MM.yyyy";
    private const string FormatDatumZeit = "dd.MM.yyyy HH:mm:ss";
    private const string FormatDezimal = "#'##0.00";
    private const string FormatGanzzahl = "0";
    private const string Tabellenblattname = "Sheet1";
    private const string TrlJa = "EXPORT_JA";
    private const string TrlNein = "EXPORT_NEIN";

    /// <summary>Nur diese Typen (und ihre Nullable-Varianten) werden automatisch als Spalte übernommen.</summary>
    private static readonly List<string> ExportierbareTypen =
    [
        typeof(string).FullName!,
        typeof(bool).FullName!,
        typeof(int).FullName!,
        typeof(decimal).FullName!,
        typeof(DateTime).FullName!,
        typeof(long).FullName!,
        typeof(DateOnly).FullName!
    ];

    private readonly IDownloadStorageService _storageService;

    public ExportToExcelService(IDownloadStorageService storageService)
    {
        _storageService = storageService;
    }

    /// <summary>
    /// Exportiert die Liste nach Excel und liefert die Download-Id.
    /// <paramref name="trlPraefix"/> setzt (falls gesetzt) eine übersetzte Kopfzeile,
    /// <paramref name="propertyList"/> wählt Spalten aus bzw. schliesst sie mit <paramref name="ignore"/> aus.
    /// </summary>
    public async Task<string> ExportToExcel<T>(string filename, IList<T>? list, string? trlPraefix = null,
        IList<string>? propertyList = null, bool ignore = false, bool includeId = false,
        IList<string>? sortedList = null, IList<DynamischeExportSpalte<T>>? zusatzSpalten = null)
    {
        var content = new List<XLCellValue[]>();
        var anzahlZusatz = zusatzSpalten?.Count ?? 0;

        if (list != null && list.Count > 0)
        {
            var model = list.FirstOrDefault();
            if (model == null)
            {
                return string.Empty;
            }

            propertyList ??= new List<string>();

            // ignore = true dreht die Bedeutung von propertyList um: Die Liste wird zur Ausschlussliste
            // und die Spalten werden anschliessend per Reflection neu ermittelt.
            IList<string>? ignorePropertyList = new List<string>();
            if (ignore)
            {
                ignorePropertyList = propertyList.Clone();
                propertyList.Clear();
            }

            if (propertyList.Count == 0)
            {
                ErgaenzeStandardSpalten(model.GetType(), propertyList, ignorePropertyList, includeId);
            }

            if (sortedList != null)
            {
                propertyList = propertyList
                    .OrderBy(p =>
                    {
                        var index = sortedList.IndexOf(p);
                        return index >= 0 ? index : int.MaxValue; // Nicht aufgeführte Spalten wandern ans Ende
                    })
                    .ToList();
            }

            if (trlPraefix != null)
            {
                content.Add(ErstelleKopfzeile(model.GetType(), propertyList, trlPraefix, zusatzSpalten, anzahlZusatz));
            }

            foreach (var row in list)
            {
                content.Add(ErstelleDatenzeile(row, propertyList, zusatzSpalten, anzahlZusatz));
            }
        }

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(Tabellenblattname);

        // Der Spaltentyp hängt nur von der Spalte ab – einmal ermitteln statt je Zelle per Reflection.
        var spaltenTypen = content.Count > 0
            ? ErmittleSpaltenTypen(list![0]!.GetType().GetProperties(), propertyList!)
            : Array.Empty<string?>();

        for (var rowCount = 0; rowCount < content.Count; rowCount++)
        {
            for (var colCount = 0; colCount < content[0].Length; colCount++)
            {
                var cell = worksheet.Cell(rowCount + 1, colCount + 1);
                cell.SetValue(content[rowCount][colCount]);

                // Dynamische Zusatzspalten bekommen kein Typ-Format
                if (colCount < spaltenTypen.Length)
                {
                    SetzeZahlenformat(cell, spaltenTypen[colCount]);
                }
            }
        }

        // Nur formatieren, wenn tatsächlich eine Kopfzeile geschrieben wurde: bei leerer Ergebnisliste
        // wird der Block oben übersprungen, propertyList bleibt dann null und das Blatt leer.
        var spaltenAnzahl = (propertyList?.Count ?? 0) + anzahlZusatz;
        if (trlPraefix != null && content.Count > 0 && spaltenAnzahl > 0)
        {
            worksheet.Range(1, 1, 1, spaltenAnzahl).Style.Font.Bold = true;
        }

        worksheet.Columns().AdjustToContents();

        await using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return _storageService.StoreFile(ms.ToArray(), filename);
    }

    /// <summary>
    /// Ermittelt die Exportspalten per Reflection: alle einfachen Typen sowie verschachtelte Modelle,
    /// die einen Anzeigetext besitzen. Guids (Fremdschlüssel) sind für den Leser wertlos und entfallen.
    /// </summary>
    private static void ErgaenzeStandardSpalten(Type modelType, IList<string> propertyList,
        IList<string>? ignorePropertyList, bool includeId)
    {
        foreach (var propertyInfo in modelType.GetProperties())
        {
            if (ignorePropertyList != null && ignorePropertyList.Contains(propertyInfo.Name))
            {
                continue;
            }

            if (includeId && propertyInfo.Name == IdProperty)
            {
                propertyList.Add(propertyInfo.Name);
                continue;
            }

            var typName = propertyInfo.PropertyType.FullName;
            if (typName == null || typName.Contains(typeof(Guid).FullName!))
            {
                continue;
            }

            if (typName.Contains(ModelNamespaceTeil))
            {
                var anzeigeProperty = propertyInfo.PropertyType.GetProperties()
                    .FirstOrDefault(p => p.Name == TranslatedTextProperty)?.Name
                    ?? propertyInfo.PropertyType.GetProperties()
                        .FirstOrDefault(p => p.Name == BezeichnungProperty)?.Name;

                if (!string.IsNullOrEmpty(anzeigeProperty))
                {
                    propertyList.Add($"{propertyInfo.Name}{UnterPropertyTrenner}{anzeigeProperty}");
                }

                continue;
            }

            if (ExportierbareTypen.Contains(typName))
            {
                propertyList.Add(propertyInfo.Name);
                continue;
            }

            if (propertyInfo.PropertyType.Name == NullableTypName
                && ExportierbareTypen.Any(typ => typName.Contains(typ)))
            {
                propertyList.Add(propertyInfo.Name);
            }
        }
    }

    /// <summary>Baut die übersetzte Kopfzeile; die Titel der Zusatzspalten kommen fertig übersetzt vom Aufrufer.</summary>
    private static XLCellValue[] ErstelleKopfzeile<T>(Type modelType, IList<string> propertyList, string trlPraefix,
        IList<DynamischeExportSpalte<T>>? zusatzSpalten, int anzahlZusatz)
    {
        var rowContent = new XLCellValue[propertyList.Count + anzahlZusatz];
        var colCount = 0;

        foreach (var col in propertyList)
        {
            var property = col.Split(UnterPropertyTrenner)[0];
            rowContent[colCount++] = TranslationHelper.Translate(
                $"{trlPraefix}_{modelType.Name.ToUpper()}_{property.ToUpper()}");
        }

        for (var zusatz = 0; zusatz < anzahlZusatz; zusatz++)
        {
            rowContent[colCount++] = zusatzSpalten![zusatz].Titel;
        }

        return rowContent;
    }

    /// <summary>Baut eine Datenzeile aus den Spaltenschlüsseln und hängt die Zusatzspalten an.</summary>
    private static XLCellValue[] ErstelleDatenzeile<T>(T row, IList<string> propertyList,
        IList<DynamischeExportSpalte<T>>? zusatzSpalten, int anzahlZusatz)
    {
        var rowContent = new XLCellValue[propertyList.Count + anzahlZusatz];
        var colCount = 0;

        foreach (var col in propertyList)
        {
            rowContent[colCount++] = ErmittleZellenwert(row!, col);
        }

        for (var zusatz = 0; zusatz < anzahlZusatz; zusatz++)
        {
            rowContent[colCount++] = ZusatzZellenwert(zusatzSpalten![zusatz].Wert(row));
        }

        return rowContent;
    }

    private static XLCellValue ZusatzZellenwert(object? wert) => wert switch
    {
        int zahl => zahl,
        null => string.Empty,
        _ => wert.ToString() ?? string.Empty
    };

    /// <summary>Liest den Wert einer Spalte aus dem Datensatz und bereitet ihn für die Anzeige auf.</summary>
    private static XLCellValue ErmittleZellenwert(object row, string col)
    {
        var split = col.Split(UnterPropertyTrenner);
        var property = row.GetType().GetProperty(split[0]);
        if (property == null || property.GetValue(row) == null)
        {
            return string.Empty;
        }

        object? value;
        if (split.Length > 1)
        {
            var subModel = property.GetValue(row);
            if (subModel == null)
            {
                return string.Empty;
            }

            value = subModel.GetType().GetProperty(split[1])?.GetValue(subModel);
            return value?.ToString() ?? string.Empty;
        }

        value = property.GetValue(row);

        // Nur ausdrücklich markierte Spalten als Zahl, damit sich die übrigen Exporte nicht ändern.
        if (value is int zahl && property.IsDefined(typeof(ExportAlsZahlAttribute)))
        {
            return zahl;
        }

        var typName = property.PropertyType.FullName!;

        if (typName == typeof(bool).FullName || typName == typeof(bool?).FullName)
        {
            value = value?.ToString()?.ToLower() == "true"
                ? TranslationHelper.Translate(TrlJa)
                : TranslationHelper.Translate(TrlNein);
        }

        // Datum ohne Uhrzeit ausgeben, wenn keine Uhrzeit erfasst ist
        if (typName.Contains(typeof(DateTime).FullName!) && value != null && value.ToString()!.EndsWith("00:00:00"))
        {
            value = value.ToString()![..10];
        }

        // Audit-Zeitstempel liegen gemäss Datums-Konvention als Ticks in der DB
        if (typName == typeof(long).FullName && (col == InsertedDateProperty || col == ChangedDateProperty)
            && long.TryParse(value?.ToString(), out var ticks))
        {
            value = new DateTime(ticks).ToString(FormatDatumZeit);
        }

        return value?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Ordnet jeder Spalte den Typnamen ihrer Property zu; die als Ticks abgelegten
    /// Audit-Zeitstempel werden dabei als Datum behandelt.
    /// </summary>
    private static string?[] ErmittleSpaltenTypen(PropertyInfo[] properties, IList<string> propertyList)
    {
        var typen = new string?[propertyList.Count];
        for (var i = 0; i < propertyList.Count; i++)
        {
            foreach (var propertyInfo in properties)
            {
                if ((propertyInfo.Name == ChangedDateProperty && propertyList[i] == ChangedDateProperty)
                    || (propertyInfo.Name == InsertedDateProperty && propertyList[i] == InsertedDateProperty))
                {
                    typen[i] = typeof(DateTime).FullName;
                    break;
                }

                if (propertyInfo.Name == propertyList[i])
                {
                    typen[i] = propertyInfo.PropertyType.FullName;
                    break;
                }
            }
        }

        return typen;
    }

    private static void SetzeZahlenformat(IXLCell cell, string? typName)
    {
        if (typName == null)
        {
            return;
        }

        if (typName.Contains(typeof(decimal).FullName!))
        {
            cell.Style.NumberFormat.Format = FormatDezimal;
        }

        if (typName.Contains(typeof(DateOnly).FullName!))
        {
            cell.Style.NumberFormat.Format = FormatDatum;
        }

        if (typName.Contains(typeof(DateTime).FullName!))
        {
            cell.Style.NumberFormat.Format = FormatDatumZeit;
        }

        if (typName.Contains(typeof(int).FullName!) || typName.Contains(typeof(long).FullName!))
        {
            cell.Style.NumberFormat.Format = FormatGanzzahl;
        }
    }
}
