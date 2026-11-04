// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO.Stammdaten;
using DoeA.Interfaces;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper.Stammdaten;

/// <summary>Mapping zwischen TVerfahrensart und dem DTO Verfahrensart.</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(TextKonvertierung))]
public partial class VerfahrensartMapper : IEntityMapper<Verfahrensart, TVerfahrensart>
{
    [MapProperty(nameof(TVerfahrensart.VeaId), nameof(Verfahrensart.Id))]
    [MapProperty(nameof(TVerfahrensart.VeaBezeichnung), nameof(Verfahrensart.Bezeichnung))]
    [MapProperty(nameof(TVerfahrensart.VeaKurzbezeichnung), nameof(Verfahrensart.Kurzbezeichnung))]
    [MapProperty(nameof(TVerfahrensart.VeaSortierung), nameof(Verfahrensart.Sortierung))]
    [MapProperty(nameof(TVerfahrensart.VeaNaturalId), nameof(Verfahrensart.NaturalId))]
    [MapProperty(nameof(TVerfahrensart.VeaAktiv), nameof(Verfahrensart.Aktiv))]
    [MapProperty(nameof(TVerfahrensart.VeaInsertedDate), nameof(Verfahrensart.InsertedDate))]
    [MapProperty(nameof(TVerfahrensart.VeaInsertedUser), nameof(Verfahrensart.InsertedUser))]
    [MapProperty(nameof(TVerfahrensart.VeaChangedDate), nameof(Verfahrensart.ChangedDate))]
    [MapProperty(nameof(TVerfahrensart.VeaChangedUser), nameof(Verfahrensart.ChangedUser))]
    public partial Verfahrensart ZuModel(TVerfahrensart entity);

    [MapperRequiredMapping(RequiredMappingStrategy.Both)]
    [MapperIgnoreSource(nameof(Verfahrensart.InsertedDateDateOnly))]
    [MapperIgnoreSource(nameof(Verfahrensart.ChangedDateDateOnly))]
    // Navigationen bleiben aussen vor: EF verwaltet sie über die Fremdschlüssel, und der Rückweg würde referenzierte Entities beim Create erneut einfügen.
    [MapperIgnoreTarget(nameof(TVerfahrensart.TAusschreibung))]
    [MapperIgnoreTarget(nameof(TVerfahrensart.TExportgruppeVerfahrensart))]
    [MapProperty(nameof(Verfahrensart.Id), nameof(TVerfahrensart.VeaId))]
    [MapProperty(nameof(Verfahrensart.Bezeichnung), nameof(TVerfahrensart.VeaBezeichnung))]
    [MapProperty(nameof(Verfahrensart.Kurzbezeichnung), nameof(TVerfahrensart.VeaKurzbezeichnung))]
    [MapProperty(nameof(Verfahrensart.Sortierung), nameof(TVerfahrensart.VeaSortierung))]
    [MapProperty(nameof(Verfahrensart.NaturalId), nameof(TVerfahrensart.VeaNaturalId))]
    [MapProperty(nameof(Verfahrensart.Aktiv), nameof(TVerfahrensart.VeaAktiv))]
    [MapProperty(nameof(Verfahrensart.InsertedDate), nameof(TVerfahrensart.VeaInsertedDate))]
    [MapProperty(nameof(Verfahrensart.InsertedUser), nameof(TVerfahrensart.VeaInsertedUser))]
    [MapProperty(nameof(Verfahrensart.ChangedDate), nameof(TVerfahrensart.VeaChangedDate))]
    [MapProperty(nameof(Verfahrensart.ChangedUser), nameof(TVerfahrensart.VeaChangedUser))]
    public partial TVerfahrensart ZuEntity(Verfahrensart model);
}
