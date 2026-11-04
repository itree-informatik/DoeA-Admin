// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

namespace DoeA.Endpoints;

/// <summary>
/// Liveness-Check für die Container-Laufzeit (HEALTHCHECK im Image, depends_on im Compose): 200 «Healthy», solange der
/// Prozess Requests beantwortet. Bewusst ohne Datenbankprüfung — ohne Datenbank startet die App gar nicht, und ein
/// späterer Unterbruch soll keinen Container-Neustart auslösen, der alle Blazor-Sitzungen beendet.
/// </summary>
public static class HealthEndpoints
{
    /// <summary>Anonym erreichbar, antwortet mit 200 und dem Text «Healthy».</summary>
    public const string PfadHealth = "/health";

    /// <summary>Registriert den Health-Check-Endpunkt.</summary>
    public static void MapHealth(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks(PfadHealth).AllowAnonymous();
    }
}
