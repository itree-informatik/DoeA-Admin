// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO;
using DoeA.Interfaces;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper;

/// <summary>Mapping zwischen TLosBkpNummer und dem DTO LosBkpNummer.</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(TextKonvertierung))]
public partial class LosBkpNummerMapper : IEntityMapper<LosBkpNummer, TLosBkpNummer>
{
    // Die Stammdaten-Navigation lädt der LosService nach.
    [MapperIgnoreTarget(nameof(LosBkpNummer.BkpNummer))]
    [MapProperty(nameof(TLosBkpNummer.LbnId), nameof(LosBkpNummer.Id))]
    [MapProperty(nameof(TLosBkpNummer.LbnLosId), nameof(LosBkpNummer.LosId))]
    [MapProperty(nameof(TLosBkpNummer.LbnBkpId), nameof(LosBkpNummer.BkpId))]
    [MapProperty(nameof(TLosBkpNummer.LbnNaturalId), nameof(LosBkpNummer.NaturalId))]
    [MapProperty(nameof(TLosBkpNummer.LbnAktiv), nameof(LosBkpNummer.Aktiv))]
    [MapProperty(nameof(TLosBkpNummer.LbnInsertedDate), nameof(LosBkpNummer.InsertedDate))]
    [MapProperty(nameof(TLosBkpNummer.LbnInsertedUser), nameof(LosBkpNummer.InsertedUser))]
    [MapProperty(nameof(TLosBkpNummer.LbnChangedDate), nameof(LosBkpNummer.ChangedDate))]
    [MapProperty(nameof(TLosBkpNummer.LbnChangedUser), nameof(LosBkpNummer.ChangedUser))]
    public partial LosBkpNummer ZuModel(TLosBkpNummer entity);

    [MapperRequiredMapping(RequiredMappingStrategy.Both)]
    [MapperIgnoreSource(nameof(LosBkpNummer.InsertedDateDateOnly))]
    [MapperIgnoreSource(nameof(LosBkpNummer.ChangedDateDateOnly))]
    // Navigationen bleiben aussen vor: EF verwaltet sie über die Fremdschlüssel, und der Rückweg würde referenzierte Entities beim Create erneut einfügen.
    [MapperIgnoreTarget(nameof(TLosBkpNummer.LbnBkp))]
    [MapperIgnoreTarget(nameof(TLosBkpNummer.LbnLos))]
    [MapperIgnoreSource(nameof(LosBkpNummer.BkpNummer))]
    [MapProperty(nameof(LosBkpNummer.Id), nameof(TLosBkpNummer.LbnId))]
    [MapProperty(nameof(LosBkpNummer.LosId), nameof(TLosBkpNummer.LbnLosId))]
    [MapProperty(nameof(LosBkpNummer.BkpId), nameof(TLosBkpNummer.LbnBkpId))]
    [MapProperty(nameof(LosBkpNummer.NaturalId), nameof(TLosBkpNummer.LbnNaturalId))]
    [MapProperty(nameof(LosBkpNummer.Aktiv), nameof(TLosBkpNummer.LbnAktiv))]
    [MapProperty(nameof(LosBkpNummer.InsertedDate), nameof(TLosBkpNummer.LbnInsertedDate))]
    [MapProperty(nameof(LosBkpNummer.InsertedUser), nameof(TLosBkpNummer.LbnInsertedUser))]
    [MapProperty(nameof(LosBkpNummer.ChangedDate), nameof(TLosBkpNummer.LbnChangedDate))]
    [MapProperty(nameof(LosBkpNummer.ChangedUser), nameof(TLosBkpNummer.LbnChangedUser))]
    public partial TLosBkpNummer ZuEntity(LosBkpNummer model);
}
