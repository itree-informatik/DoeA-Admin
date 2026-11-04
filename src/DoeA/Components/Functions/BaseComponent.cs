// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DTO;
using ItreeMud.Components;
using ItreeMud.Models;
using DoeA.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DoeA.Components.Functions;

/// <summary>Gemeinsame Basis der Razor-Komponenten: Fehlerbehandlung, Grid-Auswahlfilter und JS-Helfer für Zwischenablage, Download und Fokus.</summary>
public class BaseComponent : ComponentBase
{
    /// <summary>Zentrale Fehlerbehandlung, über die Komponenten Service-Aufrufe ausführen und Fehler melden.</summary>
    [Inject]
    protected IErrorHandlerService ErrorHandler { get; set; } = default!;

    /// <summary>Baut aus einer DTO-Liste eine <see cref="ItreeFormDropdown"/>; Wert ist die Id, Text die Bezeichnung oder <paramref name="customName"/>.</summary>
    public static ItreeFormDropdown CreateItreeFormDropdown<T>(IList<T> list, string field, string translationKey, Func<T, string>? customName = null)
    {
        var dropdown = new ItreeFormDropdown
        {
            Data = new List<ItreeFormDropdownItem>(),
            Field = field,
            TranslationKey = translationKey
        };

        foreach (var entry in list)
        {
            if (entry == null)
                throw new InvalidCastException("Entry is null");

            var dropdownItem = new ItreeFormDropdownItem();

            if (customName == null)
            {
                dropdownItem.Text = entry.GetType().GetProperty("Bezeichnung")?.GetValue(entry)?.ToString();
            }
            else
            {
                dropdownItem.Text = customName(entry);
            }

            var id = entry.GetType().GetProperty("Id")?.GetValue(entry)?.ToString();

            if (string.IsNullOrEmpty(id))
                throw new InvalidCastException("Id not found");

            dropdownItem.Value = Guid.Parse(id);
            dropdown.Data.Add(dropdownItem);
        }
        return dropdown;
    }

    /// <summary>Einträge einer <c>ItreeFilterCheckboxList</c>: Schlüssel ist die Id, angezeigt wird <paramref name="text"/>.</summary>
    protected static Dictionary<string, string> FilterEintraege<T>(IEnumerable<T> eintraege, Func<T, string> text) where T : Basis =>
        eintraege.ToDictionary(e => e.Id.ToString(), text);

    /// <summary>
    /// Filtert eine Grid-Spalte auf die in einer <c>ItreeFilterCheckboxList</c> gewählten Schlüssel (Oder-verknüpft) und schliesst
    /// das Filtermenü; ohne Auswahl wird der Filter der Spalte entfernt. Gleiches Vorgehen wie MultiFilterAsync in ItreeBlazor.
    /// </summary>
    protected static async Task MultiFilterAsync(ItreeGridFilterContext filterContext, string fieldName, IList<string> selectedKeys,
        Func<string, object>? valueConverter = null)
    {
        filterContext.FilterDescriptor.FilterDescriptors.Clear();

        if (selectedKeys.Count > 0)
        {
            var oder = new ItreeCompositeFilterDescriptor { LogicalOperator = ItreeFilterLogicalOperator.Or };
            foreach (var key in selectedKeys)
            {
                oder.FilterDescriptors.Add(new ItreeFilterDescriptor(fieldName, ItreeFilterOperator.IsEqualTo, valueConverter == null ? key : valueConverter(key)));
            }

            filterContext.FilterDescriptor.FilterDescriptors.Add(oder);
        }

        await filterContext.FilterAsync();
    }

    /// <summary>Entfernt den Auswahlfilter einer Grid-Spalte, leert die Checkbox-Liste und schliesst das Filtermenü.</summary>
    protected static async Task MultiClearFilterAsync(ItreeGridFilterContext filterContext, List<string> selectedKeys)
    {
        selectedKeys.Clear();
        await filterContext.ClearFilterAsync();
    }

    /// <summary>Kopiert den Text in die Zwischenablage des Browsers.</summary>
    protected async Task CopyToClipboard(IJSRuntime jsRuntime, string content)
    {
        await jsRuntime.InvokeVoidAsync("CopyToClipboard", content);
    }

    /// <summary>Lädt die im DownloadStorageService abgelegte Datei über die Route <c>/download/{fileId}</c> (Program.cs) herunter.</summary>
    protected async Task DownloadFileFromStream(IJSRuntime jsRuntime, Guid fileId)
    {
        await jsRuntime.InvokeVoidAsync("DownloadFileFromUrl", $"/download/{fileId}");
    }

    /// <summary>Fokussiert das Element mit der Id; mit <paramref name="selectElement"/> wird zusätzlich sein Inhalt selektiert.</summary>
    protected async Task FocusElementById(IJSRuntime jsRuntime, string id, bool selectElement = false)
    {
        await jsRuntime.InvokeVoidAsync("FocusElementById", id, selectElement);
    }
}