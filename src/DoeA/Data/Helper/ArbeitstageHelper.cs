// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.Collections.Concurrent;

namespace DoeA.Data.Helper;

/// <summary>
/// Zentrale Arbeitstag-Berechnung: Samstage, Sonntage und die offiziellen Feiertage des
/// Kantons Bern zählen nicht als Arbeitstage.
/// </summary>
public static class ArbeitstageHelper
{
    // Feiertage je Jahr nur einmal berechnen (die beweglichen Feiertage hängen vom Osterdatum ab).
    private static readonly ConcurrentDictionary<int, HashSet<DateOnly>> FeiertageProJahr = new();

    /// <summary>
    /// Anzahl Arbeitstage von <paramref name="von"/> bis und mit <paramref name="bis"/>.
    /// Liegt <paramref name="bis"/> vor <paramref name="von"/>, ist das Resultat 0.
    /// </summary>
    private static int AnzahlArbeitstage(DateOnly von, DateOnly bis)
    {
        var anzahl = 0;
        for (var datum = von; datum <= bis; datum = datum.AddDays(1))
        {
            if (IstArbeitstag(datum))
                anzahl++;
        }
        return anzahl;
    }

    /// <summary>
    /// Dauer eines Termins: Tage nach <paramref name="von"/> bis und mit <paramref name="bis"/>,
    /// bei <paramref name="nurWerktage"/> nur Arbeitstage, sonst Kalendertage; nie negativ.
    /// </summary>
    public static int TerminDauer(DateOnly von, DateOnly bis, bool nurWerktage) =>
        nurWerktage ? AnzahlArbeitstage(von.AddDays(1), bis) : Math.Max(0, bis.DayNumber - von.DayNumber);

    /// <summary>
    /// Bearbeitungsdauer bis zur Publikation in Arbeitstagen (der Publikationstag zählt nicht);
    /// <c>null</c>, solange eines der beiden Daten fehlt.
    /// </summary>
    public static int? PublikationsDauer(DateOnly? eingereicht, DateOnly? publikation) =>
        eingereicht is { } von && publikation is { } bis ? AnzahlArbeitstage(von, bis.AddDays(-1)) : null;

    /// <summary>True, wenn das Datum weder auf ein Wochenende noch auf einen Feiertag des Kantons Bern fällt.</summary>
    private static bool IstArbeitstag(DateOnly datum)
    {
        if (datum.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            return false;

        return !FeiertageKantonBern(datum.Year).Contains(datum);
    }

    /// <summary>
    /// Die offiziellen (gesetzlichen) Feiertage des Kantons Bern für ein Jahr:
    /// Neujahrstag, Berchtoldstag, Karfreitag, Ostermontag, Auffahrt, Pfingstmontag,
    /// Bundesfeiertag, Weihnachtstag und Stephanstag.
    /// </summary>
    private static IReadOnlySet<DateOnly> FeiertageKantonBern(int jahr)
    {
        return FeiertageProJahr.GetOrAdd(jahr, j =>
        {
            var ostersonntag = Ostersonntag(j);
            return new HashSet<DateOnly>
            {
                new(j, 1, 1),               // Neujahrstag
                new(j, 1, 2),               // Berchtoldstag
                ostersonntag.AddDays(-2),   // Karfreitag
                ostersonntag.AddDays(1),    // Ostermontag
                ostersonntag.AddDays(39),   // Auffahrt
                ostersonntag.AddDays(50),   // Pfingstmontag
                new(j, 8, 1),               // Bundesfeiertag
                new(j, 12, 25),             // Weihnachtstag
                new(j, 12, 26)              // Stephanstag
            };
        });
    }

    /// <summary>Ostersonntag nach dem anonymen gregorianischen Algorithmus (Meeus/Jones/Butcher).</summary>
    private static DateOnly Ostersonntag(int jahr)
    {
        var a = jahr % 19;
        var b = jahr / 100;
        var c = jahr % 100;
        var d = b / 4;
        var e = b % 4;
        var f = (b + 8) / 25;
        var g = (b - f + 1) / 3;
        var h = (19 * a + b - d - g + 15) % 30;
        var i = c / 4;
        var k = c % 4;
        var l = (32 + 2 * e + 2 * i - h - k) % 7;
        var m = (a + 11 * h + 22 * l) / 451;
        var monat = (h + l - 7 * m + 114) / 31;
        var tag = (h + l - 7 * m + 114) % 31 + 1;
        return new DateOnly(jahr, monat, tag);
    }
}
