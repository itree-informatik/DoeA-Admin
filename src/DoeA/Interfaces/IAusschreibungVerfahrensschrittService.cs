// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DTO;
using System.Collections.ObjectModel;

namespace DoeA.Interfaces;

/// <summary>Terminplanung einer Ausschreibung (Verfahrensschritte mit Datum von/bis).</summary>
public interface IAusschreibungVerfahrensschrittService : IBaseService<AusschreibungVerfahrensschritt>
{
    /// <summary>Lädt die Verfahrensschritte einer Ausschreibung in der Reihenfolge der Stammdaten-Sortierung, für App und PDF gleich.</summary>
    Task<ObservableCollection<AusschreibungVerfahrensschritt>> Load(Guid ausId);

    /// <summary>Speichert die Terminplanung gesamthaft und liefert den neu geladenen Stand zurück.</summary>
    Task<ObservableCollection<AusschreibungVerfahrensschritt>> SaveAll(Guid ausId, IEnumerable<AusschreibungVerfahrensschritt> items);
}