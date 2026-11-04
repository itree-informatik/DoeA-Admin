// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Mapper.Stammdaten;
using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO.Stammdaten;
using DoeA.Interfaces;
using DoeA.Interfaces.Stammdaten;
using DoeA.Validator.Stammdaten;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ILogger = Serilog.ILogger;

namespace DoeA.Services.Stammdaten;

/// <summary>Stammdaten der Kantone; Spaltenpräfix "Kan".</summary>
public class KantonService : BaseService<Kanton, TKanton, KantonValidator>, IKantonService
{
    public KantonService(IErrorHandlerService errorHandlerService, IDbContextFactory<DoeAContext> dbFactory, KantonMapper mapper, ILogger logger, IAktuellerBenutzer aktuellerBenutzer)
        : base(errorHandlerService, dbFactory, mapper, logger, aktuellerBenutzer, new KantonValidator(), "Kan")
    {
    }

    protected override IQueryable<TKanton> MitNavigationen(IQueryable<TKanton> queryable) =>
        queryable.Include(k => k.KanSpr);

    /// <summary>Sucht über Bezeichnung, Kurzbezeichnung und die zugehörige Sprache.</summary>
    protected override Expression<Func<TKanton, bool>> SuchBedingung(string searchValue) =>
        k => k.KanBezeichnung.ToLower().Contains(searchValue.ToLower()) ||
             k.KanKurzbezeichnung.ToLower() == searchValue.ToLower() ||
             k.KanSpr.SprBezeichnung.ToLower().Contains(searchValue.ToLower());
}