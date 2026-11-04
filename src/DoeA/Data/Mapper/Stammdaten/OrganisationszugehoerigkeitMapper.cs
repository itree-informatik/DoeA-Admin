// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO.Stammdaten;
using DoeA.Interfaces;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper.Stammdaten;

/// <summary>Mapping zwischen TOrganisationszugehoerigkeit und dem DTO Organisationszugehoerigkeit.</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(TextKonvertierung))]
public partial class OrganisationszugehoerigkeitMapper : IEntityMapper<Organisationszugehoerigkeit, TOrganisationszugehoerigkeit>
{
    [MapProperty(nameof(TOrganisationszugehoerigkeit.OrgId), nameof(Organisationszugehoerigkeit.Id))]
    [MapProperty(nameof(TOrganisationszugehoerigkeit.OrgBezeichnung), nameof(Organisationszugehoerigkeit.Bezeichnung))]
    [MapProperty(nameof(TOrganisationszugehoerigkeit.OrgKurzbezeichnung), nameof(Organisationszugehoerigkeit.Kurzbezeichnung))]
    [MapProperty(nameof(TOrganisationszugehoerigkeit.OrgNaturalId), nameof(Organisationszugehoerigkeit.NaturalId))]
    [MapProperty(nameof(TOrganisationszugehoerigkeit.OrgAktiv), nameof(Organisationszugehoerigkeit.Aktiv))]
    [MapProperty(nameof(TOrganisationszugehoerigkeit.OrgInsertedDate), nameof(Organisationszugehoerigkeit.InsertedDate))]
    [MapProperty(nameof(TOrganisationszugehoerigkeit.OrgInsertedUser), nameof(Organisationszugehoerigkeit.InsertedUser))]
    [MapProperty(nameof(TOrganisationszugehoerigkeit.OrgChangedDate), nameof(Organisationszugehoerigkeit.ChangedDate))]
    [MapProperty(nameof(TOrganisationszugehoerigkeit.OrgChangedUser), nameof(Organisationszugehoerigkeit.ChangedUser))]
    public partial Organisationszugehoerigkeit ZuModel(TOrganisationszugehoerigkeit entity);

    [MapperRequiredMapping(RequiredMappingStrategy.Both)]
    [MapperIgnoreSource(nameof(Organisationszugehoerigkeit.InsertedDateDateOnly))]
    [MapperIgnoreSource(nameof(Organisationszugehoerigkeit.ChangedDateDateOnly))]
    // Navigationen bleiben aussen vor: EF verwaltet sie über die Fremdschlüssel, und der Rückweg würde referenzierte Entities beim Create erneut einfügen.
    [MapperIgnoreTarget(nameof(TOrganisationszugehoerigkeit.TSachbearbeiter))]
    [MapProperty(nameof(Organisationszugehoerigkeit.Id), nameof(TOrganisationszugehoerigkeit.OrgId))]
    [MapProperty(nameof(Organisationszugehoerigkeit.Bezeichnung), nameof(TOrganisationszugehoerigkeit.OrgBezeichnung))]
    [MapProperty(nameof(Organisationszugehoerigkeit.Kurzbezeichnung), nameof(TOrganisationszugehoerigkeit.OrgKurzbezeichnung))]
    [MapProperty(nameof(Organisationszugehoerigkeit.NaturalId), nameof(TOrganisationszugehoerigkeit.OrgNaturalId))]
    [MapProperty(nameof(Organisationszugehoerigkeit.Aktiv), nameof(TOrganisationszugehoerigkeit.OrgAktiv))]
    [MapProperty(nameof(Organisationszugehoerigkeit.InsertedDate), nameof(TOrganisationszugehoerigkeit.OrgInsertedDate))]
    [MapProperty(nameof(Organisationszugehoerigkeit.InsertedUser), nameof(TOrganisationszugehoerigkeit.OrgInsertedUser))]
    [MapProperty(nameof(Organisationszugehoerigkeit.ChangedDate), nameof(TOrganisationszugehoerigkeit.OrgChangedDate))]
    [MapProperty(nameof(Organisationszugehoerigkeit.ChangedUser), nameof(TOrganisationszugehoerigkeit.OrgChangedUser))]
    public partial TOrganisationszugehoerigkeit ZuEntity(Organisationszugehoerigkeit model);
}
