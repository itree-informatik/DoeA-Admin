// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO.Stammdaten;
using DoeA.Interfaces;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper.Stammdaten;

/// <summary>Mapping zwischen TSprache und dem DTO Sprache.</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(TextKonvertierung))]
public partial class SpracheMapper : IEntityMapper<Sprache, TSprache>
{
    [MapProperty(nameof(TSprache.SprId), nameof(Sprache.Id))]
    [MapProperty(nameof(TSprache.SprCulture), nameof(Sprache.Culture))]
    [MapProperty(nameof(TSprache.SprBezeichnung), nameof(Sprache.Bezeichnung))]
    [MapProperty(nameof(TSprache.SprNaturalId), nameof(Sprache.NaturalId))]
    [MapProperty(nameof(TSprache.SprSortierung), nameof(Sprache.Sortierung))]
    [MapProperty(nameof(TSprache.SprAktiv), nameof(Sprache.Aktiv))]
    [MapProperty(nameof(TSprache.SprInsertedDate), nameof(Sprache.InsertedDate))]
    [MapProperty(nameof(TSprache.SprInsertedUser), nameof(Sprache.InsertedUser))]
    [MapProperty(nameof(TSprache.SprChangedDate), nameof(Sprache.ChangedDate))]
    [MapProperty(nameof(TSprache.SprChangedUser), nameof(Sprache.ChangedUser))]
    public partial Sprache ZuModel(TSprache entity);

    [MapperRequiredMapping(RequiredMappingStrategy.Both)]
    [MapperIgnoreSource(nameof(Sprache.InsertedDateDateOnly))]
    [MapperIgnoreSource(nameof(Sprache.ChangedDateDateOnly))]
    // Navigationen bleiben aussen vor: EF verwaltet sie über die Fremdschlüssel, und der Rückweg würde referenzierte Entities beim Create erneut einfügen.
    [MapperIgnoreTarget(nameof(TSprache.TAusschreibungSprache))]
    [MapperIgnoreTarget(nameof(TSprache.TKanton))]
    [MapProperty(nameof(Sprache.Id), nameof(TSprache.SprId))]
    [MapProperty(nameof(Sprache.Culture), nameof(TSprache.SprCulture))]
    [MapProperty(nameof(Sprache.Bezeichnung), nameof(TSprache.SprBezeichnung))]
    [MapProperty(nameof(Sprache.NaturalId), nameof(TSprache.SprNaturalId))]
    [MapProperty(nameof(Sprache.Sortierung), nameof(TSprache.SprSortierung))]
    [MapProperty(nameof(Sprache.Aktiv), nameof(TSprache.SprAktiv))]
    [MapProperty(nameof(Sprache.InsertedDate), nameof(TSprache.SprInsertedDate))]
    [MapProperty(nameof(Sprache.InsertedUser), nameof(TSprache.SprInsertedUser))]
    [MapProperty(nameof(Sprache.ChangedDate), nameof(TSprache.SprChangedDate))]
    [MapProperty(nameof(Sprache.ChangedUser), nameof(TSprache.SprChangedUser))]
    public partial TSprache ZuEntity(Sprache model);
}
