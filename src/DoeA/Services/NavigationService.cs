// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Interfaces;
using Microsoft.AspNetCore.Components;

namespace DoeA.Services;

/// <inheritdoc cref="INavigationService"/>
public class NavigationService : INavigationService
{
    private readonly NavigationManager _navigationManager;

    public NavigationService(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
    }

    /// <summary>Navigiert auf die Url; leere Urls werden ignoriert.</summary>
    /// <param name="url">Ziel-Url, relativ oder absolut.</param>
    /// <param name="forceReload">Erzwingt einen vollständigen Seiten-Reload statt einer Circuit-internen Navigation.</param>
    /// <param name="replace">Ersetzt den aktuellen History-Eintrag, statt einen neuen anzulegen.</param>
    public void NavigateTo(string? url, bool forceReload = false, bool replace = false)
    {
        if (string.IsNullOrEmpty(url))
        {
            return;
        }

        _navigationManager.NavigateTo(url, replace: replace, forceLoad: forceReload);
    }

    /// <summary>Liefert die aktuelle Url ohne Basis-Url (ohne führenden Slash).</summary>
    public string GetRelativeUri()
    {
        return _navigationManager.Uri.Replace(_navigationManager.BaseUri, string.Empty);
    }

    /// <summary>Liefert die aktuelle absolute Url.</summary>
    public string GetCurrentUri()
    {
        return _navigationManager.Uri;
    }
}
