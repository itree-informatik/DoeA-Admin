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

/// <summary>Stammdaten der CPV-Nummern (Common Procurement Vocabulary); Spaltenpräfix "Cpv".</summary>
public class CpvNummerService : BaseService<CpvNummer, TCpvNummer, CpvNummerValidator>, ICpvNummerService
{
    public CpvNummerService(IErrorHandlerService errorHandlerService, IDbContextFactory<DoeAContext> dbFactory, CpvNummerMapper mapper, ILogger logger, IAktuellerBenutzer aktuellerBenutzer)
        : base(errorHandlerService, dbFactory, mapper, logger, aktuellerBenutzer, new CpvNummerValidator(), "Cpv")
    {
    }

    /// <summary>Sucht über Bezeichnung, formatierte Nummer und Nummer.</summary>
    protected override Expression<Func<TCpvNummer, bool>> SuchBedingung(string searchValue) =>
        c => c.CpvBezeichnung.ToLower().Contains(searchValue.ToLower()) ||
             c.CpvNummerStr.ToLower().Contains(searchValue.ToLower()) ||
             c.CpvNummer.ToString() == searchValue;
}