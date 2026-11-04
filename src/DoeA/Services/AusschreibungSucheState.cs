// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DTO;
using ItreeMud.Components;

namespace DoeA.Services;

/// <summary>
/// Hält den letzten Suchzustand der Ausschreibungs-Suche (Jahr, Suchbegriff, Filter, Spaltenfilter
/// und Ergebnis), damit dieser beim Zurückkehren auf die Suche erhalten bleibt.
/// Scoped registriert -> Lebensdauer entspricht dem Blazor-Circuit (übersteht Navigationen,
/// wird bei vollständigem Reload/Neuverbindung zurückgesetzt).
/// </summary>
public class AusschreibungSucheState
{
    /// <summary>Zuletzt verwendete Suchparameter (Jahr, Suchbegriff, Filter).</summary>
    public GridParameter? GridParameter { get; set; }

    /// <summary>Treffer der letzten Suche; null, solange noch nicht gesucht wurde.</summary>
    public IList<AusschreibungSuche>? Ergebnis { get; set; }

    /// <summary>Eine Ausschreibung wurde seit der letzten Suche gespeichert oder gelöscht: Die Suche lädt das Ergebnis beim nächsten Aufruf neu.</summary>
    public bool ErgebnisVeraltet { get; set; }

    /// <summary>
    /// Auswahl der Werte-Spaltenfilter (Spalten-Key -> ausgewählte Werte). Es wird dieselbe
    /// Dictionary-Instanz in der Komponente verwendet und in-place mutiert.
    /// </summary>
    public Dictionary<string, List<string>> SpaltenFilter { get; } = new();

    /// <summary>Zustand des Ergebnis-Grids (Spaltenfilter, Sortierung, Seite); null bis zur ersten Änderung.</summary>
    public ItreeGridState<AusschreibungSuche>? GridState { get; set; }

    /// <summary>
    /// Sichtbare Ergebnisspalten (Property-Namen). Dieselbe Set-Instanz wird vom Spalten-Popup
    /// in-place mutiert und bleibt so über Navigationen erhalten.
    /// </summary>
    public HashSet<string> SichtbareSpalten { get; } = AusschreibungSucheSpalten.DefaultSpalten();
}
