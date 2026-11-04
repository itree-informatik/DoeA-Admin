// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO.Stammdaten;
using DoeA.Interfaces;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper.Stammdaten;

/// <summary>Mapping zwischen TOrtschaft und dem DTO Ortschaft.</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(TextKonvertierung))]
public partial class OrtschaftMapper : IEntityMapper<Ortschaft, TOrtschaft>
{
    private readonly KantonMapper _kantonMapper;
    private readonly LandMapper _landMapper;

    public OrtschaftMapper(KantonMapper kantonMapper, LandMapper landMapper)
    {
        _kantonMapper = kantonMapper;
        _landMapper = landMapper;
    }

    [MapProperty(nameof(TOrtschaft.OrtId), nameof(Ortschaft.Id))]
    [MapProperty(nameof(TOrtschaft.OrtLanId), nameof(Ortschaft.LanId))]
    [MapProperty(nameof(TOrtschaft.OrtKanId), nameof(Ortschaft.KanId))]
    [MapProperty(nameof(TOrtschaft.OrtOrt), nameof(Ortschaft.Ort))]
    [MapProperty(nameof(TOrtschaft.OrtPlz), nameof(Ortschaft.Plz))]
    [MapProperty(nameof(TOrtschaft.OrtNaturalId), nameof(Ortschaft.NaturalId))]
    [MapProperty(nameof(TOrtschaft.OrtAktiv), nameof(Ortschaft.Aktiv))]
    [MapProperty(nameof(TOrtschaft.OrtInsertedDate), nameof(Ortschaft.InsertedDate))]
    [MapProperty(nameof(TOrtschaft.OrtInsertedUser), nameof(Ortschaft.InsertedUser))]
    [MapProperty(nameof(TOrtschaft.OrtChangedDate), nameof(Ortschaft.ChangedDate))]
    [MapProperty(nameof(TOrtschaft.OrtChangedUser), nameof(Ortschaft.ChangedUser))]
    [MapProperty(nameof(TOrtschaft.OrtKan), nameof(Ortschaft.Kanton), Use = nameof(ZuKanton))]
    [MapProperty(nameof(TOrtschaft.OrtLan), nameof(Ortschaft.Land), Use = nameof(ZuLand))]
    public partial Ortschaft ZuModel(TOrtschaft entity);

    [MapperRequiredMapping(RequiredMappingStrategy.Both)]
    [MapperIgnoreSource(nameof(Ortschaft.InsertedDateDateOnly))]
    [MapperIgnoreSource(nameof(Ortschaft.ChangedDateDateOnly))]
    // Navigationen bleiben aussen vor: EF verwaltet sie über die Fremdschlüssel, und der Rückweg würde referenzierte Entities beim Create erneut einfügen.
    [MapperIgnoreTarget(nameof(TOrtschaft.OrtKan))]
    [MapperIgnoreTarget(nameof(TOrtschaft.OrtLan))]
    [MapperIgnoreTarget(nameof(TOrtschaft.TAnbieter))]
    [MapperIgnoreSource(nameof(Ortschaft.Kanton))]
    [MapperIgnoreSource(nameof(Ortschaft.Land))]
    [MapProperty(nameof(Ortschaft.Id), nameof(TOrtschaft.OrtId))]
    [MapProperty(nameof(Ortschaft.LanId), nameof(TOrtschaft.OrtLanId))]
    [MapProperty(nameof(Ortschaft.KanId), nameof(TOrtschaft.OrtKanId))]
    [MapProperty(nameof(Ortschaft.Ort), nameof(TOrtschaft.OrtOrt))]
    [MapProperty(nameof(Ortschaft.Plz), nameof(TOrtschaft.OrtPlz))]
    [MapProperty(nameof(Ortschaft.NaturalId), nameof(TOrtschaft.OrtNaturalId))]
    [MapProperty(nameof(Ortschaft.Aktiv), nameof(TOrtschaft.OrtAktiv))]
    [MapProperty(nameof(Ortschaft.InsertedDate), nameof(TOrtschaft.OrtInsertedDate))]
    [MapProperty(nameof(Ortschaft.InsertedUser), nameof(TOrtschaft.OrtInsertedUser))]
    [MapProperty(nameof(Ortschaft.ChangedDate), nameof(TOrtschaft.OrtChangedDate))]
    [MapProperty(nameof(Ortschaft.ChangedUser), nameof(TOrtschaft.OrtChangedUser))]
    public partial TOrtschaft ZuEntity(Ortschaft model);

    // EF liefert Navigationen ohne Include als null, obwohl sie non-nullable deklariert sind.
    private Kanton? ZuKanton(TKanton? kanton) => kanton == null ? null : _kantonMapper.ZuModel(kanton);

    private Land? ZuLand(TLand? land) => land == null ? null : _landMapper.ZuModel(land);
}
