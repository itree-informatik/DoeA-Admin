// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Enums;
using ItreeMud.Enums;
using ItreeMud.Models;
using static DoeA.Components.Pages.Ausschreibung.AusschreibungLoseUndOfferten;

namespace DoeA.Data.Models.DTO;

/// <summary>Los einer Ausschreibung mit Offerten und BKP-Nummern — abgebildet auf dbo.t_los (Präfix "Los").</summary>
public class Los : Basis
{
    public string Bezeichnung { get; set; } = string.Empty;
    public DateOnly? Abbruchdatum { get; set; }
    public DateOnly? Eingereicht { get; set; }
    public bool VerzoegerungDurchAuftraggeber { get; set; }
    public EnumLosZustand Zustand { get; set; }
    public Guid AusId { get; set; }
    public List<Offerte> Offerten { get; set; } = new();
    public List<LosBkpNummer> BkpNummern { get; set; } = new();
}
