// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO;

namespace DoeA.Data.Helper;

/// <summary>Gemeinsame Regel für den massgebenden Zustand einer Offerte (Loszustand beim Speichern, Anzeige in der Offertenzeile, Gesamtexport und Auswertungen).</summary>
public static class OfferteHelper
{
    /// <summary>Liefert den jüngsten Offertenzustand: jüngstes Datum, dann zuletzt erfasst, bei vollständigem Gleichstand die grösste Id.</summary>
    public static OfferteOffertezustand? LetzterZustand(this Offerte offerte) =>
        JuengsterZustand(offerte.Offertenzustaende, z => z.Datum ?? DateTime.MinValue, z => z.InsertedDate, z => z.Id);

    /// <summary>Regel von <see cref="LetzterZustand"/> für beliebige Darstellungen der Zustände einer Offerte, z.B. die Zeilen des Gesamtexports.</summary>
    public static T? JuengsterZustand<T, TDatum>(IEnumerable<T> zustaende, Func<T, TDatum> datum, Func<T, long> insertedDate, Func<T, Guid> id)
    {
        // Noch nicht gespeicherte Zustände haben InsertedDate == 0, gestempelt wird erst beim Speichern.
        // Sie sind die zuletzt erfassten und müssen daher den Gleichstand beim Datum gewinnen – sonst würde
        // ein älterer, bereits gespeicherter Zustand (z.B. "Zuschlag" am selben Tag) als aktueller gelten.
        // Untereinander gilt die Listenreihenfolge, wie LosService.SaveTab sie stempelt; zuletzt die Id wie bei
        // den effektiven Zuschlägen in der Datenbank (Guid sortiert in .NET wie uuid in PostgreSQL).
        return zustaende
            .Select((z, index) => (z, index))
            .OrderByDescending(t => datum(t.z))
            .ThenByDescending(t => insertedDate(t.z) == 0 ? long.MaxValue : insertedDate(t.z))
            .ThenByDescending(t => insertedDate(t.z) == 0 ? t.index : 0)
            .ThenByDescending(t => id(t.z))
            .Select(t => t.z)
            .FirstOrDefault();
    }

    private static readonly string[] OffertezustaendeTeilnahmeantrag = ["teilnahmeantragerfasst", "teilnahmeantragabgelehnt"];

    /// <summary>
    /// Ids der Offerten, deren jüngster Zustand ein Teilnahmeantrag ist (Phase 1 des selektiven Verfahrens): Sie sind noch
    /// bzw. nie Offerten geworden und zählen in den Auswertungen und im Export nicht als Offerten. Regel wie <see cref="LetzterZustand"/>.
    /// </summary>
    public static IQueryable<Guid> NurTeilnahmeantraege(DoeAContext context) =>
        from ooz in context.TOfferteOffertezustand
        where OffertezustaendeTeilnahmeantrag.Contains(ooz.OozOfz.OfzNaturalId!) &&
              ooz.OozId == context.TOfferteOffertezustand
                  .Where(o => o.OozOffId == ooz.OozOffId)
                  .OrderByDescending(o => o.OozDatum)
                  .ThenByDescending(o => o.OozInsertedDate)
                  .ThenByDescending(o => o.OozId)
                  .Select(o => o.OozId)
                  .First()
        select ooz.OozOffId;
}
