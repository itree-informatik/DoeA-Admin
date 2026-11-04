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

/// <summary>Stammdaten der Offertezustände; Spaltenpräfix "Ofz".</summary>
public class OffertezustandService : BaseService<Offertezustand, TOffertezustand, OffertezustandValidator>, IOffertezustandService
{
    public OffertezustandService(IErrorHandlerService errorHandlerService, IDbContextFactory<DoeAContext> dbFactory, OffertezustandMapper mapper, ILogger logger, IAktuellerBenutzer aktuellerBenutzer)
        : base(errorHandlerService, dbFactory, mapper, logger, aktuellerBenutzer, new OffertezustandValidator(), "Ofz")
    {
    }

    // Über die NaturalId applikationsrelevant: bleibt immer aktiv, Aktiv wird beim Update nie zurückgeschrieben.
    protected override IEnumerable<string> GeschuetzteProperties => [nameof(TOffertezustand.OfzAktiv)];

    /// <summary>Sucht über Bezeichnung und Kurzbezeichnung.</summary>
    protected override Expression<Func<TOffertezustand, bool>> SuchBedingung(string searchValue) =>
        o => o.OfzBezeichnung.ToLower().Contains(searchValue.ToLower()) ||
             o.OfzKurzbezeichnung.ToLower() == searchValue.ToLower();
}