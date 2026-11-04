// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

namespace DoeA.Endpoints;

/// <summary>
/// Heartbeat der Sitzungsüberwachung (sitzung.js): hält das Auth-Cookie am Leben, solange ein Tab offen ist, und meldet
/// dem Browser eine verlorene Sitzung (Ablauf, manueller Logout in einem anderen Tab).
/// </summary>
public static class SitzungEndpoints
{
    /// <summary>Anonym erreichbar: 204 bei gültigem Cookie, sonst 401 ohne Challenge.</summary>
    public const string PfadHeartbeat = "/sitzung/heartbeat";

    /// <summary>Registriert den Heartbeat-Endpunkt.</summary>
    public static void MapSitzung(this IEndpointRouteBuilder endpoints)
    {
        // Der Endpunkt selbst tut nichts: UseAuthentication prüft das Cookie ohnehin bei jedem Request und erneuert es
        // nach der Halbzeit; OnValidatePrincipal läuft dabei mit (ein SELECT) und bringt Aktiv und Sprache alle
        // 5 Minuten ins Cookie. Reiner Statuscode statt Challenge, damit fetch nie zu eIAM umgeleitet wird.
        endpoints.MapGet(PfadHeartbeat, (HttpContext httpContext) =>
                httpContext.User.Identity?.IsAuthenticated == true ? Results.NoContent() : Results.Unauthorized())
            .AllowAnonymous();
    }
}
