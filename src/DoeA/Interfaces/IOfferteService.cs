// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DTO;

namespace DoeA.Interfaces;

/// <summary>Offerten eines Los samt ihren Offertenzuständen.</summary>
public interface IOfferteService : IBaseService<Offerte>
{
    /// <summary>Lädt alle Offerten eines Los inklusive Anbieter und Offertenzuständen.</summary>
    Task<List<Offerte>> Load(Guid losId);

    /// <summary>Speichert eine einzelne Offerte samt ihren Offertenzuständen.</summary>
    Task<Offerte> Save(Offerte offerte);
}
