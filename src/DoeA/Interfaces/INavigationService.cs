// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

namespace DoeA.Interfaces;

/// <summary>Kapselt den <c>NavigationManager</c>, damit Komponenten und Services testbar navigieren können.</summary>
public interface INavigationService
{
    /// <summary>Navigiert auf die Url; leere Urls werden ignoriert.</summary>
    void NavigateTo(string? url, bool forceReload = false, bool replace = false);

    /// <summary>Liefert die aktuelle Url ohne Basis-Url (ohne führenden Slash).</summary>
    string GetRelativeUri();

    /// <summary>Liefert die aktuelle absolute Url.</summary>
    string GetCurrentUri();
}
