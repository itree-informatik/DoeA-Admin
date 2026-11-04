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

/// <summary>Stammdaten der Ressorts; Spaltenpräfix "Res".</summary>
public class RessortService : BaseService<Ressort, TRessort, RessortValidator>, IRessortService
{
    public RessortService(IErrorHandlerService errorHandlerService, IDbContextFactory<DoeAContext> dbFactory, RessortMapper mapper, ILogger logger, IAktuellerBenutzer aktuellerBenutzer)
        : base(errorHandlerService, dbFactory, mapper, logger, aktuellerBenutzer, new RessortValidator(), "Res")
    {
    }

    protected override IQueryable<TRessort> MitNavigationen(IQueryable<TRessort> queryable) =>
        queryable.Include(r => r.ResAmt);

    /// <summary>Sucht über Bezeichnung, Kurzbezeichnung und das zugehörige Amt.</summary>
    protected override Expression<Func<TRessort, bool>> SuchBedingung(string searchValue) =>
        r => r.ResBezeichnung.ToLower().Contains(searchValue.ToLower()) ||
             r.ResKurzbezeichnung.ToLower() == searchValue.ToLower() ||
             r.ResAmt.AmtBezeichnung.ToLower().Contains(searchValue.ToLower()) ||
             r.ResAmt.AmtKurzbezeichnung.ToLower() == searchValue.ToLower();
}