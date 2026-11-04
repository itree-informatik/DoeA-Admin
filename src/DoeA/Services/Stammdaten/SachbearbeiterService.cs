// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Enums;
using DoeA.Data.Mapper.Stammdaten;
using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO.Stammdaten;
using DoeA.Interfaces;
using DoeA.Interfaces.Stammdaten;
using DoeA.Validator.Stammdaten;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using ILogger = Serilog.ILogger;

namespace DoeA.Services.Stammdaten;

/// <summary>Stammdaten der Sachbearbeiter; Spaltenpräfix "Sac".</summary>
public class SachbearbeiterService :
    BaseService<Sachbearbeiter, TSachbearbeiter, SachbearbeiterValidator>,
    ISachbearbeiterService
{
    public SachbearbeiterService(IErrorHandlerService errorHandlerService, IDbContextFactory<DoeAContext> dbFactory, SachbearbeiterMapper mapper, ILogger logger, IAktuellerBenutzer aktuellerBenutzer)
        : base(errorHandlerService, dbFactory, mapper, logger, aktuellerBenutzer, new SachbearbeiterValidator(), "Sac")
    {
    }

    protected override IQueryable<TSachbearbeiter> MitNavigationen(IQueryable<TSachbearbeiter> queryable) =>
        queryable.Include(s => s.SacOrg);

    /// <summary>Sucht über Nachname, Vorname, Kurzbezeichnung und die Organisationszugehörigkeit.</summary>
    protected override Expression<Func<TSachbearbeiter, bool>> SuchBedingung(string searchValue) =>
        s => s.SacNachname.ToLower().Contains(searchValue.ToLower()) ||
             s.SacVorname.ToLower().Contains(searchValue.ToLower()) ||
             s.SacKurzbezeichnung.ToLower() == searchValue.ToLower() ||
             s.SacOrg.OrgKurzbezeichnung.ToLower() == searchValue.ToLower() ||
             s.SacOrg.OrgBezeichnung.ToLower().Contains(searchValue.ToLower());

    /// <summary>Liefert alle aktiven Sachbearbeiter als schlanke Projektion samt Organisationszugehörigkeit.</summary>
    public async Task<ObservableCollection<Sachbearbeiter>> GetAllWithOrganisationszugehoerigkeit()
    {
        await using var dbContext = await DbFactory.CreateDbContextAsync();
        var sachbearbeiter = await dbContext.TSachbearbeiter
            .AsNoTracking()
            .Where(s => s.SacAktiv)
            .Select(s => new Sachbearbeiter
            {
                Id = s.SacId,
                Nachname = s.SacNachname,
                Vorname = s.SacVorname,
                Kurzbezeichnung = s.SacKurzbezeichnung,
                OrgId = s.SacOrgId,
                Organisationszugehoerigkeit = new Organisationszugehoerigkeit
                {
                    Id = s.SacOrg.OrgId,
                    NaturalId = s.SacOrg.OrgNaturalId ?? string.Empty
                }
            })
            .ToListAsync();

        return new ObservableCollection<Sachbearbeiter>(sachbearbeiter);
    }

    /// <summary>
    /// Auswahl eines Exportgruppen-Filters: aktive Sachbearbeiter der Organisation plus alle, auch inaktive,
    /// die je einer Ausschreibung in der Rolle dieser Organisation zugewiesen waren.
    /// </summary>
    public async Task<List<Sachbearbeiter>> GetFilterAuswahl(EnumOrganisationszugehoerigkeit organisation)
    {
        await using var dbContext = await DbFactory.CreateDbContextAsync();
        var verwendet = organisation switch
        {
            EnumOrganisationszugehoerigkeit.DoeA => dbContext.TAusschreibung.Select(a => a.AusSacDoeAId),
            EnumOrganisationszugehoerigkeit.RD => dbContext.TAusschreibung.Select(a => a.AusSacRdId),
            _ => dbContext.TAusschreibung.Select(a => a.AusSacBblId)
        };
        var organisationNaturalId = organisation.ToString();

        var sachbearbeiter = await MitNavigationen(dbContext.TSachbearbeiter.AsNoTracking())
            .Where(s => (s.SacAktiv && s.SacOrg.OrgNaturalId == organisationNaturalId) || verwendet.Contains(s.SacId))
            .ToListAsync();

        return sachbearbeiter.Select(Mapper.ZuModel).ToList();
    }
}