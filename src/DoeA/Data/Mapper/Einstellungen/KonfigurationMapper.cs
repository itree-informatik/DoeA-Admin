// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO.Einstellungen;
using DoeA.Interfaces;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper.Einstellungen;

/// <summary>Mapping zwischen TKonfiguration und dem DTO Konfiguration.</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(TextKonvertierung))]
public partial class KonfigurationMapper : IEntityMapper<Konfiguration, TKonfiguration>
{
    // Die typisierten Wert-Sichten leitet der KonfigurationService aus Wert und Typ ab.
    [MapperIgnoreTarget(nameof(Konfiguration.WertBool))]
    [MapperIgnoreTarget(nameof(Konfiguration.WertInt))]
    [MapperIgnoreTarget(nameof(Konfiguration.WertDate))]
    [MapperIgnoreTarget(nameof(Konfiguration.WertDecimal))]
    [MapProperty(nameof(TKonfiguration.KonId), nameof(Konfiguration.Id))]
    [MapProperty(nameof(TKonfiguration.KonBezeichnung), nameof(Konfiguration.Bezeichnung))]
    [MapProperty(nameof(TKonfiguration.KonWert), nameof(Konfiguration.Wert))]
    [MapProperty(nameof(TKonfiguration.KonTyp), nameof(Konfiguration.Typ))]
    [MapProperty(nameof(TKonfiguration.KonBeschreibung), nameof(Konfiguration.Beschreibung))]
    [MapProperty(nameof(TKonfiguration.KonNaturalId), nameof(Konfiguration.NaturalId))]
    [MapProperty(nameof(TKonfiguration.KonAktiv), nameof(Konfiguration.Aktiv))]
    [MapProperty(nameof(TKonfiguration.KonInsertedDate), nameof(Konfiguration.InsertedDate))]
    [MapProperty(nameof(TKonfiguration.KonInsertedUser), nameof(Konfiguration.InsertedUser))]
    [MapProperty(nameof(TKonfiguration.KonChangedDate), nameof(Konfiguration.ChangedDate))]
    [MapProperty(nameof(TKonfiguration.KonChangedUser), nameof(Konfiguration.ChangedUser))]
    public partial Konfiguration ZuModel(TKonfiguration entity);

    [MapperRequiredMapping(RequiredMappingStrategy.Both)]
    [MapperIgnoreSource(nameof(Konfiguration.InsertedDateDateOnly))]
    [MapperIgnoreSource(nameof(Konfiguration.ChangedDateDateOnly))]
    [MapperIgnoreSource(nameof(Konfiguration.WertBool))]
    [MapperIgnoreSource(nameof(Konfiguration.WertInt))]
    [MapperIgnoreSource(nameof(Konfiguration.WertDate))]
    [MapperIgnoreSource(nameof(Konfiguration.WertDecimal))]
    [MapperIgnoreSource(nameof(Konfiguration.WertIsInt))]
    [MapperIgnoreSource(nameof(Konfiguration.WertIsBool))]
    [MapperIgnoreSource(nameof(Konfiguration.WertIsDate))]
    [MapperIgnoreSource(nameof(Konfiguration.WertIsDecimal))]
    [MapperIgnoreSource(nameof(Konfiguration.WertIsJson))]
    [MapProperty(nameof(Konfiguration.Id), nameof(TKonfiguration.KonId))]
    [MapProperty(nameof(Konfiguration.Bezeichnung), nameof(TKonfiguration.KonBezeichnung))]
    [MapProperty(nameof(Konfiguration.Wert), nameof(TKonfiguration.KonWert))]
    [MapProperty(nameof(Konfiguration.Typ), nameof(TKonfiguration.KonTyp))]
    [MapProperty(nameof(Konfiguration.Beschreibung), nameof(TKonfiguration.KonBeschreibung))]
    [MapProperty(nameof(Konfiguration.NaturalId), nameof(TKonfiguration.KonNaturalId))]
    [MapProperty(nameof(Konfiguration.Aktiv), nameof(TKonfiguration.KonAktiv))]
    [MapProperty(nameof(Konfiguration.InsertedDate), nameof(TKonfiguration.KonInsertedDate))]
    [MapProperty(nameof(Konfiguration.InsertedUser), nameof(TKonfiguration.KonInsertedUser))]
    [MapProperty(nameof(Konfiguration.ChangedDate), nameof(TKonfiguration.KonChangedDate))]
    [MapProperty(nameof(Konfiguration.ChangedUser), nameof(TKonfiguration.KonChangedUser))]
    public partial TKonfiguration ZuEntity(Konfiguration model);
}
