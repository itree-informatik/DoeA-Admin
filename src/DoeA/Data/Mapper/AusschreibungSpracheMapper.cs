// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO;
using DoeA.Interfaces;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper;

/// <summary>Mapping zwischen TAusschreibungSprache und dem DTO AusschreibungSprache.</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(TextKonvertierung))]
public partial class AusschreibungSpracheMapper : IEntityMapper<AusschreibungSprache, TAusschreibungSprache>
{
    // Dokumentart, Sprache und Wert befüllt der AusschreibungService für die Sprachen-Matrix.
    [MapperIgnoreTarget(nameof(AusschreibungSprache.Dokumentart))]
    [MapperIgnoreTarget(nameof(AusschreibungSprache.Sprache))]
    [MapperIgnoreTarget(nameof(AusschreibungSprache.Wert))]
    [MapProperty(nameof(TAusschreibungSprache.AspId), nameof(AusschreibungSprache.Id))]
    [MapProperty(nameof(TAusschreibungSprache.AspSprId), nameof(AusschreibungSprache.SprId))]
    [MapProperty(nameof(TAusschreibungSprache.AspAusId), nameof(AusschreibungSprache.AusId))]
    [MapProperty(nameof(TAusschreibungSprache.AspDoaId), nameof(AusschreibungSprache.DoaId))]
    [MapProperty(nameof(TAusschreibungSprache.AspNaturalId), nameof(AusschreibungSprache.NaturalId))]
    [MapProperty(nameof(TAusschreibungSprache.AspAktiv), nameof(AusschreibungSprache.Aktiv))]
    [MapProperty(nameof(TAusschreibungSprache.AspInsertedDate), nameof(AusschreibungSprache.InsertedDate))]
    [MapProperty(nameof(TAusschreibungSprache.AspInsertedUser), nameof(AusschreibungSprache.InsertedUser))]
    [MapProperty(nameof(TAusschreibungSprache.AspChangedDate), nameof(AusschreibungSprache.ChangedDate))]
    [MapProperty(nameof(TAusschreibungSprache.AspChangedUser), nameof(AusschreibungSprache.ChangedUser))]
    public partial AusschreibungSprache ZuModel(TAusschreibungSprache entity);

    [MapperRequiredMapping(RequiredMappingStrategy.Both)]
    [MapperIgnoreSource(nameof(AusschreibungSprache.InsertedDateDateOnly))]
    [MapperIgnoreSource(nameof(AusschreibungSprache.ChangedDateDateOnly))]
    // Navigationen bleiben aussen vor: EF verwaltet sie über die Fremdschlüssel, und der Rückweg würde referenzierte Entities beim Create erneut einfügen.
    [MapperIgnoreTarget(nameof(TAusschreibungSprache.AspAus))]
    [MapperIgnoreTarget(nameof(TAusschreibungSprache.AspDoa))]
    [MapperIgnoreTarget(nameof(TAusschreibungSprache.AspSpr))]
    [MapperIgnoreSource(nameof(AusschreibungSprache.Dokumentart))]
    [MapperIgnoreSource(nameof(AusschreibungSprache.Sprache))]
    [MapperIgnoreSource(nameof(AusschreibungSprache.Wert))]
    [MapProperty(nameof(AusschreibungSprache.Id), nameof(TAusschreibungSprache.AspId))]
    [MapProperty(nameof(AusschreibungSprache.SprId), nameof(TAusschreibungSprache.AspSprId))]
    [MapProperty(nameof(AusschreibungSprache.AusId), nameof(TAusschreibungSprache.AspAusId))]
    [MapProperty(nameof(AusschreibungSprache.DoaId), nameof(TAusschreibungSprache.AspDoaId))]
    [MapProperty(nameof(AusschreibungSprache.NaturalId), nameof(TAusschreibungSprache.AspNaturalId))]
    [MapProperty(nameof(AusschreibungSprache.Aktiv), nameof(TAusschreibungSprache.AspAktiv))]
    [MapProperty(nameof(AusschreibungSprache.InsertedDate), nameof(TAusschreibungSprache.AspInsertedDate))]
    [MapProperty(nameof(AusschreibungSprache.InsertedUser), nameof(TAusschreibungSprache.AspInsertedUser))]
    [MapProperty(nameof(AusschreibungSprache.ChangedDate), nameof(TAusschreibungSprache.AspChangedDate))]
    [MapProperty(nameof(AusschreibungSprache.ChangedUser), nameof(TAusschreibungSprache.AspChangedUser))]
    public partial TAusschreibungSprache ZuEntity(AusschreibungSprache model);
}
