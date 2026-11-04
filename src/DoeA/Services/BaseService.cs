// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Constants;
using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO;
using DoeA.Interfaces;
using FluentValidation;
using ItreeMud.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using ILogger = Serilog.ILogger;

namespace DoeA.Services;

/// <summary>
/// Generisches CRUD für alle Entity-Services. Die Zuordnung DTO-Property -> DB-Spalte läuft über das
/// 3-Buchstaben-Präfix der Entity (z.B. "Amt" -> AmtAktiv, AmtBezeichnung). Weicht eine Entity vom
/// Präfixschema ab, greifen Aktiv-Filter, Sortierung und Concurrency-Prüfung stillschweigend nicht mehr.
/// </summary>
public abstract class BaseService<TModel, TEntity, TValidator> : BaseFunctionService, IBaseService<TModel>
    where TModel : Basis
    where TEntity : class
    where TValidator : AbstractValidator<TModel>
{
    private const string AktivSuffix = "Aktiv";
    private const string NachnameSuffix = "Nachname";
    private const string VornameSuffix = "Vorname";

    /// <summary>
    /// Property-Suffixe der Standardsortierung von <see cref="GetAll"/> in absteigender Priorität:
    /// Es gewinnt das erste Suffix, zu dem die Entity eine Property besitzt.
    /// </summary>
    private static readonly string[] SortierSuffixe =
        ["Sortierung", "Bezeichnung", "Name", "NummerStr", "Ort", NachnameSuffix];

    private readonly IErrorHandlerService _errorHandlerService;
    private readonly string _prefix;
    private readonly TValidator _validator;

    public BaseService(IErrorHandlerService errorHandlerService, IDbContextFactory<DoeAContext> dbFactory, IEntityMapper<TModel, TEntity> mapper, ILogger logger, IAktuellerBenutzer aktuellerBenutzer, TValidator validatorFactory, string prefix) : base(dbFactory, aktuellerBenutzer)
    {
        _validator = validatorFactory;
        _errorHandlerService = errorHandlerService;
        _prefix = prefix;
        Mapper = mapper;
        Logger = logger;
        DbFactory = dbFactory;
    }

    /// <summary>Factory für kurzlebige DbContexte – pro Operation wird ein eigener Context erzeugt.</summary>
    protected IDbContextFactory<DoeAContext> DbFactory { get; }

    /// <summary>Logger für abgeleitete Services.</summary>
    protected ILogger Logger { get; }

    /// <summary>Mapping zwischen DB-Entity und DTO.</summary>
    protected IEntityMapper<TModel, TEntity> Mapper { get; }

    /// <summary>Entity-Properties, die ein Update zusätzlich zu den Erfassungsdaten nie zurückschreibt (Standard: keine).</summary>
    protected virtual IEnumerable<string> GeschuetzteProperties => [];

    /// <summary>Liefert alle aktiven Datensätze, sortiert nach der ersten passenden Sortier-Eigenschaft.</summary>
    public virtual async Task<List<TModel>?> GetAll()
    {
        return await _errorHandlerService.HandleListAsync(async () =>
        {
            await using var dbContext = await DbFactory.CreateDbContextAsync();
            var queryable = Sortiere(FiltereAktive(MitNavigationen(dbContext.Set<TEntity>().AsNoTracking())));
            var entities = await queryable.ToListAsync();

            return new ServiceResult<TModel>
            {
                Items = entities.Select(Mapper.ZuModel).ToList(),
                Result = GeneralConstants.Ok
            };
        });
    }

    /// <summary>Liefert den Datensatz zur Id; wirft, wenn er nicht existiert.</summary>
    public async Task<TModel> GetById(Guid id)
    {
        await using var dbContext = await DbFactory.CreateDbContextAsync();

        // Kein FindAsync: Es kennt keine Includes, und das DTO käme ohne die Navigationen zurück,
        // die GetAll und Search mitliefern.
        var schluessel = dbContext.Model.FindEntityType(typeof(TEntity))!.FindPrimaryKey()!.Properties[0].Name;
        var entity = await MitNavigationen(dbContext.Set<TEntity>().AsNoTracking())
            .FirstOrDefaultAsync(e => EF.Property<Guid>(e, schluessel) == id);

        if (entity == null)
        {
            throw new InvalidOperationException($"{GeneralConstants.NotFoundError}?id={id}");
        }

        return Mapper.ZuModel(entity);
    }

    /// <summary>Validiert das Modell, setzt die Audit-Felder und legt den Datensatz an.</summary>
    public async Task<TModel> Create(TModel model)
    {
        await Validate(_validator, model, true);

        await using var dbContext = await DbFactory.CreateDbContextAsync();
        var entity = Mapper.ZuEntity(model);
        dbContext.Set<TEntity>().Add(entity);
        await dbContext.SaveChangesAsync();

        return Mapper.ZuModel(entity);
    }

    /// <summary>
    /// Speichert Änderungen, sofern es welche gibt und kein Concurrency-Konflikt vorliegt.
    /// <paramref name="ignoreList"/> nimmt kommagetrennte Eigenschaften, die beim Vergleich übersprungen werden.
    /// </summary>
    public async Task<TModel> Update(TModel model, string ignoreList = "")
    {
        var status = await CheckChangesAndConcurrency<TEntity>(model, _prefix, ignoreList);

        if (status == GeneralConstants.NothingChanged)
        {
            return model;
        }

        if (status == GeneralConstants.ConcurrencyError)
        {
            throw new InvalidOperationException(GeneralConstants.ConcurrencyError);
        }

        // Verbleibende Fehlerstatus (z.B. ERROR_NOT_FOUND) dürfen nicht in den Speicherpfad durchfallen.
        if (status != GeneralConstants.Ok)
        {
            throw new InvalidOperationException($"{status}?{typeof(TModel).Name} {model.Id}");
        }

        await Validate(_validator, model, false);

        var entity = Mapper.ZuEntity(model);
        await using var dbContext = await DbFactory.CreateDbContextAsync();
        var eintrag = dbContext.Entry(entity);
        eintrag.State = EntityState.Modified;
        SchuetzeErfassungsdaten(eintrag);
        await dbContext.SaveChangesAsync();

        return model;
    }

    /// <summary>Löscht den Datensatz zur Id und liefert ihn zurück.</summary>
    public virtual async Task<TModel> Delete(Guid id)
    {
        using var context = await DbFactory.CreateDbContextAsync();
        var record = await context.Set<TEntity>().FindAsync(id);
        if (record == null)
        {
            throw new InvalidOperationException($"{GeneralConstants.NullReference}?record");
        }

        context.Set<TEntity>().Remove(record);
        try
        {
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Logger.Error(ex, GeneralConstants.MessageTemplate, nameof(BaseService<TModel, TEntity, TValidator>), nameof(Delete), ex.Message);
            throw new InvalidOperationException(GeneralConstants.HandledException);
        }

        return Mapper.ZuModel(record);
    }

    /// <summary>
    /// Sucht Datensätze der Entity: optional auf aktive eingeschränkt, über <see cref="SuchBedingung"/>
    /// gefiltert und wie <see cref="GetAll"/> sortiert. Services ohne <see cref="SuchBedingung"/>
    /// liefern die (aktiven) Datensätze ohne Textfilter.
    /// </summary>
    public virtual async Task<ObservableCollection<TModel>> Search(string searchValue, bool onlyActive)
    {
        await using var dbContext = await DbFactory.CreateDbContextAsync();
        var queryable = MitNavigationen(dbContext.Set<TEntity>().AsNoTracking());

        if (onlyActive)
        {
            queryable = FiltereAktive(queryable);
        }

        if (!string.IsNullOrWhiteSpace(searchValue) && SuchBedingung(searchValue) is { } bedingung)
        {
            queryable = queryable.Where(bedingung);
        }

        var entities = await Sortiere(queryable).ToListAsync();
        return new ObservableCollection<TModel>(entities.Select(Mapper.ZuModel));
    }

    /// <summary>
    /// Felder, über die <see cref="Search"/> sucht. <c>null</c> heisst: Der Service unterstützt
    /// keine Textsuche, ein Suchbegriff wird dann ignoriert.
    /// </summary>
    protected virtual Expression<Func<TEntity, bool>>? SuchBedingung(string searchValue) => null;

    /// <summary>
    /// Navigationen, die <see cref="SuchBedingung"/> oder das Mapping benötigen (Standard: keine).
    /// </summary>
    protected virtual IQueryable<TEntity> MitNavigationen(IQueryable<TEntity> queryable) => queryable;

    /// <summary>
    /// Nimmt die Erfassungsdaten und die <see cref="GeschuetzteProperties"/> vom Update aus.
    /// <c>EntityState.Modified</c> schreibt sonst alle Spalten aus dem DTO zurück — ein unvollständig
    /// befülltes DTO würde damit InsertedDate/InsertedUser überschreiben. Die Werte stammen aus der DB
    /// und dürfen sich nach dem Anlegen nicht mehr ändern.
    /// </summary>
    protected void SchuetzeErfassungsdaten(EntityEntry eintrag)
    {
        var namen = new[] { $"{_prefix}{nameof(Basis.InsertedDate)}", $"{_prefix}{nameof(Basis.InsertedUser)}" }
            .Concat(GeschuetzteProperties);
        foreach (var name in namen)
        {
            var property = eintrag.Properties.FirstOrDefault(p => p.Metadata.Name == name);
            if (property != null)
            {
                property.IsModified = false;
            }
        }
    }

    /// <summary>Beschränkt die Abfrage auf aktive Datensätze, sofern die Entity ein Aktiv-Flag besitzt.</summary>
    private IQueryable<TEntity> FiltereAktive(IQueryable<TEntity> queryable)
    {
        var aktivProperty = typeof(TEntity).GetProperty($"{_prefix}{AktivSuffix}");
        if (aktivProperty == null || aktivProperty.PropertyType != typeof(bool))
        {
            return queryable;
        }

        var parameter = Expression.Parameter(typeof(TEntity), "e");
        var propertyAccess = Expression.MakeMemberAccess(parameter, aktivProperty);
        var condition = Expression.Equal(propertyAccess, Expression.Constant(true));

        return queryable.Where(Expression.Lambda<Func<TEntity, bool>>(condition, parameter));
    }

    /// <summary>Sortiert nach dem ersten vorhandenen Sortier-Suffix, bei Nachname zusätzlich nach Vorname.</summary>
    private IQueryable<TEntity> Sortiere(IQueryable<TEntity> queryable)
    {
        var sortPropertyName = SortierSuffixe
            .Select(suffix => $"{_prefix}{suffix}")
            .FirstOrDefault(name => typeof(TEntity).GetProperty(name) != null);

        if (sortPropertyName == null)
        {
            return queryable;
        }

        queryable = OrderByProperty(queryable, sortPropertyName);

        return sortPropertyName.EndsWith(NachnameSuffix)
            ? OrderByProperty(queryable, $"{_prefix}{VornameSuffix}", thenBy: true)
            : queryable;
    }

    /// <summary>
    /// Sortiert nach einer erst zur Laufzeit bekannten Property; unbekannte Properties lassen
    /// die Abfrage unverändert. <c>OrderBy</c>/<c>ThenBy</c> wird per Reflection generisch geschlossen,
    /// weil der Typ der Sortier-Property nicht statisch bekannt ist.
    /// </summary>
    private static IQueryable<T> OrderByProperty<T>(IQueryable<T> query, string propertyName, bool thenBy = false)
    {
        var property = typeof(T).GetProperty(propertyName);
        if (property == null)
        {
            return query;
        }

        var parameter = Expression.Parameter(typeof(T), "e");
        var propertyAccess = Expression.MakeMemberAccess(parameter, property);
        var lambda = Expression.Lambda(propertyAccess, parameter);

        var methodName = thenBy ? nameof(Queryable.ThenBy) : nameof(Queryable.OrderBy);
        var method = typeof(Queryable).GetMethods()
            .Single(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), property.PropertyType);

        return (IQueryable<T>)method.Invoke(null, [query, lambda])!;
    }
}
