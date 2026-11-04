// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.Globalization;
using System.Security.Claims;
using DoeA.Data.Constants;

namespace DoeA.Data.Helper;

/// <summary>Absolute Sitzungsdauer nach eIAM-Standard, geprüft über den Login-Zeitpunkt im Claim doea_login.</summary>
public static class SitzungHelper
{
    /// <summary>Claim mit dem aktuellen UTC-Zeitpunkt im Roundtrip-Format; wird beim Login ins Cookie geschrieben.</summary>
    public static Claim LoginZeitpunktClaim() =>
        new(AuthKonstanten.ClaimLoginZeitpunkt, DateTimeOffset.UtcNow.ToString("O", CultureInfo.InvariantCulture));

    /// <summary>true, wenn der Login länger als SitzungMaximal zurückliegt oder der Zeitpunkt im Cookie fehlt.</summary>
    public static bool IstAbgelaufen(ClaimsPrincipal principal)
    {
        var wert = principal.FindFirst(AuthKonstanten.ClaimLoginZeitpunkt)?.Value;
        if (!DateTimeOffset.TryParse(wert, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var login))
        {
            return true;
        }

        return DateTimeOffset.UtcNow - login > AuthKonstanten.SitzungMaximal;
    }
}
