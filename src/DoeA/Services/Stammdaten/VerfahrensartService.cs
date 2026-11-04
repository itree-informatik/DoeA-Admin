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

/// <summary>Stammdaten der Verfahrensarten; Spaltenpräfix "Vea".</summary>
public class VerfahrensartService : BaseService<Verfahrensart, TVerfahrensart, VerfahrensartValidator>, IVerfahrensartService
{
    public VerfahrensartService(IErrorHandlerService errorHandlerService, IDbContextFactory<DoeAContext> dbFactory, VerfahrensartMapper mapper, ILogger logger, IAktuellerBenutzer aktuellerBenutzer)
        : base(errorHandlerService, dbFactory, mapper, logger, aktuellerBenutzer, new VerfahrensartValidator(), "Vea")
    {
    }

    // Über die NaturalId applikationsrelevant: bleibt immer aktiv, Aktiv wird beim Update nie zurückgeschrieben.
    protected override IEnumerable<string> GeschuetzteProperties => [nameof(TVerfahrensart.VeaAktiv)];

    /// <summary>Sucht über Bezeichnung und Kurzbezeichnung.</summary>
    protected override Expression<Func<TVerfahrensart, bool>> SuchBedingung(string searchValue) =>
        v => v.VeaBezeichnung.ToLower().Contains(searchValue.ToLower()) ||
             v.VeaKurzbezeichnung.ToLower() == searchValue.ToLower();
}