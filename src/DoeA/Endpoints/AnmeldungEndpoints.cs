// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using ILogger = Serilog.ILogger;

namespace DoeA.Endpoints;

/// <summary>
/// An- und Abmeldung. Einen Login-Endpunkt gibt es nicht: der 401 der Authorization-Middleware leitet direkt
/// zu eIAM. Die Pfade meiden die vom eIAM reservierten Namen (/login, /logout, /auth).
/// </summary>
public static class AnmeldungEndpoints
{
    /// <summary>Anonyme Statusseite: nicht freigeschaltet, abgemeldet oder Anmeldefehler.</summary>
    public const string PfadAnmeldung = "/anmeldung";

    /// <summary>RP-initiated Logout (Cookie und eIAM-Session) nur von der eigenen Seite, führt anschliessend auf die Statusseite.</summary>
    public const string PfadAbmeldung = "/abmeldung";

    /// <summary>Query-Parameter, mit dem eine fehlgeschlagene Anmeldung auf der Statusseite gemeldet wird.</summary>
    public const string ParameterFehler = "fehler";

    /// <summary>Ziel nach einer fehlgeschlagenen Anmeldung (Statusseite mit Fehlerhinweis).</summary>
    public const string PfadAnmeldungFehler = $"{PfadAnmeldung}?{ParameterFehler}=1";

    /// <summary>Registriert den Abmelde-Endpunkt; die Anmeldung selbst übernimmt die Challenge der Authorization-Middleware.</summary>
    public static void MapAnmeldung(this IEndpointRouteBuilder endpoints)
    {
        // Anonym statt RequireAuthorization: mit abgelaufenem Cookie würde die Challenge den Abmeldenden zum Login
        // nach eIAM schicken. Ohne Policy, damit sich auch nicht freigeschaltete Benutzer abmelden können.
        endpoints.MapGet(PfadAbmeldung, async (HttpContext httpContext, IOptionsMonitor<OpenIdConnectOptions> oidcOptions, ILogger logger) =>
        {
            if (httpContext.User.Identity?.IsAuthenticated != true)
            {
                return Results.Redirect(PfadAnmeldung);
            }

            // Logout-CSRF: das Lax-Cookie reist bei jeder Top-Level-Navigation mit, auch von fremden Seiten. Abmelden dürfen
            // nur die eigene Seite (Abmelden-Button, sitzung.js) und direkt eingegebene URLs («none»); Browser ohne
            // Fetch-Metadata senden den Header nicht und bleiben unbehelligt.
            var herkunft = httpContext.Request.Headers["Sec-Fetch-Site"].ToString();
            if (herkunft is not ("" or "same-origin" or "none"))
            {
                logger.Warning("Abmeldung von fremder Seite abgewiesen (Sec-Fetch-Site {Herkunft})", herkunft);
                return Results.Redirect("/");
            }

            var oidcKonfiguration = await oidcOptions.Get(OpenIdConnectDefaults.AuthenticationScheme)
                .ConfigurationManager!.GetConfigurationAsync(httpContext.RequestAborted);

            // Ohne end_session_endpoint in der Discovery wirft der OIDC-Handler eine InvalidOperationException
            // («Cannot redirect to the end session endpoint») — darum nur das Cookie abmelden; die eIAM-Session lebt weiter.
            if (string.IsNullOrEmpty(oidcKonfiguration.EndSessionEndpoint))
            {
                logger.Warning("Discovery ohne end_session_endpoint: nur das Cookie wird abgemeldet, die eIAM-Session bleibt bestehen");
                await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Results.Redirect(PfadAnmeldung);
            }

            // Ohne RedirectUri in den Properties: eIAM dokumentiert für den Logout weder post_logout_redirect_uri noch
            // state, der Rücksprung ist die dort registrierte Logoutseite bzw. die SignedOutRedirectUri (/anmeldung).
            // Ein Abmeldegrund reist deshalb nie über die URL — sitzung.js hinterlegt ihn im sessionStorage des Tabs.
            return Results.SignOut(authenticationSchemes:
                [CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme]);
        }).AllowAnonymous();
    }
}
