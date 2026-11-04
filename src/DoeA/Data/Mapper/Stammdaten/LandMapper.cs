// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO.Stammdaten;
using DoeA.Interfaces;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper.Stammdaten;

/// <summary>Mapping zwischen TLand und dem DTO Land.</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(TextKonvertierung))]
public partial class LandMapper : IEntityMapper<Land, TLand>
{
    [MapProperty(nameof(TLand.LanId), nameof(Land.Id))]
    [MapProperty(nameof(TLand.LanBezeichnung), nameof(Land.Bezeichnung))]
    [MapProperty(nameof(TLand.LanKurzbezeichnung), nameof(Land.Kurzbezeichnung))]
    [MapProperty(nameof(TLand.LanNaturalId), nameof(Land.NaturalId))]
    [MapProperty(nameof(TLand.LanAktiv), nameof(Land.Aktiv))]
    [MapProperty(nameof(TLand.LanInsertedDate), nameof(Land.InsertedDate))]
    [MapProperty(nameof(TLand.LanInsertedUser), nameof(Land.InsertedUser))]
    [MapProperty(nameof(TLand.LanChangedDate), nameof(Land.ChangedDate))]
    [MapProperty(nameof(TLand.LanChangedUser), nameof(Land.ChangedUser))]
    public partial Land ZuModel(TLand entity);

    [MapperRequiredMapping(RequiredMappingStrategy.Both)]
    [MapperIgnoreSource(nameof(Land.InsertedDateDateOnly))]
    [MapperIgnoreSource(nameof(Land.ChangedDateDateOnly))]
    // Navigationen bleiben aussen vor: EF verwaltet sie über die Fremdschlüssel, und der Rückweg würde referenzierte Entities beim Create erneut einfügen.
    [MapperIgnoreTarget(nameof(TLand.TOrtschaft))]
    [MapProperty(nameof(Land.Id), nameof(TLand.LanId))]
    [MapProperty(nameof(Land.Bezeichnung), nameof(TLand.LanBezeichnung))]
    [MapProperty(nameof(Land.Kurzbezeichnung), nameof(TLand.LanKurzbezeichnung))]
    [MapProperty(nameof(Land.NaturalId), nameof(TLand.LanNaturalId))]
    [MapProperty(nameof(Land.Aktiv), nameof(TLand.LanAktiv))]
    [MapProperty(nameof(Land.InsertedDate), nameof(TLand.LanInsertedDate))]
    [MapProperty(nameof(Land.InsertedUser), nameof(TLand.LanInsertedUser))]
    [MapProperty(nameof(Land.ChangedDate), nameof(TLand.LanChangedDate))]
    [MapProperty(nameof(Land.ChangedUser), nameof(TLand.LanChangedUser))]
    public partial TLand ZuEntity(Land model);
}
