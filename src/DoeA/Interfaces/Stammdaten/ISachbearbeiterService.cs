// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Enums;
using DoeA.Data.Models.DTO.Stammdaten;
using System.Collections.ObjectModel;

namespace DoeA.Interfaces.Stammdaten;

/// <summary>Stammdaten der Sachbearbeiter.</summary>
public interface ISachbearbeiterService : IBaseService<Sachbearbeiter>
{
    /// <summary>Liefert alle aktiven Sachbearbeiter als schlanke Projektion samt Organisationszugehörigkeit.</summary>
    Task<ObservableCollection<Sachbearbeiter>> GetAllWithOrganisationszugehoerigkeit();

    /// <summary>Aktive Sachbearbeiter der Organisation plus alle, auch inaktive, die je in deren Rolle einer Ausschreibung zugewiesen waren.</summary>
    Task<List<Sachbearbeiter>> GetFilterAuswahl(EnumOrganisationszugehoerigkeit organisation);
}