// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Helper;
using ItreeMud.Extensions;
using ItreeMud.Models;
using DoeA.Data.Models.DTO.Stammdaten;

namespace DoeA.Data.Models.DTO;

/// <summary>Terminplan-Eintrag: Verfahrensschritt einer Ausschreibung mit Von-/Bis-Datum — abgebildet auf dbo.t_ausschreibungverfahrensschritt (Präfix "Avs").</summary>
public class AusschreibungVerfahrensschritt : Basis
{
    public Guid AusId { get; set; }
    public Guid VesId { get; set; }
    public DateOnly? DatumVon { get; set; }
    public DateOnly? DatumBis { get; set; }
    public Verfahrensschritt Verfahrensschritt { get; set; } = new();

    public string Dauer
    {
        get
        {
            if (DatumVon.HasValue && DatumBis.HasValue)
            {
                return ArbeitstageHelper.TerminDauer(DatumVon.Value, DatumBis.Value, Verfahrensschritt.NurWerktage).FormatNumber();
            }

            return string.Empty;
        }
    }
}