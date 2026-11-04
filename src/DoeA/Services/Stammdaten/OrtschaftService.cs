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

/// <summary>Stammdaten der Ortschaften; Spaltenpräfix "Ort".</summary>
public class OrtschaftService : BaseService<Ortschaft, TOrtschaft, OrtschaftValidator>, IOrtschaftService
{
    public OrtschaftService(IErrorHandlerService errorHandlerService, IDbContextFactory<DoeAContext> dbFactory, OrtschaftMapper mapper, ILogger logger, IAktuellerBenutzer aktuellerBenutzer)
        : base(errorHandlerService, dbFactory, mapper, logger, aktuellerBenutzer, new OrtschaftValidator(), "Ort")
    {
    }

    protected override IQueryable<TOrtschaft> MitNavigationen(IQueryable<TOrtschaft> queryable) =>
        queryable.Include(o => o.OrtLan).Include(o => o.OrtKan);

    /// <summary>Sucht über Ort, PLZ und das zugehörige Land.</summary>
    protected override Expression<Func<TOrtschaft, bool>> SuchBedingung(string searchValue) =>
        o => o.OrtOrt.ToLower().Contains(searchValue.ToLower()) ||
             o.OrtPlz.ToLower() == searchValue.ToLower() ||
             o.OrtLan.LanBezeichnung.ToLower().Contains(searchValue.ToLower()) ||
             o.OrtLan.LanKurzbezeichnung.ToLower() == searchValue.ToLower();
}