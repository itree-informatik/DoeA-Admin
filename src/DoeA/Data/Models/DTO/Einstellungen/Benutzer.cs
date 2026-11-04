// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Helper;

namespace DoeA.Data.Models.DTO.Einstellungen;

/// <summary>
/// Benutzer aus dem eIAM — abgebildet auf dbo.t_user (Präfix "Usr"). Bewusst nicht von Basis abgeleitet,
/// weil T_User keine Audit-Spalten hat; der Datensatz wird nur über BenutzerService geschrieben.
/// </summary>
public class Benutzer
{
    public Guid Id { get; set; }
    public string ExtId { get; set; } = string.Empty;
    public string Vorname { get; set; } = string.Empty;
    public string Nachname { get; set; } = string.Empty;
    public string EMail { get; set; } = string.Empty;
    public string Sprache { get; set; } = SprachHelper.Standard;

    /// <summary>Letzte erfolgreiche Anmeldung als DateTime.Ticks; null bei noch nie freigeschalteten Benutzern.</summary>
    public long? LastLogin { get; set; }

    public bool Aktiv { get; set; }
}
