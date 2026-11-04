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

/// <summary>Stammdaten der Leistungsarten; Spaltenpräfix "Lea".</summary>
public class LeistungsartService : BaseService<Leistungsart, TLeistungsart, LeistungsartValidator>, ILeistungsartService
{
    public LeistungsartService(IErrorHandlerService errorHandlerService, IDbContextFactory<DoeAContext> dbFactory, LeistungsartMapper mapper, ILogger logger, IAktuellerBenutzer aktuellerBenutzer)
        : base(errorHandlerService, dbFactory, mapper, logger, aktuellerBenutzer, new LeistungsartValidator(), "Lea")
    {
    }

    /// <summary>Sucht über Bezeichnung und Kurzbezeichnung.</summary>
    protected override Expression<Func<TLeistungsart, bool>> SuchBedingung(string searchValue) =>
        l => l.LeaBezeichnung.ToLower().Contains(searchValue.ToLower()) ||
             l.LeaKurzbezeichnung.ToLower() == searchValue.ToLower();
}