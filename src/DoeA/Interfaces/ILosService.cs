// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DTO;
using System.Collections.ObjectModel;

namespace DoeA.Interfaces;

/// <summary>Lose einer Ausschreibung samt Offerten, Offertenzuständen und BKP-Nummern.</summary>
public interface ILosService : IBaseService<Los>
{
    /// <summary>Lädt alle Lose einer Ausschreibung inklusive ihrer BKP-Nummern.</summary>
    Task<ObservableCollection<Los>> Load(Guid ausId);

    /// <summary>
    /// Schliesst ein offenes Los ab: lehnt alle Offerten ohne Schlusszustand ab und markiert die
    /// Ausschreibung als abgeschlossen, sobald kein Los mehr offen ist.
    /// </summary>
    Task Abschliessen(Guid losId);

    /// <summary>Speichert den gesamten Tab "Lose und Offerten" einer Ausschreibung in einer Transaktion.</summary>
    Task SaveTab(Guid ausId, IEnumerable<Los> lose, int? pflichtenhefter);
}