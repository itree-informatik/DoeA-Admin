// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.Security.Claims;
using DoeA.Data.Constants;
using DoeA.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;

namespace DoeA.Services;

/// <summary>
/// Liest den angemeldeten Benutzer aus dem AuthenticationStateProvider des Circuits. Der Zugriff ist bewusst
/// synchron: Blazor setzt den AuthenticationState beim Start des Circuits (und im Prerender) als bereits
/// abgeschlossenen Task, bevor eine Komponente einen Service anfordert. Ausserhalb eines Circuits
/// (Minimal-API, OIDC-Events) wirft der Provider — dort gilt der Aufrufer als anonym; Services mit
/// Audit-Stempel werden in solchen Scopes nicht aufgelöst.
/// </summary>
public class AktuellerBenutzer : IAktuellerBenutzer
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public AktuellerBenutzer(AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }

    /// <summary>eIAM-Identifikator (Claim "sub"); leer für anonyme Aufrufer.</summary>
    public string ExtId => ClaimWert(AuthKonstanten.ClaimSub);

    /// <summary>Vorname und Nachname aus den eIAM-Claims.</summary>
    public string Anzeigename => $"{ClaimWert(AuthKonstanten.ClaimVorname)} {ClaimWert(AuthKonstanten.ClaimNachname)}".Trim();

    /// <summary>In T_User gespeicherte UI-Sprache (Claim doea_sprache); leer für anonyme Aufrufer.</summary>
    public string Sprache => ClaimWert(AuthKonstanten.ClaimUsrSprache);

    /// <summary>true, wenn ein eIAM-Login vorliegt — unabhängig von der Freischaltung in T_User.</summary>
    public bool IstAuthentisiert => Principal.Identity?.IsAuthenticated == true;

    /// <summary>true, wenn der Benutzer in T_User aktiv ist (Claim doea_autorisiert).</summary>
    public bool IstAutorisiert => Principal.HasClaim(AuthKonstanten.ClaimAutorisiert, AuthKonstanten.ClaimWertTrue);

    private ClaimsPrincipal Principal
    {
        get
        {
            try
            {
                var state = _authenticationStateProvider.GetAuthenticationStateAsync();
                return state.IsCompletedSuccessfully ? state.Result.User : Anonym();
            }
            catch (InvalidOperationException)
            {
                return Anonym();
            }
        }
    }

    private static ClaimsPrincipal Anonym() => new(new ClaimsIdentity());

    private string ClaimWert(string claimTyp) => Principal.FindFirst(claimTyp)?.Value ?? string.Empty;
}
