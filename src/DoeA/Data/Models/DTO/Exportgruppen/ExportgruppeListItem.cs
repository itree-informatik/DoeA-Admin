// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using ItreeMud.Models;

namespace DoeA.Data.Models.DTO.Exportgruppen;

/// <summary>Schlanke Listendarstellung einer Exportgruppe (nur Name) für die Übersicht.</summary>
public class ExportgruppeListItem : Basis
{
    public string Name { get; set; } = null!;
}