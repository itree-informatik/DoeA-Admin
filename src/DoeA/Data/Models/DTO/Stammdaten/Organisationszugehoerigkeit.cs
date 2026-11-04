// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using ItreeMud.Models;

namespace DoeA.Data.Models.DTO.Stammdaten;

/// <summary>Organisationseinheit der Sachbearbeiter (RD/DoeA/BBL, über EnumOrganisationszugehoerigkeit im Code verankert) — abgebildet auf dbo.t_organisationszugehoerigkeit (Präfix "Org").</summary>
public class Organisationszugehoerigkeit : Basis
{
    public string Bezeichnung { get; set; } = string.Empty;
    public string Kurzbezeichnung { get; set; } = string.Empty;

}