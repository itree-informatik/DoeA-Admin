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

/// <summary>Stammdaten der Projektarten; Spaltenpräfix "Pra".</summary>
public class ProjektartService : BaseService<Projektart, TProjektart, ProjektartValidator>, IProjektartService
{
    public ProjektartService(IErrorHandlerService errorHandlerService, IDbContextFactory<DoeAContext> dbFactory, ProjektartMapper mapper, ILogger logger, IAktuellerBenutzer aktuellerBenutzer)
        : base(errorHandlerService, dbFactory, mapper, logger, aktuellerBenutzer, new ProjektartValidator(), "Pra")
    {
    }

    // Über die NaturalId applikationsrelevant: bleibt immer aktiv, Aktiv wird beim Update nie zurückgeschrieben.
    protected override IEnumerable<string> GeschuetzteProperties => [nameof(TProjektart.PraAktiv)];

    /// <summary>Sucht über Bezeichnung und Kurzbezeichnung.</summary>
    protected override Expression<Func<TProjektart, bool>> SuchBedingung(string searchValue) =>
        p => p.PraBezeichnung.ToLower().Contains(searchValue.ToLower()) ||
             p.PraKurzbezeichnung.ToLower() == searchValue.ToLower();
}