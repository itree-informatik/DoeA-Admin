// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO.Stammdaten;
using DoeA.Interfaces;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper.Stammdaten;

/// <summary>Mapping zwischen TCpvNummer und dem DTO CpvNummer.</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(TextKonvertierung))]
public partial class CpvNummerMapper : IEntityMapper<CpvNummer, TCpvNummer>
{
    [MapProperty(nameof(TCpvNummer.CpvId), nameof(CpvNummer.Id))]
    [MapProperty(nameof(TCpvNummer.CpvNummerStr), nameof(CpvNummer.NummerStr))]
    [MapProperty(nameof(TCpvNummer.CpvBezeichnung), nameof(CpvNummer.Bezeichnung))]
    [MapProperty(nameof(TCpvNummer.CpvNummer), nameof(CpvNummer.Nummer))]
    [MapProperty(nameof(TCpvNummer.CpvNaturalId), nameof(CpvNummer.NaturalId))]
    [MapProperty(nameof(TCpvNummer.CpvAktiv), nameof(CpvNummer.Aktiv))]
    [MapProperty(nameof(TCpvNummer.CpvInsertedDate), nameof(CpvNummer.InsertedDate))]
    [MapProperty(nameof(TCpvNummer.CpvInsertedUser), nameof(CpvNummer.InsertedUser))]
    [MapProperty(nameof(TCpvNummer.CpvChangedDate), nameof(CpvNummer.ChangedDate))]
    [MapProperty(nameof(TCpvNummer.CpvChangedUser), nameof(CpvNummer.ChangedUser))]
    public partial CpvNummer ZuModel(TCpvNummer entity);

    [MapperRequiredMapping(RequiredMappingStrategy.Both)]
    [MapperIgnoreSource(nameof(CpvNummer.InsertedDateDateOnly))]
    [MapperIgnoreSource(nameof(CpvNummer.ChangedDateDateOnly))]
    // Navigationen bleiben aussen vor: EF verwaltet sie über die Fremdschlüssel, und der Rückweg würde referenzierte Entities beim Create erneut einfügen.
    [MapperIgnoreTarget(nameof(TCpvNummer.TAusschreibungCpvNummer))]
    [MapProperty(nameof(CpvNummer.Id), nameof(TCpvNummer.CpvId))]
    [MapProperty(nameof(CpvNummer.NummerStr), nameof(TCpvNummer.CpvNummerStr))]
    [MapProperty(nameof(CpvNummer.Bezeichnung), nameof(TCpvNummer.CpvBezeichnung))]
    [MapProperty(nameof(CpvNummer.Nummer), nameof(TCpvNummer.CpvNummer))]
    [MapProperty(nameof(CpvNummer.NaturalId), nameof(TCpvNummer.CpvNaturalId))]
    [MapProperty(nameof(CpvNummer.Aktiv), nameof(TCpvNummer.CpvAktiv))]
    [MapProperty(nameof(CpvNummer.InsertedDate), nameof(TCpvNummer.CpvInsertedDate))]
    [MapProperty(nameof(CpvNummer.InsertedUser), nameof(TCpvNummer.CpvInsertedUser))]
    [MapProperty(nameof(CpvNummer.ChangedDate), nameof(TCpvNummer.CpvChangedDate))]
    [MapProperty(nameof(CpvNummer.ChangedUser), nameof(TCpvNummer.CpvChangedUser))]
    public partial TCpvNummer ZuEntity(CpvNummer model);
}
