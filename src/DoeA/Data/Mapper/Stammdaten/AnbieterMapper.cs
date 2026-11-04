// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO.Stammdaten;
using DoeA.Interfaces;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper.Stammdaten;

/// <summary>Mapping zwischen TAnbieter und dem DTO Anbieter.</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(TextKonvertierung))]
public partial class AnbieterMapper : IEntityMapper<Anbieter, TAnbieter>
{
    private readonly OrtschaftMapper _ortschaftMapper;

    public AnbieterMapper(OrtschaftMapper ortschaftMapper)
    {
        _ortschaftMapper = ortschaftMapper;
    }

    [MapProperty(nameof(TAnbieter.AnbId), nameof(Anbieter.Id))]
    [MapProperty(nameof(TAnbieter.AnbOrtId), nameof(Anbieter.OrtId))]
    [MapProperty(nameof(TAnbieter.AnbName), nameof(Anbieter.Name))]
    [MapProperty(nameof(TAnbieter.AnbAdresse), nameof(Anbieter.Adresse))]
    [MapProperty(nameof(TAnbieter.AnbAdresse2), nameof(Anbieter.Adresse2))]
    [MapProperty(nameof(TAnbieter.AnbAdresse3), nameof(Anbieter.Adresse3))]
    [MapProperty(nameof(TAnbieter.AnbNaturalId), nameof(Anbieter.NaturalId))]
    [MapProperty(nameof(TAnbieter.AnbAktiv), nameof(Anbieter.Aktiv))]
    [MapProperty(nameof(TAnbieter.AnbInsertedDate), nameof(Anbieter.InsertedDate))]
    [MapProperty(nameof(TAnbieter.AnbInsertedUser), nameof(Anbieter.InsertedUser))]
    [MapProperty(nameof(TAnbieter.AnbChangedDate), nameof(Anbieter.ChangedDate))]
    [MapProperty(nameof(TAnbieter.AnbChangedUser), nameof(Anbieter.ChangedUser))]
    [MapProperty(nameof(TAnbieter.AnbOrt), nameof(Anbieter.Ortschaft), Use = nameof(ZuOrtschaft))]
    public partial Anbieter ZuModel(TAnbieter entity);

    [MapperRequiredMapping(RequiredMappingStrategy.Both)]
    [MapperIgnoreSource(nameof(Anbieter.InsertedDateDateOnly))]
    [MapperIgnoreSource(nameof(Anbieter.ChangedDateDateOnly))]
    // Navigationen bleiben aussen vor: EF verwaltet sie über die Fremdschlüssel, und der Rückweg würde referenzierte Entities beim Create erneut einfügen.
    [MapperIgnoreTarget(nameof(TAnbieter.AnbOrt))]
    [MapperIgnoreTarget(nameof(TAnbieter.TOfferte))]
    [MapperIgnoreSource(nameof(Anbieter.Ortschaft))]
    [MapProperty(nameof(Anbieter.Id), nameof(TAnbieter.AnbId))]
    [MapProperty(nameof(Anbieter.OrtId), nameof(TAnbieter.AnbOrtId))]
    [MapProperty(nameof(Anbieter.Name), nameof(TAnbieter.AnbName))]
    [MapProperty(nameof(Anbieter.Adresse), nameof(TAnbieter.AnbAdresse))]
    [MapProperty(nameof(Anbieter.Adresse2), nameof(TAnbieter.AnbAdresse2))]
    [MapProperty(nameof(Anbieter.Adresse3), nameof(TAnbieter.AnbAdresse3))]
    [MapProperty(nameof(Anbieter.NaturalId), nameof(TAnbieter.AnbNaturalId))]
    [MapProperty(nameof(Anbieter.Aktiv), nameof(TAnbieter.AnbAktiv))]
    [MapProperty(nameof(Anbieter.InsertedDate), nameof(TAnbieter.AnbInsertedDate))]
    [MapProperty(nameof(Anbieter.InsertedUser), nameof(TAnbieter.AnbInsertedUser))]
    [MapProperty(nameof(Anbieter.ChangedDate), nameof(TAnbieter.AnbChangedDate))]
    [MapProperty(nameof(Anbieter.ChangedUser), nameof(TAnbieter.AnbChangedUser))]
    public partial TAnbieter ZuEntity(Anbieter model);

    // EF liefert Navigationen ohne Include als null, obwohl sie non-nullable deklariert sind.
    private Ortschaft? ZuOrtschaft(TOrtschaft? ortschaft) => ortschaft == null ? null : _ortschaftMapper.ZuModel(ortschaft);
}
