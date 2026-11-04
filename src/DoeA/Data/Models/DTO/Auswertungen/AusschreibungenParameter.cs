// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DTO.Stammdaten;

namespace DoeA.Data.Models.DTO.Auswertungen;

/// <summary>Parameter der Auswertung «Ausschreibungen»: Zeitraum, Verfahrens-/Projektarten, Leistungsarten, Rollen und Ämter.</summary>
public class AusschreibungenParameter
{
    public DateOnly DatumVon { get; set; }
    public DateOnly DatumBis { get; set; }

    /// <summary>true = Auswertung nach Jahr/Quartal (Tabelle «JahrUndQuartal»), false = nach Sprache/Dokumentart (Tabelle «Sprache»).</summary>
    public bool NachJahrQuartalen { get; set; }

    public bool Offen { get; set; }
    public bool Selektiv { get; set; }
    public bool Freihaendig { get; set; }
    public bool Vorankuendigung { get; set; }

    public bool Logistik { get; set; }
    public bool Bau { get; set; }

    /// <summary>Leer oder null = keine Einschränkung auf Leistungsarten.</summary>
    public List<Leistungsart>? Leistungsarten { get; set; }

    public bool Meldestelle { get; set; }
    public bool Vergabestelle { get; set; }

    public bool Personalverleih { get; set; }
    public bool BeschaffungDezentral { get; set; }

    public Departement? Dep { get; set; }
    public List<Amt>? Aemter { get; set; }

    public bool Excel { get; set; }
}
