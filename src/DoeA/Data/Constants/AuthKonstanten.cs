// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

namespace DoeA.Data.Constants;

/// <summary>Namen und Zeitwerte der Authentifizierung: Cookie, Claims, Policy und das Session-Timing nach eIAM-Standard.</summary>
public static class AuthKonstanten
{
    /// <summary>Name des Auth-Cookies.</summary>
    public const string CookieName = "DoeA.Auth";
    /// <summary>Policy, die den Claim doea_autorisiert = true verlangt; über _Imports.razor an allen Seiten.</summary>
    public const string PolicyAutorisiert = "Autorisiert";

    /// <summary>eIAM-Standardattribut (docs.eiam.admin.ch, OIDC Standard Attribute Identifier): eindeutige Benutzer-ID, roh als UsrExtId gespeichert.</summary>
    public const string ClaimSub = "sub";
    /// <summary>eIAM-Standardattribut: Vorname.</summary>
    public const string ClaimVorname = "given_name";
    /// <summary>eIAM-Standardattribut: Nachname.</summary>
    public const string ClaimNachname = "family_name";
    /// <summary>eIAM-Standardattribut: E-Mail, zugleich Name-Claim der Identity.</summary>
    public const string ClaimEmail = "email";
    /// <summary>eIAM-Standardattribut: Sprache; zählt nur beim Anlegen des Benutzers.</summary>
    public const string ClaimSprache = "language";

    /// <summary>Eigener Claim: Sprache aus T_User, beim Login ins Cookie geschrieben und pro Request mit der DB abgeglichen.</summary>
    public const string ClaimUsrSprache = "doea_sprache";
    /// <summary>Eigener Claim: UsrAktiv aus T_User, beim Login ins Cookie geschrieben und pro Request mit der DB abgeglichen.</summary>
    public const string ClaimAutorisiert = "doea_autorisiert";
    /// <summary>Eigener Claim: Login-Zeitpunkt (UTC, Roundtrip-Format) für die absolute Sitzungsdauer.</summary>
    public const string ClaimLoginZeitpunkt = "doea_login";
    /// <summary>Wert von doea_autorisiert für freigeschaltete Benutzer.</summary>
    public const string ClaimWertTrue = "true";
    /// <summary>Wert von doea_autorisiert für nicht freigeschaltete Benutzer.</summary>
    public const string ClaimWertFalse = "false";

    /// <summary>Abmeldegrund, den sitzung.js vor /abmeldung im sessionStorage des Tabs hinterlegt und den /anmeldung nach dem eIAM-Rücksprung anzeigt.</summary>
    public const string AbmeldegrundInaktivitaet = "inaktivitaet";

    /// <summary>eIAM-Standard (Anforderungen an Session-Management): Inactivity Interval 7'200 s, als Sliding-Ablauf des Cookies.</summary>
    public static readonly TimeSpan SitzungInaktivitaet = TimeSpan.FromMinutes(120);

    /// <summary>eIAM-Standard: Session-Lifetime 43'200 s, als absolute Obergrenze ab dem Login unabhängig von Aktivität.</summary>
    public static readonly TimeSpan SitzungMaximal = TimeSpan.FromHours(12);

    /// <summary>DoeA-Wert (kein eIAM-Standard): Abstand, in dem der offene Circuit Aktiv/Ablauf nachprüft — spätestens dann wirkt eine Deaktivierung im offenen Fenster.</summary>
    public static readonly TimeSpan RevalidierungIntervall = TimeSpan.FromMinutes(5);

    /// <summary>DoeA-Wert (kein eIAM-Standard): so lange vor dem Inaktivitätsablauf warnt der Browser mit Countdown — Dialog nach 110 Minuten.</summary>
    public static readonly TimeSpan SitzungWarnungVorlauf = TimeSpan.FromMinutes(10);

    /// <summary>DoeA-Wert (kein eIAM-Standard): Abstand der Heartbeats, die das Cookie am Leben halten. Muss unter der Hälfte von SitzungInaktivitaet liegen, weil der Cookie-Handler erst nach der Halbzeit erneuert.</summary>
    public static readonly TimeSpan HeartbeatIntervall = TimeSpan.FromMinutes(5);

    /// <summary>DoeA-Wert (kein eIAM-Standard): Abstand, in dem der Browser die letzte Aktivität gegen Warn- und Ablaufschwelle prüft.</summary>
    public static readonly TimeSpan AktivitaetsPruefungIntervall = TimeSpan.FromSeconds(30);
}
