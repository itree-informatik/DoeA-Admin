// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using ItreeMud.Models;
using System.Collections.ObjectModel;

namespace DoeA.Interfaces;

/// <summary>Generisches CRUD-Protokoll aller Entity-Services (siehe <c>BaseService</c>).</summary>
public interface IBaseService<TModel> where TModel : Basis
{
    /// <summary>Liefert alle aktiven Datensätze, sortiert nach der ersten passenden Sortier-Eigenschaft.</summary>
    Task<List<TModel>?> GetAll();

    /// <summary>Liefert den Datensatz zur Id; wirft, wenn er nicht existiert.</summary>
    Task<TModel> GetById(Guid id);

    /// <summary>Validiert das Modell, setzt die Audit-Felder und legt den Datensatz an.</summary>
    Task<TModel> Create(TModel model);

    /// <summary>
    /// Speichert Änderungen, sofern es welche gibt und kein Concurrency-Konflikt vorliegt.
    /// <paramref name="ignoreList"/> nimmt kommagetrennte Eigenschaften, die beim Vergleich übersprungen werden.
    /// </summary>
    Task<TModel> Update(TModel model, string ignoreList = "");

    /// <summary>Löscht den Datensatz zur Id und liefert ihn zurück.</summary>
    Task<TModel> Delete(Guid id);

    /// <summary>Sucht über die entitätsspezifische SuchBedingung, optional nur aktive Datensätze; sortiert wie GetAll.</summary>
    Task<ObservableCollection<TModel>> Search(string searchValue, bool onlyActive);
}