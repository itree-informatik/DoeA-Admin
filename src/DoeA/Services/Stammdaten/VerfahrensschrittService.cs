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

/// <summary>Stammdaten der Verfahrensschritte; Spaltenpräfix "Ves".</summary>
public class VerfahrensschrittService : BaseService<Verfahrensschritt, TVerfahrensschritt, VerfahrensschrittValidator>, IVerfahrensschrittService
{
    public VerfahrensschrittService(IErrorHandlerService errorHandlerService, IDbContextFactory<DoeAContext> dbFactory, VerfahrensschrittMapper mapper, ILogger logger, IAktuellerBenutzer aktuellerBenutzer)
        : base(errorHandlerService, dbFactory, mapper, logger, aktuellerBenutzer, new VerfahrensschrittValidator(), "Ves")
    {
    }

    /// <summary>Sucht über die Bezeichnung.</summary>
    protected override Expression<Func<TVerfahrensschritt, bool>> SuchBedingung(string searchValue) =>
        v => v.VesBezeichnung.ToLower().Contains(searchValue.ToLower());
}