// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using ItreeMud.Models;

namespace DoeA.Data.Models.DTO.Stammdaten;

/// <summary>Sachbearbeiter mit Organisationszugehörigkeit (RD/DoeA/BBL) — abgebildet auf dbo.t_sachbearbeiter (Präfix "Sac").</summary>
public class Sachbearbeiter : Basis
{
    public string Nachname { get; set; } = string.Empty;
    public string Vorname { get; set; } = string.Empty;
    public string Kurzbezeichnung { get; set; } = string.Empty;
    public Guid OrgId { get; set; }

    public string Fullname
    {
        get
        {
            return $"{Nachname} {Vorname}";
        }
    }

    public Organisationszugehoerigkeit? Organisationszugehoerigkeit { get; set; }
}