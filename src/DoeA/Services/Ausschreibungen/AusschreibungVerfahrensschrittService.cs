// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Mapper;
using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO;
using DoeA.Data.Models.DTO.Stammdaten;
using DoeA.Interfaces;
using DoeA.Validator;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using ILogger = Serilog.ILogger;

namespace DoeA.Services.Ausschreibungen;

/// <summary>Lädt und speichert die Terminplanung einer Ausschreibung (Verfahrensschritte mit Datum von/bis).</summary>
public class AusschreibungVerfahrensschrittService : BaseService<AusschreibungVerfahrensschritt, TAusschreibungVerfahrensschritt,
    AusschreibungVerfahrensschrittValidator>, IAusschreibungVerfahrensschrittService
{
    public AusschreibungVerfahrensschrittService(IErrorHandlerService errorHandlerService, IDbContextFactory<DoeAContext> dbFactory,
        AusschreibungVerfahrensschrittMapper mapper, ILogger logger, IAktuellerBenutzer aktuellerBenutzer)
        : base(errorHandlerService, dbFactory, mapper, logger, aktuellerBenutzer, new AusschreibungVerfahrensschrittValidator(), "Avs")
    {
    }

    /// <summary>Lädt die Verfahrensschritte einer Ausschreibung in der Reihenfolge der Stammdaten-Sortierung, für App und PDF gleich.</summary>
    public async Task<ObservableCollection<AusschreibungVerfahrensschritt>> Load(Guid ausId)
    {
        await using var dbContext = await DbFactory.CreateDbContextAsync();
        // Datum, Bezeichnung und Id ordnen gleiche Sortierungen eindeutig, damit App und PDF übereinstimmen.
        var query = from avs in dbContext.TAusschreibungVerfahrensschritt
            join ves in dbContext.TVerfahrensschritt on avs.AvsVesId equals ves.VesId
            where avs.AvsAusId == ausId
            orderby ves.VesSortierung, avs.AvsDatumVon, ves.VesBezeichnung, avs.AvsId
            select new AusschreibungVerfahrensschritt
            {
                Id = avs.AvsId,
                AusId = avs.AvsAusId,
                VesId = avs.AvsVesId,
                DatumVon = avs.AvsDatumVon,
                DatumBis = avs.AvsDatumBis,
                InsertedDate = avs.AvsInsertedDate,
                InsertedUser = avs.AvsInsertedUser,
                ChangedDate = avs.AvsChangedDate,
                ChangedUser = avs.AvsChangedUser,
                Verfahrensschritt = new Verfahrensschritt
                {
                    Id = ves.VesId,
                    Bezeichnung = ves.VesBezeichnung,
                    NaturalId = ves.VesNaturalId!,
                    Sortierung = ves.VesSortierung,
                    NurWerktage = ves.VesNurWerktage,
                }
            };

        // Kein Distinct nötig: der Join geht auf den Primärschlüssel des Verfahrensschritts,
        // pro Zeile in T_AusschreibungVerfahrensschritt entsteht also genau ein Ergebnis.
        return new ObservableCollection<AusschreibungVerfahrensschritt>(await query.ToListAsync());
    }

    /// <summary>
    /// Speichert die Terminplanung einer Ausschreibung gesamthaft: fügt neue Verfahrensschritte ein,
    /// aktualisiert geänderte und entfernt nicht mehr vorhandene – alles in einem <c>SaveChanges</c>.
    /// "Neu" wird an der Abwesenheit der client-seitig vergebenen Id in der DB erkannt.
    /// Unveränderte Datensätze werden übersprungen (kein Neustempeln von ChangedDate); geänderte werden
    /// pro Datensatz auf Concurrency geprüft. Löschungen sind bewusst Last-write-wins.
    /// </summary>
    public async Task<ObservableCollection<AusschreibungVerfahrensschritt>> SaveAll(Guid ausId,
        IEnumerable<AusschreibungVerfahrensschritt> items)
    {
        var list = items.ToList();

        // Sicherheitsnetz zur Validierung in der Seite: jeder Verfahrensschritt höchstens einmal je Terminplanung.
        if (list.Where(i => i.VesId != Guid.Empty).GroupBy(i => i.VesId).Any(g => g.Count() > 1))
        {
            throw new InvalidOperationException("ERROR_TERMINPLANUNG_VERFAHRENSSCHRITT_DOPPELT");
        }

        await using var dbContext = await DbFactory.CreateDbContextAsync();

        var bestand = await dbContext.TAusschreibungVerfahrensschritt
            .Where(a => a.AvsAusId == ausId)
            .ToListAsync();
        var bestandIds = bestand.Select(e => e.AvsId).ToHashSet();
        var behaltenIds = list.Select(i => i.Id).ToHashSet();

        dbContext.TAusschreibungVerfahrensschritt.RemoveRange(
            bestand.Where(e => !behaltenIds.Contains(e.AvsId)));

        var now = DateTime.Now.Ticks;
        foreach (var item in list)
        {
            item.AusId = ausId;
            var tAvs = item.Id == Guid.Empty || !bestandIds.Contains(item.Id)
                ? null
                : bestand.Single(e => e.AvsId == item.Id);

            if (tAvs == null)
            {
                StampForSave(item, true, now);
                await Validate(new AusschreibungVerfahrensschrittValidator(), item);
                dbContext.TAusschreibungVerfahrensschritt.Add(Mapper.ZuEntity(item));
            }
            else if (item.VesId != tAvs.AvsVesId || item.DatumVon != tAvs.AvsDatumVon || item.DatumBis != tAvs.AvsDatumBis)
            {
                PruefeConcurrency(item, tAvs.AvsChangedDate);
                StampForSave(item, false, now);
                await Validate(new AusschreibungVerfahrensschrittValidator(), item);
                tAvs.AvsVesId = item.VesId;
                tAvs.AvsDatumVon = item.DatumVon;
                tAvs.AvsDatumBis = item.DatumBis;
                tAvs.AvsChangedDate = item.ChangedDate;
                tAvs.AvsChangedUser = item.ChangedUser!;
            }
        }

        await dbContext.SaveChangesAsync();

        return await Load(ausId);
    }
}
