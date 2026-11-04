// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DTO.Einstellungen;

namespace DoeA.Interfaces.Einstellungen;

/// <summary>Zugriff auf die von Serilog geschriebenen Logeinträge (Tabelle T_Log) inklusive Loglevel-Steuerung.</summary>
public interface ILogService
{
    /// <summary>Liefert die neuesten Logeinträge zu den Suchkriterien.</summary>
    Task<IList<Log>> Search(LogSearchParameter parameter);

    /// <summary>Exportiert die Treffer der Suchkriterien nach Excel und liefert die Id des bereitgestellten Downloads.</summary>
    Task<string> Export(LogSearchParameter parameter);

    /// <summary>
    /// Löscht Logeinträge, die älter sind als die konfigurierte Aufbewahrungsdauer
    /// (<c>AnzahlTageLog</c>), und liefert die Anzahl der entfernten Zeilen.
    /// </summary>
    Task<int> Delete();

    /// <summary>Setzt das Mindest-Loglevel zur Laufzeit, ohne die Applikation neu zu starten.</summary>
    void SetLogLevel(int logLevel);

    /// <summary>Liefert das aktuelle Mindest-Loglevel als Zahlenwert des Serilog-LogEventLevel.</summary>
    int GetLogLevel();
}
