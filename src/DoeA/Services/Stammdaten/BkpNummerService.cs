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

/// <summary>Stammdaten der BKP-Nummern (Baukostenplan); Spaltenpräfix "Bkp".</summary>
public class BkpNummerService : BaseService<BkpNummer, TBkpNummer, BkpNummerValidator>, IBkpNummerService
{
    public BkpNummerService(IErrorHandlerService errorHandlerService, IDbContextFactory<DoeAContext> dbFactory, BkpNummerMapper mapper, ILogger logger, IAktuellerBenutzer aktuellerBenutzer)
        : base(errorHandlerService, dbFactory, mapper, logger, aktuellerBenutzer, new BkpNummerValidator(), "Bkp")
    {
    }

    /// <summary>Sucht über Bezeichnung, formatierte Nummer und Nummer.</summary>
    protected override Expression<Func<TBkpNummer, bool>> SuchBedingung(string searchValue) =>
        b => b.BkpBezeichnung.ToLower().Contains(searchValue.ToLower()) ||
             b.BkpNummerStr.ToLower().Contains(searchValue.ToLower()) ||
             b.BkpNummer.ToString() == searchValue;
}