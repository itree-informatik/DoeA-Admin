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

/// <summary>Stammdaten der Anbieter; Spaltenpräfix "Anb".</summary>
public class AnbieterService : BaseService<Anbieter, TAnbieter, AnbieterValidator>, IAnbieterService
{
    public AnbieterService(IErrorHandlerService errorHandlerService, IDbContextFactory<DoeAContext> dbFactory, AnbieterMapper mapper, ILogger logger, IAktuellerBenutzer aktuellerBenutzer)
        : base(errorHandlerService, dbFactory, mapper, logger, aktuellerBenutzer, new AnbieterValidator(), "Anb")
    {
    }

    protected override IQueryable<TAnbieter> MitNavigationen(IQueryable<TAnbieter> queryable) =>
        queryable.Include(a => a.AnbOrt);

    /// <summary>Sucht über Name, alle Adresszeilen sowie Ort und PLZ der Ortschaft.</summary>
    protected override Expression<Func<TAnbieter, bool>> SuchBedingung(string searchValue) =>
        a => a.AnbName.ToLower().Contains(searchValue.ToLower()) ||
             a.AnbAdresse.ToLower().Contains(searchValue.ToLower()) ||
             (a.AnbAdresse2 != null && a.AnbAdresse2.ToLower().Contains(searchValue.ToLower())) ||
             (a.AnbAdresse3 != null && a.AnbAdresse3.ToLower().Contains(searchValue.ToLower())) ||
             a.AnbOrt.OrtOrt.ToLower().Contains(searchValue.ToLower()) ||
             a.AnbOrt.OrtPlz.ToLower().Contains(searchValue.ToLower());
}