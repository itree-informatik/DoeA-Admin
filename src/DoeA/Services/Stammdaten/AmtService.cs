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

/// <summary>Stammdaten der Ämter; Spaltenpräfix "Amt".</summary>
public class AmtService : BaseService<Amt, TAmt, AmtValidator>, IAmtService
{
    public AmtService(IErrorHandlerService errorHandlerService, IDbContextFactory<DoeAContext> dbFactory, AmtMapper mapper, ILogger logger, IAktuellerBenutzer aktuellerBenutzer)
        : base(errorHandlerService, dbFactory, mapper, logger, aktuellerBenutzer, new AmtValidator(), "Amt")
    {
    }

    protected override IQueryable<TAmt> MitNavigationen(IQueryable<TAmt> queryable) =>
        queryable.Include(a => a.AmtDep);

    /// <summary>Sucht über Bezeichnung, Kurzbezeichnung, Dienststellennummer und das zugehörige Departement.</summary>
    protected override Expression<Func<TAmt, bool>> SuchBedingung(string searchValue) =>
        a => a.AmtBezeichnung.ToLower().Contains(searchValue.ToLower()) ||
             a.AmtKurzbezeichnung.ToLower() == searchValue.ToLower() ||
             a.AmtDienststellennummer.ToLower() == searchValue.ToLower() ||
             a.AmtDep.DepBezeichnung.ToLower().Contains(searchValue.ToLower()) ||
             a.AmtDep.DepKurzbezeichnung.ToLower() == searchValue.ToLower();
}