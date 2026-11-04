// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.Security.Claims;
using DoeA.Data.Enums;
using DoeA.Data.Models.DTO.Einstellungen;

namespace DoeA.Interfaces.Einstellungen;

/// <summary>Benutzer aus dem eIAM: Abgleich beim Login, Statusabfragen für Cookie und Circuit, Verwaltung auf der Benutzerseite.</summary>
public interface IBenutzerService
{
    /// <summary>
    /// Sucht den Benutzer über den Claim "sub", übernimmt Vorname/Nachname/E-Mail aus den Claims und stempelt bei
    /// aktiven Benutzern die letzte Anmeldung; Unbekannte werden inaktiv angelegt. Liefert die DoeA-Claims fürs Cookie.
    /// </summary>
    Task<IReadOnlyList<Claim>> SynchronisiereLogin(ClaimsPrincipal principal);

    /// <summary>Aktueller DB-Stand (Aktiv, Sprache) zum Abgleich mit den Cookie-Claims; null, wenn der Benutzer nicht existiert.</summary>
    Task<BenutzerStatus?> HoleStatus(string extId);

    /// <summary>Stempelt die letzte Anmeldung, wenn ein Benutzer mit bestehendem Cookie freigeschaltet wurde.</summary>
    Task MarkiereLogin(Guid id);

    /// <summary>DoeA-Claims (Sprache, Autorisierung) aus dem DB-Stand; bestimmt allein, welche Claims der Cookie-Abgleich ersetzt.</summary>
    IReadOnlyList<Claim> ErzeugeClaims(BenutzerStatus status);

    /// <summary>Benutzerliste nach vordefiniertem Filter und optionalem Suchtext (ExtId, Vorname, Nachname, E-Mail).</summary>
    Task<List<Benutzer>> Suche(EnumBenutzerFilter filter, string? suchtext);

    /// <summary>Schaltet einen Benutzer frei oder deaktiviert ihn; den eigenen Status kann niemand ändern.</summary>
    Task SetzeAktiv(Guid id, bool aktiv);

    /// <summary>Persistiert die vom Benutzer gewählte UI-Sprache.</summary>
    Task SetzeSprache(string extId, string sprache);
}
