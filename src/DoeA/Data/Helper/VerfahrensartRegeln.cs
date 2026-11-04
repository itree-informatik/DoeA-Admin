// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Enums;

namespace DoeA.Data.Helper;

/// <summary>
/// Zentrale Regeln, welche Registerkarten der Ausschreibungsseite je Verfahrensart sichtbar sind
/// und wann das "Zuschlagsempfänger"-Verhalten (Freihändig) gilt.
/// Abgleich erfolgt über die <see cref="EnumVerfahrensart"/>-NaturalId.
/// </summary>
public static class VerfahrensartRegeln
{
    /// <summary>Terminplanung-Tab: nur Offen, Einladung, Selektiv.</summary>
    public static bool ZeigeTerminplanung(string? naturalId) => naturalId is
        nameof(EnumVerfahrensart.Offen)
        or nameof(EnumVerfahrensart.Einladung)
        or nameof(EnumVerfahrensart.Selektiv);

    /// <summary>Lose-Tab: Offen, Einladung, Selektiv und Freihändig (dort als "Zuschlagsempfänger").</summary>
    public static bool ZeigeLose(string? naturalId) => naturalId is
        nameof(EnumVerfahrensart.Offen)
        or nameof(EnumVerfahrensart.Einladung)
        or nameof(EnumVerfahrensart.Selektiv)
        or nameof(EnumVerfahrensart.Freihändig);

    /// <summary>Freihändig: Los wird als "Zuschlagsempfänger" geführt und darf genau eine Offerte haben.</summary>
    public static bool IstZuschlagsempfaengerModus(string? naturalId) =>
        naturalId is nameof(EnumVerfahrensart.Freihändig);

    /// <summary>Übersetzungsschlüssel für den Lose- bzw. Zuschlagsempfänger-Tab/-Titel.</summary>
    public static string LosTitelKey(string? naturalId) =>
        IstZuschlagsempfaengerModus(naturalId)
            ? "AUSSCHREIBUNG_MENU_ZUSCHLAGSEMPFAENGER"
            : "AUSSCHREIBUNG_MENU_LOSEUNDOFFERTEN";

    /// <summary>Berichtigungen: alle Verfahrensarten ausser Freihändig (eine freihändige Vergabe wird nicht berichtigt, nur widerrufen).</summary>
    public static bool DarfBerichtigungErfassen(string? naturalId) =>
        naturalId is not nameof(EnumVerfahrensart.Freihändig);

    /// <summary>Alle Vorankündigungs-Varianten (offen, selektiv, unbekannt); sie verhalten sich im Client identisch.</summary>
    public static bool IstVorankuendigung(string? naturalId) => naturalId is
        nameof(EnumVerfahrensart.VorankündigungOffen)
        or nameof(EnumVerfahrensart.VorankündigungSelektiv)
        or nameof(EnumVerfahrensart.VorankündigungUnbekannt);

    /// <summary>RFI und Vorankündigungen haben keine Lose: Ihr Widerruf wird in der Ausschreibung erfasst (Grunddaten).</summary>
    public static bool DarfWiderrufErfassen(string? naturalId) =>
        naturalId is nameof(EnumVerfahrensart.RFI) || IstVorankuendigung(naturalId);

    /// <summary>RFI und Vorankündigungen haben keine Lose: abgeschlossen sind sie, sobald ein Publikationsdatum gespeichert ist.</summary>
    public static bool IstAbgeschlossenMitPublikation(string? naturalId) =>
        naturalId is nameof(EnumVerfahrensart.RFI) || IstVorankuendigung(naturalId);

    /// <summary>
    /// Titel des Ausschreibungs-Bereichs in den Grunddaten: bei Vorankündigungen und Freihändig
    /// gibt es keine eigentliche Ausschreibung, sondern nur eine Publikation.
    /// </summary>
    public static string AusschreibungBereichTitelKey(string? naturalId) =>
        IstVorankuendigung(naturalId) || naturalId is nameof(EnumVerfahrensart.Freihändig)
            ? "AUSSCHREIBUNG_PUBLIKATION"
            : "AUSSCHREIBUNG_AUSSCHREIBUNG";

    /// <summary>
    /// Darf eine Vorankündigung referenzieren: alle Hauptverfahren (Offen, Selektiv, Freihändig, Einladung).
    /// Eine Vorankündigung selbst und eine RFI referenzieren keine Vorankündigung.
    /// </summary>
    public static bool DarfVorankuendigungReferenzieren(string? naturalId) => naturalId is
        nameof(EnumVerfahrensart.Offen)
        or nameof(EnumVerfahrensart.Selektiv)
        or nameof(EnumVerfahrensart.Freihändig)
        or nameof(EnumVerfahrensart.Einladung);

    /// <summary>
    /// Darf eine RFI referenzieren: alle Verfahrensarten ausser RFI selbst
    /// (Vorankündigungs-Varianten sowie die Hauptverfahren Offen, Selektiv, Freihändig, Einladung).
    /// </summary>
    public static bool DarfRfiReferenzieren(string? naturalId) =>
        IstVorankuendigung(naturalId)
        || naturalId is
            nameof(EnumVerfahrensart.Offen)
            or nameof(EnumVerfahrensart.Selektiv)
            or nameof(EnumVerfahrensart.Freihändig)
            or nameof(EnumVerfahrensart.Einladung);

    /// <summary>
    /// Verfahrensart der Folgeausschreibung einer Vorankündigung: die Variante gibt sie vor
    /// (selektiv → Selektiv, offen → Offen); bei "unbekannt" wird Offen vorbelegt und kann
    /// nachträglich geändert werden.
    /// </summary>
    public static string FolgeVerfahrensart(string? vorankuendigungNaturalId) => vorankuendigungNaturalId switch
    {
        nameof(EnumVerfahrensart.VorankündigungSelektiv) => nameof(EnumVerfahrensart.Selektiv),
        _ => nameof(EnumVerfahrensart.Offen),
    };

    /// <summary>NaturalId-Filter für die Vorankündigungs-Auswahl im Picker (alle Varianten).</summary>
    public static readonly string[] FilterVorankuendigung =
    [
        nameof(EnumVerfahrensart.VorankündigungOffen),
        nameof(EnumVerfahrensart.VorankündigungSelektiv),
        nameof(EnumVerfahrensart.VorankündigungUnbekannt),
    ];

    /// <summary>NaturalId-Filter für die RFI-Auswahl im Picker.</summary>
    public static readonly string[] FilterRfi = [nameof(EnumVerfahrensart.RFI)];

    /// <summary>
    /// Offertezustände (NaturalIds), die nur bei Verfahrensart Selektiv zur Auswahl stehen: Teilnahmeanträge der Phase 1
    /// und Zuschlag/Widerruf der Präqualifikation.
    /// </summary>
    private static readonly string[] OffertezustaendeNurSelektiv =
        ["teilnahmeantragerfasst", "teilnahmeantragabgelehnt", "zuschlagpraequali", "widerrufpraequali"];

    /// <summary>Ob ein Offertezustand bei der gegebenen Verfahrensart zur Auswahl steht.</summary>
    public static bool IstOffertezustandWaehlbar(string? verfahrensartNaturalId, string? offertezustandNaturalId) =>
        verfahrensartNaturalId is nameof(EnumVerfahrensart.Selektiv)
        || !OffertezustaendeNurSelektiv.Contains(offertezustandNaturalId);

    /// <summary>Vorbelegung der Sprachmatrix einer neuen Ausschreibung und einer Folgeausschreibung: Dokumentart → Sprachen (NaturalIds).</summary>
    public static IReadOnlyDictionary<string, string[]> StandardSprachen(string? naturalId) => naturalId switch
    {
        nameof(EnumVerfahrensart.Offen) or nameof(EnumVerfahrensart.Selektiv) or nameof(EnumVerfahrensart.Einladung) => new Dictionary<string, string[]>
        {
            [nameof(EnumDokumentart.Publikation)] = [SpracheDeutsch, SpracheFranzoesisch],
            [nameof(EnumDokumentart.Unterlagen)] = [SpracheDeutsch],
            [nameof(EnumDokumentart.Offerten)] = [SpracheDeutsch, SpracheFranzoesisch],
        },
        nameof(EnumVerfahrensart.Freihändig) or nameof(EnumVerfahrensart.VorankündigungOffen)
            or nameof(EnumVerfahrensart.VorankündigungSelektiv) or nameof(EnumVerfahrensart.VorankündigungUnbekannt)
            or nameof(EnumVerfahrensart.RFI) => new Dictionary<string, string[]>
        {
            [nameof(EnumDokumentart.Publikation)] = [SpracheDeutsch, SpracheFranzoesisch],
        },
        _ => new Dictionary<string, string[]>()
    };

    private const string SpracheDeutsch = "DE";
    private const string SpracheFranzoesisch = "FR";
}
