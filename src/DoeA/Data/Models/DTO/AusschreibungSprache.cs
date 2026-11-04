// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using ItreeMud.Models;

namespace DoeA.Data.Models.DTO;

/// <summary>Publikationssprache je Dokumentart einer Ausschreibung (Wert = angekreuzt) — abgebildet auf dbo.t_ausschreibungsprache (Präfix "Asp").</summary>
public class AusschreibungSprache : Basis
{
    public Guid AusId { get; set; }
    public string Dokumentart { get; set; } = string.Empty;
    public Guid DoaId { get; set; }
    public string Sprache { get; set; } = string.Empty;
    public Guid SprId { get; set; }
    public bool Wert { get; set; }
}