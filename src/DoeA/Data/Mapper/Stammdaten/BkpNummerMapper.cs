// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO.Stammdaten;
using DoeA.Interfaces;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper.Stammdaten;

/// <summary>Mapping zwischen TBkpNummer und dem DTO BkpNummer.</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(TextKonvertierung))]
public partial class BkpNummerMapper : IEntityMapper<BkpNummer, TBkpNummer>
{
    [MapProperty(nameof(TBkpNummer.BkpId), nameof(BkpNummer.Id))]
    [MapProperty(nameof(TBkpNummer.BkpNummerStr), nameof(BkpNummer.NummerStr))]
    [MapProperty(nameof(TBkpNummer.BkpBezeichnung), nameof(BkpNummer.Bezeichnung))]
    [MapProperty(nameof(TBkpNummer.BkpNummer), nameof(BkpNummer.Nummer))]
    [MapProperty(nameof(TBkpNummer.BkpNaturalId), nameof(BkpNummer.NaturalId))]
    [MapProperty(nameof(TBkpNummer.BkpAktiv), nameof(BkpNummer.Aktiv))]
    [MapProperty(nameof(TBkpNummer.BkpInsertedDate), nameof(BkpNummer.InsertedDate))]
    [MapProperty(nameof(TBkpNummer.BkpInsertedUser), nameof(BkpNummer.InsertedUser))]
    [MapProperty(nameof(TBkpNummer.BkpChangedDate), nameof(BkpNummer.ChangedDate))]
    [MapProperty(nameof(TBkpNummer.BkpChangedUser), nameof(BkpNummer.ChangedUser))]
    public partial BkpNummer ZuModel(TBkpNummer entity);

    [MapperRequiredMapping(RequiredMappingStrategy.Both)]
    [MapperIgnoreSource(nameof(BkpNummer.InsertedDateDateOnly))]
    [MapperIgnoreSource(nameof(BkpNummer.ChangedDateDateOnly))]
    // Navigationen bleiben aussen vor: EF verwaltet sie über die Fremdschlüssel, und der Rückweg würde referenzierte Entities beim Create erneut einfügen.
    [MapperIgnoreTarget(nameof(TBkpNummer.TLosBkpNummer))]
    [MapProperty(nameof(BkpNummer.Id), nameof(TBkpNummer.BkpId))]
    [MapProperty(nameof(BkpNummer.NummerStr), nameof(TBkpNummer.BkpNummerStr))]
    [MapProperty(nameof(BkpNummer.Bezeichnung), nameof(TBkpNummer.BkpBezeichnung))]
    [MapProperty(nameof(BkpNummer.Nummer), nameof(TBkpNummer.BkpNummer))]
    [MapProperty(nameof(BkpNummer.NaturalId), nameof(TBkpNummer.BkpNaturalId))]
    [MapProperty(nameof(BkpNummer.Aktiv), nameof(TBkpNummer.BkpAktiv))]
    [MapProperty(nameof(BkpNummer.InsertedDate), nameof(TBkpNummer.BkpInsertedDate))]
    [MapProperty(nameof(BkpNummer.InsertedUser), nameof(TBkpNummer.BkpInsertedUser))]
    [MapProperty(nameof(BkpNummer.ChangedDate), nameof(TBkpNummer.BkpChangedDate))]
    [MapProperty(nameof(BkpNummer.ChangedUser), nameof(TBkpNummer.BkpChangedUser))]
    public partial TBkpNummer ZuEntity(BkpNummer model);
}
