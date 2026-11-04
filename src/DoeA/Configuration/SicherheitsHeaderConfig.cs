// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

namespace DoeA.Configuration;

/// <summary>Security-Header für alle Antworten, ohne Kestrel-Server-Header und mit sicherem Antiforgery-Cookie.</summary>
public static class SicherheitsHeaderConfig
{
    // style-src braucht 'unsafe-inline': MudBlazor setzt Inline-Styles. frame-ancestors steht auch hier, weil die scharfe
    // Policy den gleichnamigen Header von Blazor (nur frame-ancestors) ersetzt.
    private const string ContentSecurityPolicy =
        "default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; " +
        "font-src 'self' data:; connect-src 'self'; form-action 'self'; frame-ancestors 'self'; base-uri 'self'; object-src 'none'";

    private const string PermissionsPolicy =
        "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()";

    /// <summary>Schaltet den Server-Header ab und liefert das Antiforgery-Cookie über https mit Secure-Flag aus.</summary>
    public static void AddSicherheitsHeader(this WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);
        // SameAsRequest wie beim Sitzungscookie: lokal läuft die App auch über http, produktiv meldet der Proxy https.
        builder.Services.AddAntiforgery(options => options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest);
    }

    /// <summary>Setzt die Security-Header auf jeder Antwort, auch auf Fehlerseiten und statischen Dateien.</summary>
    public static void UseSicherheitsHeader(this WebApplication app)
    {
        // Im Development nur Report-Only: Browser Link und Hot Reload von Visual Studio laden Skripte von einem
        // anderen Port und würden sonst blockiert.
        var cspHeader = app.Environment.IsDevelopment() ? "Content-Security-Policy-Report-Only" : "Content-Security-Policy";

        app.Use(async (context, next) =>
        {
            // OnStarting statt direkt: UseExceptionHandler leert die Header einer fehlgeschlagenen Antwort.
            context.Response.OnStarting(() =>
            {
                var headers = context.Response.Headers;
                headers["X-Content-Type-Options"] = "nosniff";
                headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
                headers["Permissions-Policy"] = PermissionsPolicy;
                headers[cspHeader] = ContentSecurityPolicy;
                return Task.CompletedTask;
            });
            await next();
        });
    }
}
