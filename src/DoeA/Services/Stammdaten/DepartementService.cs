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

/// <summary>Stammdaten der Departemente; Spaltenpräfix "Dep".</summary>
public class DepartementService : BaseService<Departement, TDepartement, DepartementValidator>, IDepartementService
{
    public DepartementService(IErrorHandlerService errorHandlerService, IDbContextFactory<DoeAContext> dbFactory, DepartementMapper mapper, ILogger logger, IAktuellerBenutzer aktuellerBenutzer)
        : base(errorHandlerService, dbFactory, mapper, logger, aktuellerBenutzer, new DepartementValidator(), "Dep")
    {
    }

    /// <summary>Sucht über Bezeichnung und Kurzbezeichnung.</summary>
    protected override Expression<Func<TDepartement, bool>> SuchBedingung(string searchValue) =>
        d => d.DepBezeichnung.ToLower().Contains(searchValue.ToLower()) ||
             d.DepKurzbezeichnung.ToLower() == searchValue.ToLower();
}