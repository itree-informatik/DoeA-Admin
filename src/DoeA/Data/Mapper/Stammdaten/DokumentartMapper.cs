// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO.Stammdaten;
using DoeA.Interfaces;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper.Stammdaten;

/// <summary>Mapping zwischen TDokumentart und dem DTO Dokumentart.</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(TextKonvertierung))]
public partial class DokumentartMapper : IEntityMapper<Dokumentart, TDokumentart>
{
    [MapProperty(nameof(TDokumentart.DoaId), nameof(Dokumentart.Id))]
    [MapProperty(nameof(TDokumentart.DoaBezeichnung), nameof(Dokumentart.Bezeichnung))]
    [MapProperty(nameof(TDokumentart.DoaNaturalId), nameof(Dokumentart.NaturalId))]
    [MapProperty(nameof(TDokumentart.DoaSortierung), nameof(Dokumentart.Sortierung))]
    [MapProperty(nameof(TDokumentart.DoaAktiv), nameof(Dokumentart.Aktiv))]
    [MapProperty(nameof(TDokumentart.DoaInsertedDate), nameof(Dokumentart.InsertedDate))]
    [MapProperty(nameof(TDokumentart.DoaInsertedUser), nameof(Dokumentart.InsertedUser))]
    [MapProperty(nameof(TDokumentart.DoaChangedDate), nameof(Dokumentart.ChangedDate))]
    [MapProperty(nameof(TDokumentart.DoaChangedUser), nameof(Dokumentart.ChangedUser))]
    public partial Dokumentart ZuModel(TDokumentart entity);

    [MapperRequiredMapping(RequiredMappingStrategy.Both)]
    [MapperIgnoreSource(nameof(Dokumentart.InsertedDateDateOnly))]
    [MapperIgnoreSource(nameof(Dokumentart.ChangedDateDateOnly))]
    // Navigationen bleiben aussen vor: EF verwaltet sie über die Fremdschlüssel, und der Rückweg würde referenzierte Entities beim Create erneut einfügen.
    [MapperIgnoreTarget(nameof(TDokumentart.TAusschreibungSprache))]
    [MapProperty(nameof(Dokumentart.Id), nameof(TDokumentart.DoaId))]
    [MapProperty(nameof(Dokumentart.Bezeichnung), nameof(TDokumentart.DoaBezeichnung))]
    [MapProperty(nameof(Dokumentart.NaturalId), nameof(TDokumentart.DoaNaturalId))]
    [MapProperty(nameof(Dokumentart.Sortierung), nameof(TDokumentart.DoaSortierung))]
    [MapProperty(nameof(Dokumentart.Aktiv), nameof(TDokumentart.DoaAktiv))]
    [MapProperty(nameof(Dokumentart.InsertedDate), nameof(TDokumentart.DoaInsertedDate))]
    [MapProperty(nameof(Dokumentart.InsertedUser), nameof(TDokumentart.DoaInsertedUser))]
    [MapProperty(nameof(Dokumentart.ChangedDate), nameof(TDokumentart.DoaChangedDate))]
    [MapProperty(nameof(Dokumentart.ChangedUser), nameof(TDokumentart.DoaChangedUser))]
    public partial TDokumentart ZuEntity(Dokumentart model);
}
