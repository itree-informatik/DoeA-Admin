// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

namespace DoeA.Interfaces;

/// <summary>Angemeldeter Benutzer des aktuellen Circuits aus den Cookie-Claims; für Audit-Felder und UI.</summary>
public interface IAktuellerBenutzer
{
    /// <summary>eIAM-Identifikator (Claim "sub"); leer für anonyme Aufrufer.</summary>
    string ExtId { get; }

    /// <summary>Vorname und Nachname aus den eIAM-Claims.</summary>
    string Anzeigename { get; }

    /// <summary>In T_User gespeicherte UI-Sprache (Claim doea_sprache); leer für anonyme Aufrufer.</summary>
    string Sprache { get; }

    /// <summary>true, wenn ein eIAM-Login vorliegt — unabhängig von der Freischaltung in T_User.</summary>
    bool IstAuthentisiert { get; }

    /// <summary>true, wenn der Benutzer in T_User aktiv ist (Claim doea_autorisiert).</summary>
    bool IstAutorisiert { get; }
}
