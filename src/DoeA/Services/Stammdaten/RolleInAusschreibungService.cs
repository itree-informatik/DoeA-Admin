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

/// <summary>Stammdaten der Rollen in einer Ausschreibung; Spaltenpräfix "Ria".</summary>
public class RolleInAusschreibungService :
    BaseService<RolleInAusschreibung, TRolleInAusschreibung, RolleInAusschreibungValidator>,
    IRolleInAusschreibungService
{
    public RolleInAusschreibungService(IErrorHandlerService errorHandlerService, IDbContextFactory<DoeAContext> dbFactory, RolleInAusschreibungMapper mapper, ILogger logger, IAktuellerBenutzer aktuellerBenutzer)
        : base(errorHandlerService, dbFactory, mapper, logger, aktuellerBenutzer, new RolleInAusschreibungValidator(), "Ria")
    {
    }

    // Über die NaturalId applikationsrelevant: bleibt immer aktiv, Aktiv wird beim Update nie zurückgeschrieben.
    protected override IEnumerable<string> GeschuetzteProperties => [nameof(TRolleInAusschreibung.RiaAktiv)];

    /// <summary>Sucht über Bezeichnung und Kurzbezeichnung.</summary>
    protected override Expression<Func<TRolleInAusschreibung, bool>> SuchBedingung(string searchValue) =>
        r => r.RiaBezeichnung.ToLower().Contains(searchValue.ToLower()) ||
             r.RiaKurzbezeichnung.ToLower() == searchValue.ToLower();
}