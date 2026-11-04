// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO;
using DoeA.Interfaces;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper;

/// <summary>Mapping zwischen TAusschreibungCpvNummer und dem DTO AusschreibungCpvNummer.</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(TextKonvertierung))]
public partial class AusschreibungCpvNummerMapper : IEntityMapper<AusschreibungCpvNummer, TAusschreibungCpvNummer>
{
    // Die Stammdaten-Navigation lädt der AusschreibungService nach.
    [MapperIgnoreTarget(nameof(AusschreibungCpvNummer.CpvNummer))]
    [MapProperty(nameof(TAusschreibungCpvNummer.AcnId), nameof(AusschreibungCpvNummer.Id))]
    [MapProperty(nameof(TAusschreibungCpvNummer.AcnAusId), nameof(AusschreibungCpvNummer.AusId))]
    [MapProperty(nameof(TAusschreibungCpvNummer.AcnCpvId), nameof(AusschreibungCpvNummer.CpvId))]
    [MapProperty(nameof(TAusschreibungCpvNummer.AcnNaturalId), nameof(AusschreibungCpvNummer.NaturalId))]
    [MapProperty(nameof(TAusschreibungCpvNummer.AcnAktiv), nameof(AusschreibungCpvNummer.Aktiv))]
    [MapProperty(nameof(TAusschreibungCpvNummer.AcnInsertedDate), nameof(AusschreibungCpvNummer.InsertedDate))]
    [MapProperty(nameof(TAusschreibungCpvNummer.AcnInsertedUser), nameof(AusschreibungCpvNummer.InsertedUser))]
    [MapProperty(nameof(TAusschreibungCpvNummer.AcnChangedDate), nameof(AusschreibungCpvNummer.ChangedDate))]
    [MapProperty(nameof(TAusschreibungCpvNummer.AcnChangedUser), nameof(AusschreibungCpvNummer.ChangedUser))]
    public partial AusschreibungCpvNummer ZuModel(TAusschreibungCpvNummer entity);

    [MapperRequiredMapping(RequiredMappingStrategy.Both)]
    [MapperIgnoreSource(nameof(AusschreibungCpvNummer.InsertedDateDateOnly))]
    [MapperIgnoreSource(nameof(AusschreibungCpvNummer.ChangedDateDateOnly))]
    // Navigationen bleiben aussen vor: EF verwaltet sie über die Fremdschlüssel, und der Rückweg würde referenzierte Entities beim Create erneut einfügen.
    [MapperIgnoreTarget(nameof(TAusschreibungCpvNummer.AcnAus))]
    [MapperIgnoreTarget(nameof(TAusschreibungCpvNummer.AcnCpv))]
    [MapperIgnoreSource(nameof(AusschreibungCpvNummer.CpvNummer))]
    [MapProperty(nameof(AusschreibungCpvNummer.Id), nameof(TAusschreibungCpvNummer.AcnId))]
    [MapProperty(nameof(AusschreibungCpvNummer.AusId), nameof(TAusschreibungCpvNummer.AcnAusId))]
    [MapProperty(nameof(AusschreibungCpvNummer.CpvId), nameof(TAusschreibungCpvNummer.AcnCpvId))]
    [MapProperty(nameof(AusschreibungCpvNummer.NaturalId), nameof(TAusschreibungCpvNummer.AcnNaturalId))]
    [MapProperty(nameof(AusschreibungCpvNummer.Aktiv), nameof(TAusschreibungCpvNummer.AcnAktiv))]
    [MapProperty(nameof(AusschreibungCpvNummer.InsertedDate), nameof(TAusschreibungCpvNummer.AcnInsertedDate))]
    [MapProperty(nameof(AusschreibungCpvNummer.InsertedUser), nameof(TAusschreibungCpvNummer.AcnInsertedUser))]
    [MapProperty(nameof(AusschreibungCpvNummer.ChangedDate), nameof(TAusschreibungCpvNummer.AcnChangedDate))]
    [MapProperty(nameof(AusschreibungCpvNummer.ChangedUser), nameof(TAusschreibungCpvNummer.AcnChangedUser))]
    public partial TAusschreibungCpvNummer ZuEntity(AusschreibungCpvNummer model);
}
