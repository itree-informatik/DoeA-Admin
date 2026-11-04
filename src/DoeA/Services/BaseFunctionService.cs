// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Constants;
using DoeA.Data.Models.DB;
using DoeA.Interfaces;
using FluentValidation;
using ItreeMud.Models;
using Microsoft.EntityFrameworkCore;

namespace DoeA.Services;

/// <summary>
/// Querschnittsfunktionen für Services: Validierung inklusive Audit-Feldern sowie Änderungs- und
/// Concurrency-Prüfung. Der Abgleich mit der DB läuft per Reflection über das 3-Buchstaben-
/// Spaltenpräfix der Entity ("Amt" + "Bezeichnung" -> AmtBezeichnung).
/// </summary>
public class BaseFunctionService : IBaseFunctionService
{
    /// <summary>
    /// Property- und Typnamen, die nie verglichen werden: Navigations-Collections und übersetzte
    /// Anzeigetexte haben keine Entsprechung auf der Entity.
    /// </summary>
    private static readonly string[] StandardIgnorierteNamen = ["ICollection`1", "TranslatedText", "IList`1"];

    private readonly IAktuellerBenutzer _aktuellerBenutzer;
    private readonly IDbContextFactory<DoeAContext> _dbFactory;

    public BaseFunctionService(IDbContextFactory<DoeAContext> dbFactory, IAktuellerBenutzer aktuellerBenutzer)
    {
        _dbFactory = dbFactory;
        _aktuellerBenutzer = aktuellerBenutzer;
    }

    /// <summary>Benutzer für die Audit-Felder (eIAM-Identifikator); wirft ohne angemeldeten Benutzer, damit nie ein leerer Stempel in die DB gelangt.</summary>
    protected string User
    {
        get
        {
            var extId = _aktuellerBenutzer.ExtId;
            return string.IsNullOrEmpty(extId)
                ? throw new InvalidOperationException($"{GeneralConstants.NullReference}?User")
                : extId;
        }
    }

    /// <summary>Setzt Id, Aktiv-Flag und Audit-Felder per Reflection und validiert anschliessend.</summary>
    public async Task Validate<T>(AbstractValidator<T> validator, T model, bool isnew, long? date = null)
       where T : class
    {
        var type = model.GetType();
        date ??= DateTime.Now.Ticks;

        if (isnew)
        {
            type.GetProperty(nameof(Basis.Id))?.SetValue(model, Guid.NewGuid());
            type.GetProperty(nameof(Basis.Aktiv))?.SetValue(model, true);
            type.GetProperty(nameof(Basis.InsertedUser))?.SetValue(model, User);
            type.GetProperty(nameof(Basis.InsertedDate))?.SetValue(model, date);
        }

        type.GetProperty(nameof(Basis.ChangedUser))?.SetValue(model, User);
        type.GetProperty(nameof(Basis.ChangedDate))?.SetValue(model, date);

        await Validate(validator, model);
    }

    /// <summary>Validiert das Modell und wirft bei Fehlern eine Exception mit allen Meldungen.</summary>
    public async Task Validate<T>(AbstractValidator<T> validator, T model) where T : class
    {
        var result = await validator.ValidateAsync(model);
        if (result.IsValid)
        {
            return;
        }

        var fehler = string.Join(", ", result.Errors.Select(e => $"{e.ErrorMessage} ({e.PropertyName})"));
        throw new InvalidOperationException($"{GeneralConstants.ValidationException}?{fehler}");
    }

    /// <summary>
    /// Vergleicht das Modell mit dem DB-Stand und liefert <c>OK</c>, <c>NOTHING_CHANGED</c>,
    /// <c>ERROR_NOT_FOUND</c> oder <c>ERROR_CONCURRENCY_EXCEPTION</c>.
    /// <paramref name="prefix"/> ist das 3-Buchstaben-Spaltenpräfix der Entity.
    /// </summary>
    /// <remarks>
    /// Prüfung und anschliessendes Speichern laufen in getrennten DbContexts ohne DB-seitiges
    /// Concurrency-Token — das Millisekunden-Restfenster dazwischen ist bei der geringen
    /// Parallelität des Tools bewusst akzeptiert (ein xmin-Token müsste durch alle DTOs gereicht werden).
    /// </remarks>
    public async Task<string> CheckChangesAndConcurrency<TDbEntity>(
        object model,
        string prefix,
        string? ignoreProperties = null) where TDbEntity : class
    {
        var ignored = new HashSet<string>(
            (ignoreProperties?.Split(',') ?? Array.Empty<string>())
            .Concat(StandardIgnorierteNamen));

        var modelType = model.GetType();
        var id = modelType.GetProperty(nameof(Basis.Id))?.GetValue(model);
        if (id == null)
        {
            return GeneralConstants.NotFoundError;
        }

        await using var context = await _dbFactory.CreateDbContextAsync();

        var dbEntity = context.Set<TDbEntity>().Find(id);
        if (dbEntity == null)
        {
            return GeneralConstants.NotFoundError;
        }

        // Nur der Vergleichsstand ist gefragt — der Datensatz darf nicht im ChangeTracker hängen bleiben.
        context.Entry(dbEntity).State = EntityState.Detached;

        // Die Schleife muss alle Properties durchlaufen (kein Early Return bei der ersten Abweichung),
        // sonst wird ChangedDate nie erreicht und die Concurrency-Prüfung ist wirkungslos.
        var hatAenderungen = false;
        foreach (var propModel in modelType.GetProperties())
        {
            if (ignored.Contains(propModel.Name) || ignored.Contains(propModel.PropertyType.Name))
            {
                continue;
            }

            var dbProp = dbEntity.GetType().GetProperty($"{prefix}{propModel.Name}");
            if (dbProp == null)
            {
                continue;
            }

            if (AreEqual(dbProp.GetValue(dbEntity), propModel.GetValue(model), propModel.PropertyType))
            {
                continue;
            }

            // Abweichendes ChangedDate heisst: jemand anderes hat den Datensatz zwischenzeitlich gespeichert.
            if (propModel.Name == nameof(Basis.ChangedDate))
            {
                return GeneralConstants.ConcurrencyError;
            }

            hatAenderungen = true;
        }

        return hatAenderungen ? GeneralConstants.Ok : GeneralConstants.NothingChanged;
    }

    /// <summary>Wie <see cref="CheckChangesAndConcurrency{TDbEntity}"/>, meldet aber alles ausser «geändert» als Exception.</summary>
    public async Task CheckChangesAndConcurrencyVoid<TDbEntity>(object model, string prefix, string? ignoreProperties = null) where TDbEntity : class
    {
        var status = await CheckChangesAndConcurrency<TDbEntity>(model, prefix, ignoreProperties);
        if (status == GeneralConstants.Ok)
        {
            return;
        }

        if (status == GeneralConstants.ConcurrencyError)
        {
            throw new InvalidOperationException(GeneralConstants.ConcurrencyError);
        }

        var modelType = model.GetType();
        var id = modelType.GetProperty(nameof(Basis.Id))?.GetValue(model);
        throw new InvalidOperationException($"{status}?{modelType.Name} {id}");
    }

    /// <summary>
    /// Concurrency-Prüfung für Batch-Saves: wirft <c>ERROR_CONCURRENCY</c>, wenn der Datensatz seit dem
    /// Laden von jemand anderem gespeichert wurde. Muss vor <see cref="StampForSave"/> laufen —
    /// das Stempeln überschreibt das zu vergleichende ChangedDate.
    /// </summary>
    protected static void PruefeConcurrency(Basis model, long dbChangedDate)
    {
        if (model.ChangedDate != dbChangedDate)
        {
            throw new InvalidOperationException(GeneralConstants.ConcurrencyError);
        }
    }

    /// <summary>
    /// Setzt die Audit-Felder für einen Batch-Save. Im Gegensatz zur Überladung <see cref="Validate{T}(AbstractValidator{T}, T, bool, long?)"/>
    /// wird eine bereits vergebene <c>Id</c> nicht überschrieben – wichtig, wenn neue Datensätze schon client-seitig
    /// eine Guid besitzen (damit Kind-Datensätze in-memory auf den Parent verweisen können).
    /// </summary>
    protected void StampForSave(Basis model, bool isNew, long date)
    {
        if (isNew)
        {
            if (model.Id == Guid.Empty)
            {
                model.Id = Guid.NewGuid();
            }

            model.Aktiv = true;
            model.InsertedUser = User;
            model.InsertedDate = date;
        }

        model.ChangedUser = User;
        model.ChangedDate = date;
    }

    private static bool AreEqual(object? original, object? updated, Type type)
    {
        if (original == null && updated == null)
        {
            return true;
        }

        if (original == null || updated == null)
        {
            return false;
        }

        // Enums kommen je nach Quelle als Enum oder als Zahl an, deshalb über den Zahlenwert vergleichen.
        return type.IsEnum
            ? Convert.ToInt32(original).Equals(Convert.ToInt32(updated))
            : original.Equals(updated);
    }
}
