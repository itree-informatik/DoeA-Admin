// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Constants;
using DoeA.Data.Models.DTO.Einstellungen;
using DoeA.Interfaces.Einstellungen;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using ILogger = Serilog.ILogger;

namespace DoeA.Services;

/// <summary>
/// Prüft im offenen Circuit periodisch, ob der Benutzer noch autorisiert und die Sitzung nicht abgelaufen ist —
/// das Cookie wird nur bei HTTP-Requests geprüft, SignalR-Verkehr löst keine Prüfung aus. Bei Abweichung wird der
/// Circuit anonym und Routes.razor (auf /anmeldung die Seite selbst) lädt die Seite neu; die HTTP-Pipeline entscheidet
/// dann über Re-Login oder /anmeldung.
/// </summary>
public class DoeAAuthenticationStateProvider : RevalidatingServerAuthenticationStateProvider
{
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public DoeAAuthenticationStateProvider(ILoggerFactory loggerFactory, IServiceScopeFactory scopeFactory, ILogger logger) : base(loggerFactory)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override TimeSpan RevalidationInterval => AuthKonstanten.RevalidierungIntervall;

    protected override async Task<bool> ValidateAuthenticationStateAsync(AuthenticationState authenticationState, CancellationToken cancellationToken)
    {
        var principal = authenticationState.User;
        if (principal.Identity?.IsAuthenticated != true || SitzungHelper.IstAbgelaufen(principal))
        {
            return false;
        }

        var extId = principal.FindFirst(AuthKonstanten.ClaimSub)?.Value;
        if (string.IsNullOrEmpty(extId))
        {
            return false;
        }

        BenutzerStatus? status;
        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            status = await scope.ServiceProvider.GetRequiredService<IBenutzerService>().HoleStatus(extId);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // Infrastrukturfehler (z.B. DB nicht erreichbar) beenden den Circuit nicht: die Basisklasse würde sonst
            // abmelden, und der Reload liefe in denselben Fehler. Die HTTP-Pipeline prüft beim nächsten Request erneut.
            _logger.Warning(ex, "Revalidierung der Sitzung nicht möglich, Sitzung bleibt bestehen");
            return true;
        }

        // Gelöschter Benutzer: wie in OnValidatePrincipal endet die Sitzung, der Re-Login legt ihn inaktiv neu an.
        if (status == null)
        {
            return false;
        }

        // Die Sprache wird bewusst nicht verglichen: sie ändert sich im Circuit sofort und landet erst mit dem
        // nächsten HTTP-Request im Cookie — ein Vergleich würde nach jedem Sprachwechsel einen Reload auslösen.
        return status.Aktiv == principal.HasClaim(AuthKonstanten.ClaimAutorisiert, AuthKonstanten.ClaimWertTrue);
    }
}
