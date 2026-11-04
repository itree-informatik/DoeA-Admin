// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DTO.Auswertungen;
using DoeA.Data.Models.DTO.Stammdaten;

namespace DoeA.Interfaces.Auswertungen;

/// <summary>
/// Erstellt die Auswertungen als PDF oder Excel-Datei; der Rückgabewert ist jeweils die
/// Id der Datei im Download-Speicher (<see cref="IDownloadStorageService"/>).
/// </summary>
public interface IAuswertungService
{
    /// <summary>Abbrüche und Widerrufe über die fünf Jahre ab <paramref name="jahrVon"/>, nach Projektart gegliedert.</summary>
    Task<string> GetAbbruchWiderrufAsync(int jahrVon, Departement? dep = null, List<Amt>? aemter = null, bool excel = false);

    /// <summary>Effektive Zuschläge mit Zuschlagspublikation im gewählten Bereich nach Kanton oder Sprachregion des Zuschlagsempfängers.</summary>
    Task<string> GetZuschlaegeAsync(ZuschlaegeParameter parameter);

    /// <summary>Ausschreibungen nach Jahr/Quartal oder nach Sprache und Dokumentart.</summary>
    Task<string> GetAusschreibungenAsync(AusschreibungenParameter parameter);

    /// <summary>Effektive Zuschläge freihändiger Vergaben der gewählten Projektarten und Rollen BBL mit Publikationsdatum im gewählten Bereich (Excel, eine Zeile pro Offerte).</summary>
    Task<string> GetFreihaenderAsync(DateOnly datumVon, DateOnly datumBis, bool freihaendig,
        bool logistik, bool bau, bool aemterDezentral, bool logistikZentral);

    /// <summary>Effektive Zuschläge der gewählten Verfahrensarten (offen, selektiv, Einladung), Projektarten und Rollen BBL mit Zuschlagspublikation im gewählten Bereich (Excel, eine Zeile pro Offerte).</summary>
    Task<string> GetZuschlaegeOhneFreihaenderAsync(DateOnly datumVon, DateOnly datumBis, bool offen, bool selektiv, bool einladung,
        bool logistik, bool bau, bool aemterDezentral, bool logistikZentral);

    /// <summary>Laufende WTO-Verfahren mit offenen Losen und Offerten im Zustand "erfasst" bzw. in Phase 1/2 des selektiven Verfahrens (Excel).</summary>
    Task<string> GetLaufendeWtoAsync(DateOnly datumVon, DateOnly datumBis, bool offen, bool selektiv, bool einladung,
        bool logistik, bool bau, bool aemterDezentral, bool logistikZentral);

    /// <summary>Beschaffungscontrolling-Dashboard eines Jahres: Kategorien je Quartal und Spaltengruppe.</summary>
    Task<string> GetBeschaffungscontrollingAsync(int jahr);

    /// <summary>Terminplan einer Ausschreibung mit den Verfahrensschritten und deren Dauer gemäss «Nur Werktage».</summary>
    Task<string> GetTerminplanAsync(Guid ausId);

    /// <summary>Zielerreichung: Bearbeitungsdauer bis zur Publikation gegen die konfigurierte Frist.</summary>
    Task<string> GetZielerreichungAsync(DateOnly datumVon, DateOnly datumBis);
}
