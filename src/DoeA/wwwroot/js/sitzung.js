// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

// Sitzungsüberwachung bei Inaktivität. Bewusst ein ES-Modul statt globaler Funktionen wie in doea.js: das Modul hält
// Zustand (Listener, Intervalle, DotNet-Referenz) und wird deshalb über IJSObjectReference gekapselt.
//
// Der Browser ist die einzige Autorität: nur er sieht den localStorage aller Tabs und das Cookie, und nur er kann auch
// ohne lebenden Circuit (Server-Neustart, Netzunterbruch) noch abmelden. Blazor (SitzungsUeberwachung.razor) zeigt nur
// Dialog und Countdown und reicht «Sitzung verlängern» hierher zurück; navigiert wird ausschliesslich hier.
//
// Ein manueller Logout in Tab A erreicht Tab B erst beim nächsten Heartbeat (401 → Reload → eIAM-Login, weil auch die
// SSO-Session beendet ist); der Revalidator sieht das nicht, weil der DB-Status unverändert ist.
//
// Den Abmeldegrund erhält /anmeldung nicht über die URL: eIAM dokumentiert für den RP-initiated Logout weder
// post_logout_redirect_uri noch state, der Rücksprung ist eine fest registrierte Logoutseite. Der Grund liegt deshalb
// im sessionStorage des Tabs, überlebt dort den Umweg über eIAM und wird von /anmeldung einmalig abgeholt.

const SchluesselLetzteAktivitaet = 'doea.letzteAktivitaet';
const SchluesselAbmeldung = 'doea.abmeldung';
const SchluesselAbmeldegrund = 'doea.abmeldegrund';
const Aktivitaetsereignisse = ['click', 'keydown', 'input'];

let dotNetRef = null;
let optionen = null;
let heartbeatIntervall = null;
let pruefIntervall = null;
let warnungSichtbar = false;

// Idempotent (zuerst stop): ES-Module werden pro URL einmal instanziiert, ein zweiter Start darf Listener nicht verdoppeln.
export function start(ref, opt) {
    stop();
    dotNetRef = ref;
    optionen = opt;

    // Jeder benutzerinitiierte Seitenaufbau ist Aktivität; app-initiierte Vollreloads passieren nur bei totem Cookie
    // oder deaktiviertem Benutzer.
    setzeAktivitaet();
    // Ein liegengebliebener Grund (Abbruch zwischen /abmeldung und /anmeldung) gilt nach erneutem Login nicht mehr.
    sessionStorage.removeItem(SchluesselAbmeldegrund);

    // Capture-Phase, damit die Listener vor jedem stopPropagation in MudBlazor-Handlern laufen.
    Aktivitaetsereignisse.forEach(typ => document.addEventListener(typ, beiAktivitaet, true));
    window.addEventListener('storage', beiStorage);
    document.addEventListener('visibilitychange', beiSichtbarkeit);
    window.addEventListener('focus', pruefe);
    heartbeatIntervall = setInterval(heartbeat, opt.heartbeatIntervallMs);
    pruefIntervall = setInterval(pruefe, opt.pruefIntervallMs);
}

export function stop() {
    Aktivitaetsereignisse.forEach(typ => document.removeEventListener(typ, beiAktivitaet, true));
    window.removeEventListener('storage', beiStorage);
    document.removeEventListener('visibilitychange', beiSichtbarkeit);
    window.removeEventListener('focus', pruefe);
    clearInterval(heartbeatIntervall);
    clearInterval(pruefIntervall);
    heartbeatIntervall = null;
    pruefIntervall = null;
    dotNetRef = null;
    optionen = null;
    warnungSichtbar = false;
}

// Explizit statt über den Klick-Listener, damit die Funktion nicht von der Event-Reihenfolge des Button-Klicks abhängt.
export function verlaengern() {
    if (!dotNetRef) {
        return;
    }

    setzeAktivitaet();
    heartbeat();
    pruefe();
}

// Liest den Grund und löscht ihn: ein Reload von /anmeldung zeigt danach wieder den generischen Text.
export function holeAbmeldegrund() {
    const grund = sessionStorage.getItem(SchluesselAbmeldegrund);
    sessionStorage.removeItem(SchluesselAbmeldegrund);
    return grund;
}

export function pruefe() {
    if (!dotNetRef) {
        return;
    }

    const rest = letzteAktivitaet() + optionen.inaktivitaetMs - Date.now();
    if (rest <= 0) {
        abmelden();
    } else if (rest <= optionen.warnungVorlaufMs) {
        // Bei jedem Durchlauf, damit sich der Countdown korrigiert, wenn ein anderer Tab zwischenzeitlich aktiv war.
        warnungSichtbar = true;
        rufeDotNet('ZeigeWarnung', rest);
    } else if (warnungSichtbar) {
        warnungSichtbar = false;
        rufeDotNet('VerbergeWarnung');
    }
}

function beiAktivitaet(ereignis) {
    // Nur echte Benutzereingaben zählen, keine per dispatchEvent erzeugten Ereignisse.
    if (!ereignis.isTrusted) {
        return;
    }

    setzeAktivitaet();
    if (warnungSichtbar) {
        pruefe();
    }
}

// Feuert nur in den anderen Tabs desselben Browsers.
function beiStorage(ereignis) {
    if (ereignis.key === SchluesselLetzteAktivitaet) {
        pruefe();
    } else if (ereignis.key === SchluesselAbmeldung && ereignis.newValue) {
        navigiereZurAbmeldung();
    }
}

// Nach Standby oder Tab-Wechsel sofort entscheiden statt auf das gedrosselte Intervall zu warten.
function beiSichtbarkeit() {
    if (document.visibilityState === 'visible') {
        pruefe();
    }
}

function setzeAktivitaet() {
    const jetzt = Date.now();
    localStorage.setItem(SchluesselLetzteAktivitaet, String(jetzt));
    return jetzt;
}

function letzteAktivitaet() {
    const wert = Number(localStorage.getItem(SchluesselLetzteAktivitaet));
    // Fehlender oder ungültiger Wert (gelöschte Website-Daten) zählt als jetzt, sonst würde sofort abgemeldet.
    return wert > 0 ? wert : setzeAktivitaet();
}

async function heartbeat() {
    if (!dotNetRef) {
        return;
    }

    try {
        const antwort = await fetch(optionen.heartbeatUrl, { credentials: 'same-origin', cache: 'no-store', redirect: 'manual' });
        // Nach einem stop() (Abmeldung läuft bereits) darf ein noch offener Heartbeat die Navigation nicht überholen.
        if (!dotNetRef) {
            return;
        }

        // Sitzung verloren: Reload, die HTTP-Pipeline entscheidet (eIAM oder /anmeldung). Alle anderen Antworten (5xx)
        // werden ignoriert, ein Deployment darf nicht abmelden.
        if (antwort.status === 401 || antwort.type === 'opaqueredirect') {
            window.location.reload();
        }
    } catch {
        // Netzfehler: das nächste Intervall versucht es erneut.
    }
}

// Die anderen Tabs ziehen über das storage-Ereignis nach, ohne den Schlüssel erneut zu schreiben.
function abmelden() {
    localStorage.setItem(SchluesselAbmeldung, String(Date.now()));
    navigiereZurAbmeldung();
}

function navigiereZurAbmeldung() {
    const url = optionen.abmeldungUrl;
    sessionStorage.setItem(SchluesselAbmeldegrund, optionen.abmeldegrund);
    stop();
    window.location.assign(url);
}

// In der SignalR-Reconnect-Phase schlägt invokeMethodAsync fehl; ohne catch gäbe das eine «Unhandled promise rejection».
function rufeDotNet(methode, ...argumente) {
    dotNetRef?.invokeMethodAsync(methode, ...argumente).catch(() => { });
}
