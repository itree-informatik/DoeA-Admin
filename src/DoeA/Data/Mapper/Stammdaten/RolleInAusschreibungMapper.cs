// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO.Stammdaten;
using DoeA.Interfaces;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper.Stammdaten;

/// <summary>Mapping zwischen TRolleInAusschreibung und dem DTO RolleInAusschreibung.</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(TextKonvertierung))]
public partial class RolleInAusschreibungMapper : IEntityMapper<RolleInAusschreibung, TRolleInAusschreibung>
{
    [MapProperty(nameof(TRolleInAusschreibung.RiaId), nameof(RolleInAusschreibung.Id))]
    [MapProperty(nameof(TRolleInAusschreibung.RiaBezeichnung), nameof(RolleInAusschreibung.Bezeichnung))]
    [MapProperty(nameof(TRolleInAusschreibung.RiaKurzbezeichnung), nameof(RolleInAusschreibung.Kurzbezeichnung))]
    [MapProperty(nameof(TRolleInAusschreibung.RiaNaturalId), nameof(RolleInAusschreibung.NaturalId))]
    [MapProperty(nameof(TRolleInAusschreibung.RiaAktiv), nameof(RolleInAusschreibung.Aktiv))]
    [MapProperty(nameof(TRolleInAusschreibung.RiaInsertedDate), nameof(RolleInAusschreibung.InsertedDate))]
    [MapProperty(nameof(TRolleInAusschreibung.RiaInsertedUser), nameof(RolleInAusschreibung.InsertedUser))]
    [MapProperty(nameof(TRolleInAusschreibung.RiaChangedDate), nameof(RolleInAusschreibung.ChangedDate))]
    [MapProperty(nameof(TRolleInAusschreibung.RiaChangedUser), nameof(RolleInAusschreibung.ChangedUser))]
    public partial RolleInAusschreibung ZuModel(TRolleInAusschreibung entity);

    [MapperRequiredMapping(RequiredMappingStrategy.Both)]
    [MapperIgnoreSource(nameof(RolleInAusschreibung.InsertedDateDateOnly))]
    [MapperIgnoreSource(nameof(RolleInAusschreibung.ChangedDateDateOnly))]
    // Navigationen bleiben aussen vor: EF verwaltet sie über die Fremdschlüssel, und der Rückweg würde referenzierte Entities beim Create erneut einfügen.
    [MapperIgnoreTarget(nameof(TRolleInAusschreibung.TAusschreibung))]
    [MapperIgnoreTarget(nameof(TRolleInAusschreibung.TExportgruppeRolleInAusschreibung))]
    [MapProperty(nameof(RolleInAusschreibung.Id), nameof(TRolleInAusschreibung.RiaId))]
    [MapProperty(nameof(RolleInAusschreibung.Bezeichnung), nameof(TRolleInAusschreibung.RiaBezeichnung))]
    [MapProperty(nameof(RolleInAusschreibung.Kurzbezeichnung), nameof(TRolleInAusschreibung.RiaKurzbezeichnung))]
    [MapProperty(nameof(RolleInAusschreibung.NaturalId), nameof(TRolleInAusschreibung.RiaNaturalId))]
    [MapProperty(nameof(RolleInAusschreibung.Aktiv), nameof(TRolleInAusschreibung.RiaAktiv))]
    [MapProperty(nameof(RolleInAusschreibung.InsertedDate), nameof(TRolleInAusschreibung.RiaInsertedDate))]
    [MapProperty(nameof(RolleInAusschreibung.InsertedUser), nameof(TRolleInAusschreibung.RiaInsertedUser))]
    [MapProperty(nameof(RolleInAusschreibung.ChangedDate), nameof(TRolleInAusschreibung.RiaChangedDate))]
    [MapProperty(nameof(RolleInAusschreibung.ChangedUser), nameof(TRolleInAusschreibung.RiaChangedUser))]
    public partial TRolleInAusschreibung ZuEntity(RolleInAusschreibung model);
}
