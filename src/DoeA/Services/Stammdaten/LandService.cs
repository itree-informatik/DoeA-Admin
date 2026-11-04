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

/// <summary>Stammdaten der Länder; Spaltenpräfix "Lan".</summary>
public class LandService : BaseService<Land, TLand, LandValidator>, ILandService
{
    public LandService(IErrorHandlerService errorHandlerService, IDbContextFactory<DoeAContext> dbFactory, LandMapper mapper, ILogger logger, IAktuellerBenutzer aktuellerBenutzer)
        : base(errorHandlerService, dbFactory, mapper, logger, aktuellerBenutzer, new LandValidator(), "Lan")
    {
    }

    /// <summary>Sucht über Bezeichnung und Kurzbezeichnung.</summary>
    protected override Expression<Func<TLand, bool>> SuchBedingung(string searchValue) =>
        l => l.LanBezeichnung.ToLower().Contains(searchValue.ToLower()) ||
             l.LanKurzbezeichnung.ToLower() == searchValue.ToLower();
}