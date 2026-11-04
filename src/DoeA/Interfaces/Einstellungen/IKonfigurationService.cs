// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DTO.Einstellungen;

namespace DoeA.Interfaces.Einstellungen;

/// <summary>CRUD auf den Applikationskonfigurationen; die typisierten Wert-Properties werden dabei mitgefüllt.</summary>
public interface IKonfigurationService : IBaseService<Konfiguration>
{
}
