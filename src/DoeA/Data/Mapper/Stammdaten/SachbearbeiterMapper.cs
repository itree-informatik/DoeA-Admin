// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO.Stammdaten;
using DoeA.Interfaces;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper.Stammdaten;

/// <summary>Mapping zwischen TSachbearbeiter und dem DTO Sachbearbeiter.</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(TextKonvertierung))]
public partial class SachbearbeiterMapper : IEntityMapper<Sachbearbeiter, TSachbearbeiter>
{
    private readonly OrganisationszugehoerigkeitMapper _organisationszugehoerigkeitMapper;

    public SachbearbeiterMapper(OrganisationszugehoerigkeitMapper organisationszugehoerigkeitMapper)
    {
        _organisationszugehoerigkeitMapper = organisationszugehoerigkeitMapper;
    }

    [MapProperty(nameof(TSachbearbeiter.SacId), nameof(Sachbearbeiter.Id))]
    [MapProperty(nameof(TSachbearbeiter.SacOrgId), nameof(Sachbearbeiter.OrgId))]
    [MapProperty(nameof(TSachbearbeiter.SacNachname), nameof(Sachbearbeiter.Nachname))]
    [MapProperty(nameof(TSachbearbeiter.SacVorname), nameof(Sachbearbeiter.Vorname))]
    [MapProperty(nameof(TSachbearbeiter.SacKurzbezeichnung), nameof(Sachbearbeiter.Kurzbezeichnung))]
    [MapProperty(nameof(TSachbearbeiter.SacNaturalId), nameof(Sachbearbeiter.NaturalId))]
    [MapProperty(nameof(TSachbearbeiter.SacAktiv), nameof(Sachbearbeiter.Aktiv))]
    [MapProperty(nameof(TSachbearbeiter.SacInsertedDate), nameof(Sachbearbeiter.InsertedDate))]
    [MapProperty(nameof(TSachbearbeiter.SacInsertedUser), nameof(Sachbearbeiter.InsertedUser))]
    [MapProperty(nameof(TSachbearbeiter.SacChangedDate), nameof(Sachbearbeiter.ChangedDate))]
    [MapProperty(nameof(TSachbearbeiter.SacChangedUser), nameof(Sachbearbeiter.ChangedUser))]
    [MapProperty(nameof(TSachbearbeiter.SacOrg), nameof(Sachbearbeiter.Organisationszugehoerigkeit), Use = nameof(ZuOrganisationszugehoerigkeit))]
    public partial Sachbearbeiter ZuModel(TSachbearbeiter entity);

    [MapperRequiredMapping(RequiredMappingStrategy.Both)]
    [MapperIgnoreSource(nameof(Sachbearbeiter.InsertedDateDateOnly))]
    [MapperIgnoreSource(nameof(Sachbearbeiter.ChangedDateDateOnly))]
    // Navigationen bleiben aussen vor: EF verwaltet sie über die Fremdschlüssel, und der Rückweg würde referenzierte Entities beim Create erneut einfügen.
    [MapperIgnoreTarget(nameof(TSachbearbeiter.SacOrg))]
    [MapperIgnoreTarget(nameof(TSachbearbeiter.TAusschreibungAusSacBbl))]
    [MapperIgnoreTarget(nameof(TSachbearbeiter.TAusschreibungAusSacDoeA))]
    [MapperIgnoreTarget(nameof(TSachbearbeiter.TAusschreibungAusSacRd))]
    [MapperIgnoreTarget(nameof(TSachbearbeiter.TExportgruppeSachbearbeiter))]
    [MapperIgnoreSource(nameof(Sachbearbeiter.Fullname))]
    [MapperIgnoreSource(nameof(Sachbearbeiter.Organisationszugehoerigkeit))]
    [MapProperty(nameof(Sachbearbeiter.Id), nameof(TSachbearbeiter.SacId))]
    [MapProperty(nameof(Sachbearbeiter.OrgId), nameof(TSachbearbeiter.SacOrgId))]
    [MapProperty(nameof(Sachbearbeiter.Nachname), nameof(TSachbearbeiter.SacNachname))]
    [MapProperty(nameof(Sachbearbeiter.Vorname), nameof(TSachbearbeiter.SacVorname))]
    [MapProperty(nameof(Sachbearbeiter.Kurzbezeichnung), nameof(TSachbearbeiter.SacKurzbezeichnung))]
    [MapProperty(nameof(Sachbearbeiter.NaturalId), nameof(TSachbearbeiter.SacNaturalId))]
    [MapProperty(nameof(Sachbearbeiter.Aktiv), nameof(TSachbearbeiter.SacAktiv))]
    [MapProperty(nameof(Sachbearbeiter.InsertedDate), nameof(TSachbearbeiter.SacInsertedDate))]
    [MapProperty(nameof(Sachbearbeiter.InsertedUser), nameof(TSachbearbeiter.SacInsertedUser))]
    [MapProperty(nameof(Sachbearbeiter.ChangedDate), nameof(TSachbearbeiter.SacChangedDate))]
    [MapProperty(nameof(Sachbearbeiter.ChangedUser), nameof(TSachbearbeiter.SacChangedUser))]
    public partial TSachbearbeiter ZuEntity(Sachbearbeiter model);

    // EF liefert Navigationen ohne Include als null, obwohl sie non-nullable deklariert sind.
    private Organisationszugehoerigkeit? ZuOrganisationszugehoerigkeit(TOrganisationszugehoerigkeit? organisationszugehoerigkeit) => organisationszugehoerigkeit == null ? null : _organisationszugehoerigkeitMapper.ZuModel(organisationszugehoerigkeit);
}
