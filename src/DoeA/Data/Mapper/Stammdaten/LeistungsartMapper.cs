// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO.Stammdaten;
using DoeA.Interfaces;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper.Stammdaten;

/// <summary>Mapping zwischen TLeistungsart und dem DTO Leistungsart.</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(TextKonvertierung))]
public partial class LeistungsartMapper : IEntityMapper<Leistungsart, TLeistungsart>
{
    private readonly ProjektartMapper _projektartMapper;

    public LeistungsartMapper(ProjektartMapper projektartMapper)
    {
        _projektartMapper = projektartMapper;
    }

    [MapProperty(nameof(TLeistungsart.LeaId), nameof(Leistungsart.Id))]
    [MapProperty(nameof(TLeistungsart.LeaPraId), nameof(Leistungsart.PraId))]
    [MapProperty(nameof(TLeistungsart.LeaBezeichnung), nameof(Leistungsart.Bezeichnung))]
    [MapProperty(nameof(TLeistungsart.LeaKurzbezeichnung), nameof(Leistungsart.Kurzbezeichnung))]
    [MapProperty(nameof(TLeistungsart.LeaNaturalId), nameof(Leistungsart.NaturalId))]
    [MapProperty(nameof(TLeistungsart.LeaAktiv), nameof(Leistungsart.Aktiv))]
    [MapProperty(nameof(TLeistungsart.LeaInsertedDate), nameof(Leistungsart.InsertedDate))]
    [MapProperty(nameof(TLeistungsart.LeaInsertedUser), nameof(Leistungsart.InsertedUser))]
    [MapProperty(nameof(TLeistungsart.LeaChangedDate), nameof(Leistungsart.ChangedDate))]
    [MapProperty(nameof(TLeistungsart.LeaChangedUser), nameof(Leistungsart.ChangedUser))]
    [MapProperty(nameof(TLeistungsart.LeaPra), nameof(Leistungsart.Projektart), Use = nameof(ZuProjektart))]
    public partial Leistungsart ZuModel(TLeistungsart entity);

    [MapperRequiredMapping(RequiredMappingStrategy.Both)]
    [MapperIgnoreSource(nameof(Leistungsart.InsertedDateDateOnly))]
    [MapperIgnoreSource(nameof(Leistungsart.ChangedDateDateOnly))]
    // Navigationen bleiben aussen vor: EF verwaltet sie über die Fremdschlüssel, und der Rückweg würde referenzierte Entities beim Create erneut einfügen.
    [MapperIgnoreTarget(nameof(TLeistungsart.LeaPra))]
    [MapperIgnoreTarget(nameof(TLeistungsart.TAusschreibung))]
    [MapperIgnoreTarget(nameof(TLeistungsart.TExportgruppeLeistungsart))]
    [MapperIgnoreSource(nameof(Leistungsart.Projektart))]
    [MapProperty(nameof(Leistungsart.Id), nameof(TLeistungsart.LeaId))]
    [MapProperty(nameof(Leistungsart.PraId), nameof(TLeistungsart.LeaPraId))]
    [MapProperty(nameof(Leistungsart.Bezeichnung), nameof(TLeistungsart.LeaBezeichnung))]
    [MapProperty(nameof(Leistungsart.Kurzbezeichnung), nameof(TLeistungsart.LeaKurzbezeichnung))]
    [MapProperty(nameof(Leistungsart.NaturalId), nameof(TLeistungsart.LeaNaturalId))]
    [MapProperty(nameof(Leistungsart.Aktiv), nameof(TLeistungsart.LeaAktiv))]
    [MapProperty(nameof(Leistungsart.InsertedDate), nameof(TLeistungsart.LeaInsertedDate))]
    [MapProperty(nameof(Leistungsart.InsertedUser), nameof(TLeistungsart.LeaInsertedUser))]
    [MapProperty(nameof(Leistungsart.ChangedDate), nameof(TLeistungsart.LeaChangedDate))]
    [MapProperty(nameof(Leistungsart.ChangedUser), nameof(TLeistungsart.LeaChangedUser))]
    public partial TLeistungsart ZuEntity(Leistungsart model);

    // EF liefert Navigationen ohne Include als null.
    private Projektart? ZuProjektart(TProjektart? projektart) => projektart == null ? null : _projektartMapper.ZuModel(projektart);
}
