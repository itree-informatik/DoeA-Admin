// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;

namespace DoeA.Components.Layout;

/// <summary>Hauptnavigation mit aufklappbaren Bereichen; die Aktiv-Markierung folgt der aktuellen URL.</summary>
public partial class NavMenu : ComponentBase, IDisposable
{
    [Inject]
    private INavigationService? NavigationService { get; set; }

    [Inject]
    private NavigationManager? NavigationManager { get; set; }

    private bool _collapsed;

    /// <summary>Einträge der obersten Menüebene mit ihren Untereinträgen.</summary>
    protected List<NavMenuItem> MenuItems = new();

    private void ToggleCollapsed() => _collapsed = !_collapsed;

    protected override void OnInitialized()
    {
        if (NavigationService == null)
        {
            throw new InvalidOperationException($"{nameof(NavigationService)} has not been initialized.");
        }

        MenuItems.Add(new NavMenuItem { Text = "MENU_AUSSCHREIBUNGEN", Url = "/", Id = "ausschreibungen", Icon = Icons.Material.Filled.Mail, IsRoot = true});
        MenuItems.Add(new NavMenuItem
        {
            Text = "MENU_EINSTELLUNGEN",
            Id = "einstellungen",
            Icon = Icons.Material.Filled.Settings,
            Children = new List<NavMenuItem>
            {
                new NavMenuItem { Text = "MENU_EINSTELLUNGEN_LOG", Url = "einstellungen/log", Id = "log" },
                new NavMenuItem { Text = "MENU_EINSTELLUNGEN_KONFIGURATION", Url = "einstellungen/config", Id = "config" },
                new NavMenuItem { Text = "MENU_EINSTELLUNGEN_TRANSLATIONS", Url = "einstellungen/translations", Id = "translations" },
                new NavMenuItem { Text = "MENU_EINSTELLUNGEN_SPRACHE", Url = "einstellungen/sprache", Id = "sprache" },
                new NavMenuItem { Text = "MENU_EINSTELLUNGEN_BENUTZER", Url = "einstellungen/benutzer", Id = "benutzer" },
            }, IsRoot = true
        });
        MenuItems.Add(new NavMenuItem
        {
            Text = "MENU_STAMMDATEN",
            Id = "stammdaten",
            Icon = Icons.Material.Filled.Description,
            Children = new List<NavMenuItem>
            {
                new NavMenuItem { Text = "MENU_STAMMDATEN_PROJEKT", Url = "stammdaten/projekt", Id = "projekt" },
                new NavMenuItem { Text = "MENU_STAMMDATEN_GEOGRAPHISCHE", Url = "/stammdaten/geographische", Id = "geographische" },
                new NavMenuItem { Text = "MENU_STAMMDATEN_ORGANISATION", Url = "/stammdaten/organisation", Id = "organisation" },
                new NavMenuItem { Text = "MENU_STAMMDATEN_GENERELL", Url = "/stammdaten/generell", Id = "generell" },
                new NavMenuItem { Text = "MENU_STAMMDATEN_ANBIETER", Url = "/stammdaten/anbieter", Id = "anbieter" }
            }, IsRoot = true
        });

        MenuItems.Add(new NavMenuItem { Text = "MENU_AUSWERTUNGEN", Url = "auswertungen", Id = "auswertungen", Icon = Icons.Material.Filled.BarChart, IsRoot = true });
        MenuItems.Add(new NavMenuItem { Text = "MENU_EXPORTGRUPPEN", Url = "exportgruppen", Id = "exportgruppen", Icon = Icons.Material.Filled.Output, IsRoot = true });

        // Aktiv-Markierung folgt der URL (auch bei Navigationen ausserhalb des Menüs,
        // z.B. Exportgruppe -> Ausschreibung, und bei Deep-Links).
        NavigationManager!.LocationChanged += OnLocationChanged;
        SetActiveFromUrl();
    }

    /// <summary>Klappt den angeklickten Bereich auf bzw. zu und markiert Einträge ohne Url direkt als aktiv.</summary>
    protected void ToggleItem(NavMenuItem item)
    {
        // Im reduzierten Menü öffnet ein Klick auf eine Gruppe zuerst wieder die volle Ansicht,
        // damit die Untereinträge sichtbar werden.
        if (_collapsed && item.Children is { Count: > 0 })
        {
            _collapsed = false;
            MenuItems.ForEach(m => m.IsExpanded = false);
            item.IsExpanded = true;
            SetActive(item);
            return;
        }

        MenuItems.ForEach(m => m.IsExpanded = false);
        item.IsExpanded = !item.IsExpanded;

        // Menüpunkte ohne URL (aufklappbare Bereiche) lösen keine Navigation aus ->
        // Markierung direkt setzen; bei Links übernimmt das OnLocationChanged.
        if (string.IsNullOrEmpty(item.Url))
        {
            SetActive(item);
        }
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        SetActiveFromUrl();
        InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// Bestimmt den aktiven Menüpunkt aus dem ersten Segment der aktuellen URL;
    /// Detailrouten (z.B. ausschreibung/{id}, exportgruppe/{id}) zählen zu ihrem Bereich.
    /// </summary>
    private void SetActiveFromUrl()
    {
        var segment = NavigationService!.GetRelativeUri().Split('/', '?')[0].ToLowerInvariant();
        var id = segment switch
        {
            "" or "ausschreibungen" or "ausschreibung" => "ausschreibungen",
            "einstellungen" => "einstellungen",
            "stammdaten" => "stammdaten",
            "auswertungen" => "auswertungen",
            "exportgruppen" or "exportgruppe" => "exportgruppen",
            _ => null
        };
        SetActive(MenuItems.FirstOrDefault(m => m.Id == id));
    }

    private void SetActive(NavMenuItem? item)
    {
        MenuItems.ForEach(m => m.Class = string.Empty);
        if (item == null)
        {
            return;
        }

        if (item.Children != null && item.Children.Any())
        {
            MenuItems.ForEach(m => m.Class2 = "not-active");
        }
        else
        {
            MenuItems.ForEach(m => m.Class2 = string.Empty);
        }

        item.Class = "active";
    }

    /// <summary>Meldet den LocationChanged-Handler wieder ab.</summary>
    public void Dispose()
    {
        if (NavigationManager != null)
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
        }
    }
}

/// <summary>Eintrag des Navigationsmenüs inklusive allfälliger Untereinträge.</summary>
public class NavMenuItem
{
    /// <summary>Übersetzungsschlüssel der Beschriftung.</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>Ziel-Url; leer bei reinen Gruppen-Einträgen.</summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>Routen-Kennung, über die der aktive Eintrag ermittelt wird.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>MudBlazor-Icon des Eintrags; leer bei Untereinträgen ohne Icon.</summary>
    public string Icon { get; set; } = string.Empty;

    /// <summary>CSS-Klasse für die Aktiv-Markierung.</summary>
    public string Class { get; set; } = string.Empty;

    /// <summary>CSS-Klasse zum Abblenden der nicht aktiven Einträge.</summary>
    public string Class2 { get; set; } = string.Empty;

    /// <summary>Untereinträge; <c>null</c> bei Einträgen ohne Untermenü.</summary>
    public List<NavMenuItem>? Children { get; set; }

    /// <summary>Kennzeichnet Einträge der obersten Menüebene.</summary>
    public bool IsRoot { get; set; }

    /// <summary>Steuert, ob die Untereinträge aufgeklappt sind.</summary>
    public bool IsExpanded { get; set; }
}
