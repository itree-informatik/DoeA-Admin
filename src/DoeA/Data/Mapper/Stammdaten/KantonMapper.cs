// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO.Stammdaten;
using DoeA.Interfaces;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper.Stammdaten;

/// <summary>Mapping zwischen TKanton und dem DTO Kanton.</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(TextKonvertierung))]
public partial class KantonMapper : IEntityMapper<Kanton, TKanton>
{
    private readonly SpracheMapper _spracheMapper;

    public KantonMapper(SpracheMapper spracheMapper)
    {
        _spracheMapper = spracheMapper;
    }

    [MapProperty(nameof(TKanton.KanId), nameof(Kanton.Id))]
    [MapProperty(nameof(TKanton.KanSprId), nameof(Kanton.SprId))]
    [MapProperty(nameof(TKanton.KanBezeichnung), nameof(Kanton.Bezeichnung))]
    [MapProperty(nameof(TKanton.KanKurzbezeichnung), nameof(Kanton.Kurzbezeichnung))]
    [MapProperty(nameof(TKanton.KanNaturalId), nameof(Kanton.NaturalId))]
    [MapProperty(nameof(TKanton.KanAktiv), nameof(Kanton.Aktiv))]
    [MapProperty(nameof(TKanton.KanInsertedDate), nameof(Kanton.InsertedDate))]
    [MapProperty(nameof(TKanton.KanInsertedUser), nameof(Kanton.InsertedUser))]
    [MapProperty(nameof(TKanton.KanChangedDate), nameof(Kanton.ChangedDate))]
    [MapProperty(nameof(TKanton.KanChangedUser), nameof(Kanton.ChangedUser))]
    [MapProperty(nameof(TKanton.KanSpr), nameof(Kanton.Sprache), Use = nameof(ZuSprache))]
    public partial Kanton ZuModel(TKanton entity);

    [MapperRequiredMapping(RequiredMappingStrategy.Both)]
    [MapperIgnoreSource(nameof(Kanton.InsertedDateDateOnly))]
    [MapperIgnoreSource(nameof(Kanton.ChangedDateDateOnly))]
    // Navigationen bleiben aussen vor: EF verwaltet sie über die Fremdschlüssel, und der Rückweg würde referenzierte Entities beim Create erneut einfügen.
    [MapperIgnoreTarget(nameof(TKanton.KanSpr))]
    [MapperIgnoreTarget(nameof(TKanton.TExportgruppeKanton))]
    [MapperIgnoreTarget(nameof(TKanton.TOrtschaft))]
    [MapperIgnoreSource(nameof(Kanton.Sprache))]
    [MapProperty(nameof(Kanton.Id), nameof(TKanton.KanId))]
    [MapProperty(nameof(Kanton.SprId), nameof(TKanton.KanSprId))]
    [MapProperty(nameof(Kanton.Bezeichnung), nameof(TKanton.KanBezeichnung))]
    [MapProperty(nameof(Kanton.Kurzbezeichnung), nameof(TKanton.KanKurzbezeichnung))]
    [MapProperty(nameof(Kanton.NaturalId), nameof(TKanton.KanNaturalId))]
    [MapProperty(nameof(Kanton.Aktiv), nameof(TKanton.KanAktiv))]
    [MapProperty(nameof(Kanton.InsertedDate), nameof(TKanton.KanInsertedDate))]
    [MapProperty(nameof(Kanton.InsertedUser), nameof(TKanton.KanInsertedUser))]
    [MapProperty(nameof(Kanton.ChangedDate), nameof(TKanton.KanChangedDate))]
    [MapProperty(nameof(Kanton.ChangedUser), nameof(TKanton.KanChangedUser))]
    public partial TKanton ZuEntity(Kanton model);

    // EF liefert Navigationen ohne Include als null.
    private Sprache? ZuSprache(TSprache? sprache) => sprache == null ? null : _spracheMapper.ZuModel(sprache);
}
