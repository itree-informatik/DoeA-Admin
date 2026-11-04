// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DTO.Stammdaten;

namespace DoeA.Data.Models.DTO.Auswertungen;

/// <summary>Parameter der Auswertung «Zuschläge»: Zeitraum, Verfahrens-/Projektarten, Rollen sowie CPV-/BKP-Filter.</summary>
public class ZuschlaegeParameter
{
    public DateOnly DatumVon { get; set; }
    public DateOnly DatumBis { get; set; }
    public bool Offen { get; set; }
    public bool Selektiv { get; set; }
    public bool Freihaendig { get; set; }
    public bool Logistik { get; set; }
    public bool Bau { get; set; }
    public bool Meldestelle { get; set; }
    public bool Vergabestelle { get; set; }
    public string CpvNummern { get; set; } = string.Empty;
    public string BkpNummern { get; set; } = string.Empty;
    public bool Kanton { get; set; }
    public bool MitBetrag { get; set; }
    public Departement? Dep { get; set; }
    public List<Amt>? Aemter { get; set; }
    public bool Excel { get; set; }
}
