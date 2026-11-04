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

/// <summary>Stammdaten der Sprachen; Spaltenpräfix "Spr".</summary>
public class SpracheService : BaseService<Sprache, TSprache, SpracheValidator>, ISpracheService
{
    public SpracheService(IErrorHandlerService errorHandlerService, IDbContextFactory<DoeAContext> dbFactory, SpracheMapper mapper, ILogger logger, IAktuellerBenutzer aktuellerBenutzer)
        : base(errorHandlerService, dbFactory, mapper, logger, aktuellerBenutzer, new SpracheValidator(), "Spr")
    {
    }

    /// <summary>Sucht über die Bezeichnung.</summary>
    protected override Expression<Func<TSprache, bool>> SuchBedingung(string searchValue) =>
        s => s.SprBezeichnung.ToLower().Contains(searchValue.ToLower());
}