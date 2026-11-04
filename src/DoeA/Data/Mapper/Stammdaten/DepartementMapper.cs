// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO.Stammdaten;
using DoeA.Interfaces;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper.Stammdaten;

/// <summary>Mapping zwischen TDepartement und dem DTO Departement.</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(TextKonvertierung))]
public partial class DepartementMapper : IEntityMapper<Departement, TDepartement>
{
    [MapProperty(nameof(TDepartement.DepId), nameof(Departement.Id))]
    [MapProperty(nameof(TDepartement.DepBezeichnung), nameof(Departement.Bezeichnung))]
    [MapProperty(nameof(TDepartement.DepKurzbezeichnung), nameof(Departement.Kurzbezeichnung))]
    [MapProperty(nameof(TDepartement.DepNaturalId), nameof(Departement.NaturalId))]
    [MapProperty(nameof(TDepartement.DepAktiv), nameof(Departement.Aktiv))]
    [MapProperty(nameof(TDepartement.DepInsertedDate), nameof(Departement.InsertedDate))]
    [MapProperty(nameof(TDepartement.DepInsertedUser), nameof(Departement.InsertedUser))]
    [MapProperty(nameof(TDepartement.DepChangedDate), nameof(Departement.ChangedDate))]
    [MapProperty(nameof(TDepartement.DepChangedUser), nameof(Departement.ChangedUser))]
    public partial Departement ZuModel(TDepartement entity);

    [MapperRequiredMapping(RequiredMappingStrategy.Both)]
    [MapperIgnoreSource(nameof(Departement.InsertedDateDateOnly))]
    [MapperIgnoreSource(nameof(Departement.ChangedDateDateOnly))]
    // Navigationen bleiben aussen vor: EF verwaltet sie über die Fremdschlüssel, und der Rückweg würde referenzierte Entities beim Create erneut einfügen.
    [MapperIgnoreTarget(nameof(TDepartement.TAmt))]
    [MapProperty(nameof(Departement.Id), nameof(TDepartement.DepId))]
    [MapProperty(nameof(Departement.Bezeichnung), nameof(TDepartement.DepBezeichnung))]
    [MapProperty(nameof(Departement.Kurzbezeichnung), nameof(TDepartement.DepKurzbezeichnung))]
    [MapProperty(nameof(Departement.NaturalId), nameof(TDepartement.DepNaturalId))]
    [MapProperty(nameof(Departement.Aktiv), nameof(TDepartement.DepAktiv))]
    [MapProperty(nameof(Departement.InsertedDate), nameof(TDepartement.DepInsertedDate))]
    [MapProperty(nameof(Departement.InsertedUser), nameof(TDepartement.DepInsertedUser))]
    [MapProperty(nameof(Departement.ChangedDate), nameof(TDepartement.DepChangedDate))]
    [MapProperty(nameof(Departement.ChangedUser), nameof(TDepartement.DepChangedUser))]
    public partial TDepartement ZuEntity(Departement model);
}
