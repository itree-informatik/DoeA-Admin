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

/// <summary>Stammdaten der Organisationszugehörigkeiten; Spaltenpräfix "Org".</summary>
public class OrganisationszugehoerigkeitService :
    BaseService<Organisationszugehoerigkeit, TOrganisationszugehoerigkeit, OrganisationszugehoerigkeitValidator>,
    IOrganisationszugehoerigkeitService
{
    public OrganisationszugehoerigkeitService(IErrorHandlerService errorHandlerService, IDbContextFactory<DoeAContext> dbFactory, OrganisationszugehoerigkeitMapper mapper, ILogger logger, IAktuellerBenutzer aktuellerBenutzer)
        : base(errorHandlerService, dbFactory, mapper, logger, aktuellerBenutzer, new OrganisationszugehoerigkeitValidator(), "Org")
    {
    }

    /// <summary>Aktiv ist fachlich fix — die drei Einträge sind über <see cref="Data.Enums.EnumOrganisationszugehoerigkeit"/> im Code verankert.</summary>
    protected override IEnumerable<string> GeschuetzteProperties => [nameof(TOrganisationszugehoerigkeit.OrgAktiv)];

    /// <summary>Sucht über Bezeichnung und Kurzbezeichnung.</summary>
    protected override Expression<Func<TOrganisationszugehoerigkeit, bool>> SuchBedingung(string searchValue) =>
        o => o.OrgBezeichnung.ToLower().Contains(searchValue.ToLower()) ||
             o.OrgKurzbezeichnung.ToLower() == searchValue.ToLower();
}