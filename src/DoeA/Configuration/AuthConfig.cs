// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.Security.Claims;
using DoeA.Data.Constants;
using DoeA.Data.Models.DB;
using DoeA.Endpoints;
using DoeA.Interfaces.Einstellungen;
using DoeA.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using ILogger = Serilog.ILogger;

namespace DoeA.Configuration;

/// <summary>
/// Anbindung an das eIAM (OIDC Authorization Code Flow mit Client Secret) nach dem eIAM-Muster «Klassisch»: das
/// ID-Token baut die Cookie-Session, danach wird nur noch das Cookie geprüft. Die Autorisierung (UsrAktiv in T_User)
/// steht als Claim im Cookie und wird bei jedem HTTP-Request mit der DB abgeglichen; das Session-Timing folgt dem
/// eIAM-Standard (AuthKonstanten).
/// </summary>
public static class AuthConfig
{
    private const string KonfigAuthority = "OpenIdConnect:Authority";
    private const string KonfigClientId = "OpenIdConnect:ClientId";
    private const string KonfigClientSecret = "OpenIdConnect:ClientSecret";
    private const string ScopeProfile = "profile";

    /// <summary>Registriert Cookie- und OIDC-Authentifizierung, die Policy «Autorisiert» und die Data Protection mit Schlüsseln in der DB.</summary>
    public static void AddAuthServices(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        var authority = Pflichtwert(configuration, KonfigAuthority);
        var clientId = Pflichtwert(configuration, KonfigClientId);
        var clientSecret = Pflichtwert(configuration, KonfigClientSecret);
        if (!environment.IsDevelopment() && !authority.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException($"{KonfigAuthority} muss ausserhalb von Development https verwenden");
        }

        services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                // Der 401 der Authorization-Middleware geht direkt zu eIAM; eine eigene Login-Seite gibt es nicht.
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = AuthKonstanten.CookieName;
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                // SameAsRequest statt Always: lokal läuft die App auch über http, produktiv meldet der Proxy https.
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.ExpireTimeSpan = AuthKonstanten.SitzungInaktivitaet;
                options.SlidingExpiration = true;
                options.AccessDeniedPath = AnmeldungEndpoints.PfadAnmeldung;
                options.Events.OnValidatePrincipal = GleicheMitDatenbankAb;
            })
            .AddOpenIdConnect(options =>
            {
                options.Authority = authority;
                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.UsePkce = true;
                // Nur wegen id_token_hint beim Logout; OnTicketReceived verwirft die übrigen Tokens wieder.
                options.SaveTokens = true;
                options.Scope.Clear();
                options.Scope.Add(OpenIdConnectScope.OpenId);
                options.Scope.Add(ScopeProfile);
                options.Scope.Add(OpenIdConnectScope.Email);
                // eIAM: das Access-Token ist opak und nur für den UserInfo-Endpunkt gedacht.
                options.GetClaimsFromUserInfoEndpoint = true;
                // Claim-Namen bleiben wie im Token (sub, given_name, ...), statt auf die langen SOAP-URIs gemappt zu werden.
                options.MapInboundClaims = false;
                options.ClaimActions.MapUniqueJsonKey(AuthKonstanten.ClaimSprache, AuthKonstanten.ClaimSprache);
                options.TokenValidationParameters.NameClaimType = AuthKonstanten.ClaimEmail;
                // Lokales Keycloak läuft über http; produktiv ist https Pflicht (siehe Prüfung oben).
                options.RequireHttpsMetadata = !environment.IsDevelopment();
                options.SignedOutRedirectUri = AnmeldungEndpoints.PfadAnmeldung;
                options.Events.OnTicketReceived = UebernimmBenutzer;
                options.Events.OnRemoteFailure = BehandleAnmeldefehler;
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthKonstanten.PolicyAutorisiert,
                policy => policy.RequireClaim(AuthKonstanten.ClaimAutorisiert, AuthKonstanten.ClaimWertTrue));
        });

        services.AddCascadingAuthenticationState();
        services.AddScoped<AuthenticationStateProvider, DoeAAuthenticationStateProvider>();

        // Schlüssel in der DB, damit Cookies Container-Neustarts und mehrere Instanzen überleben.
        services.AddDataProtection()
            .SetApplicationName("DoeA")
            .PersistKeysToDbContext<DoeAContext>();
    }

    private static string Pflichtwert(IConfiguration configuration, string schluessel)
    {
        var wert = configuration.GetValue<string>(schluessel);
        if (string.IsNullOrWhiteSpace(wert))
        {
            throw new InvalidDataException($"{schluessel} ist nicht konfiguriert");
        }

        return wert;
    }

    /// <summary>
    /// Läuft nach ID-Token und UserInfo (erst dort sind alle eIAM-Claims beisammen): gleicht den Benutzer mit T_User ab
    /// und schreibt die DoeA-Claims samt Login-Zeitpunkt ins Cookie.
    /// </summary>
    private static async Task UebernimmBenutzer(TicketReceivedContext context)
    {
        var identity = (ClaimsIdentity)context.Principal!.Identity!;
        try
        {
            var benutzerService = context.HttpContext.RequestServices.GetRequiredService<IBenutzerService>();
            identity.AddClaims(await benutzerService.SynchronisiereLogin(context.Principal));
            identity.AddClaim(SitzungHelper.LoginZeitpunktClaim());
        }
        catch (Exception ex)
        {
            context.HttpContext.RequestServices.GetRequiredService<ILogger>().Error(ex, "Benutzerabgleich beim Login fehlgeschlagen");
            context.Response.Redirect(AnmeldungEndpoints.PfadAnmeldungFehler);
            context.HandleResponse();
            return;
        }

        // Nur das ID-Token bleibt im Cookie (id_token_hint für den Logout); Access-/Refresh-Token braucht DoeA nicht.
        var properties = context.Properties ??= new AuthenticationProperties();
        var idToken = properties.GetTokenValue(OpenIdConnectParameterNames.IdToken);
        properties.StoreTokens(idToken == null
            ? []
            : [new AuthenticationToken { Name = OpenIdConnectParameterNames.IdToken, Value = idToken }]);
    }

    private static Task BehandleAnmeldefehler(RemoteFailureContext context)
    {
        context.HttpContext.RequestServices.GetRequiredService<ILogger>().Warning(context.Failure, "OIDC-Anmeldung fehlgeschlagen");
        context.Response.Redirect(AnmeldungEndpoints.PfadAnmeldungFehler);
        context.HandleResponse();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Bei jedem HTTP-Request: abgelaufene Sitzung oder gelöschter Benutzer beenden die Sitzung (der Re-Login über eIAM
    /// legt den Benutzer bei Bedarf inaktiv neu an), sonst Autorisierung und Sprache aus der DB in die Claims übernehmen.
    /// Nie RejectPrincipal für Inaktive — das ergäbe eine Schleife über eIAM.
    /// </summary>
    private static async Task GleicheMitDatenbankAb(CookieValidatePrincipalContext context)
    {
        if (context.Principal?.Identity is not ClaimsIdentity { IsAuthenticated: true } identity)
        {
            return;
        }

        var extId = context.Principal.FindFirst(AuthKonstanten.ClaimSub)?.Value;
        if (SitzungHelper.IstAbgelaufen(context.Principal) || string.IsNullOrEmpty(extId))
        {
            await BeendeSitzung(context);
            return;
        }

        var benutzerService = context.HttpContext.RequestServices.GetRequiredService<IBenutzerService>();
        var status = await benutzerService.HoleStatus(extId);
        if (status == null)
        {
            await BeendeSitzung(context);
            return;
        }

        var autorisiertLautCookie = context.Principal.HasClaim(AuthKonstanten.ClaimAutorisiert, AuthKonstanten.ClaimWertTrue);
        var spracheLautCookie = context.Principal.FindFirst(AuthKonstanten.ClaimUsrSprache)?.Value;
        if (autorisiertLautCookie == status.Aktiv && spracheLautCookie == status.Sprache)
        {
            return;
        }

        // Freischaltung bei bestehendem Cookie zählt als erste erfolgreiche Anmeldung.
        if (status.Aktiv && !autorisiertLautCookie)
        {
            await benutzerService.MarkiereLogin(status.Id);
        }

        // Welche Claims ersetzt werden, bestimmt allein ErzeugeClaims — eine zweite Liste könnte auseinanderlaufen.
        var neueClaims = benutzerService.ErzeugeClaims(status);
        var ersetzteTypen = neueClaims.Select(c => c.Type).ToHashSet();
        var neueIdentity = new ClaimsIdentity(
            identity.Claims.Where(c => !ersetzteTypen.Contains(c.Type)),
            identity.AuthenticationType, identity.NameClaimType, identity.RoleClaimType);
        neueIdentity.AddClaims(neueClaims);
        context.ReplacePrincipal(new ClaimsPrincipal(neueIdentity));
        context.ShouldRenew = true;
    }

    private static async Task BeendeSitzung(CookieValidatePrincipalContext context)
    {
        context.RejectPrincipal();
        await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
