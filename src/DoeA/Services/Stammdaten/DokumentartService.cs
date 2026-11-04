// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Constants;
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

/// <summary>Stammdaten der Dokumentarten; Spaltenpräfix "Doa".</summary>
public class DokumentartService : BaseService<Dokumentart, TDokumentart, DokumentartValidator>, IDokumentartService
{
    public DokumentartService(IErrorHandlerService errorHandlerService, IDbContextFactory<DoeAContext> dbFactory, DokumentartMapper mapper, ILogger logger, IAktuellerBenutzer aktuellerBenutzer)
        : base(errorHandlerService, dbFactory, mapper, logger, aktuellerBenutzer, new DokumentartValidator(), "Doa")
    {
    }

    // Die Dokumentarten sind über ihre NaturalId applikationsrelevant (Sprachmatrix, Vorbelegung, Exporte): Der Bestand ist fix,
    // sie werden weder gelöscht noch deaktiviert. Aktiv wird beim Update nie zurückgeschrieben.
    protected override IEnumerable<string> GeschuetzteProperties => [nameof(TDokumentart.DoaAktiv)];

    /// <summary>Dokumentarten lassen sich nicht löschen, der Bestand ist fix.</summary>
    public override Task<Dokumentart> Delete(Guid id) =>
        throw new InvalidOperationException(GeneralConstants.ForbiddenError);

    /// <summary>Sucht über die Bezeichnung.</summary>
    protected override Expression<Func<TDokumentart, bool>> SuchBedingung(string searchValue) =>
        d => d.DoaBezeichnung.ToLower().Contains(searchValue.ToLower());
}