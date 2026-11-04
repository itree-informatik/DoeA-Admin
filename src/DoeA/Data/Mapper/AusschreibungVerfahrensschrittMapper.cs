// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO;
using DoeA.Interfaces;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper;

/// <summary>Mapping zwischen TAusschreibungVerfahrensschritt und dem DTO AusschreibungVerfahrensschritt.</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(TextKonvertierung))]
public partial class AusschreibungVerfahrensschrittMapper : IEntityMapper<AusschreibungVerfahrensschritt, TAusschreibungVerfahrensschritt>
{
    // Die Stammdaten-Navigation lädt der Service nach.
    [MapperIgnoreTarget(nameof(AusschreibungVerfahrensschritt.Verfahrensschritt))]
    [MapProperty(nameof(TAusschreibungVerfahrensschritt.AvsId), nameof(AusschreibungVerfahrensschritt.Id))]
    [MapProperty(nameof(TAusschreibungVerfahrensschritt.AvsAusId), nameof(AusschreibungVerfahrensschritt.AusId))]
    [MapProperty(nameof(TAusschreibungVerfahrensschritt.AvsVesId), nameof(AusschreibungVerfahrensschritt.VesId))]
    [MapProperty(nameof(TAusschreibungVerfahrensschritt.AvsDatumVon), nameof(AusschreibungVerfahrensschritt.DatumVon))]
    [MapProperty(nameof(TAusschreibungVerfahrensschritt.AvsDatumBis), nameof(AusschreibungVerfahrensschritt.DatumBis))]
    [MapProperty(nameof(TAusschreibungVerfahrensschritt.AvsNaturalId), nameof(AusschreibungVerfahrensschritt.NaturalId))]
    [MapProperty(nameof(TAusschreibungVerfahrensschritt.AvsAktiv), nameof(AusschreibungVerfahrensschritt.Aktiv))]
    [MapProperty(nameof(TAusschreibungVerfahrensschritt.AvsInsertedDate), nameof(AusschreibungVerfahrensschritt.InsertedDate))]
    [MapProperty(nameof(TAusschreibungVerfahrensschritt.AvsInsertedUser), nameof(AusschreibungVerfahrensschritt.InsertedUser))]
    [MapProperty(nameof(TAusschreibungVerfahrensschritt.AvsChangedDate), nameof(AusschreibungVerfahrensschritt.ChangedDate))]
    [MapProperty(nameof(TAusschreibungVerfahrensschritt.AvsChangedUser), nameof(AusschreibungVerfahrensschritt.ChangedUser))]
    public partial AusschreibungVerfahrensschritt ZuModel(TAusschreibungVerfahrensschritt entity);

    [MapperRequiredMapping(RequiredMappingStrategy.Both)]
    [MapperIgnoreSource(nameof(AusschreibungVerfahrensschritt.InsertedDateDateOnly))]
    [MapperIgnoreSource(nameof(AusschreibungVerfahrensschritt.ChangedDateDateOnly))]
    // Navigationen bleiben aussen vor: EF verwaltet sie über die Fremdschlüssel, und der Rückweg würde referenzierte Entities beim Create erneut einfügen.
    [MapperIgnoreTarget(nameof(TAusschreibungVerfahrensschritt.AvsAus))]
    [MapperIgnoreTarget(nameof(TAusschreibungVerfahrensschritt.AvsVes))]
    [MapperIgnoreSource(nameof(AusschreibungVerfahrensschritt.Verfahrensschritt))]
    [MapperIgnoreSource(nameof(AusschreibungVerfahrensschritt.Dauer))]
    [MapProperty(nameof(AusschreibungVerfahrensschritt.Id), nameof(TAusschreibungVerfahrensschritt.AvsId))]
    [MapProperty(nameof(AusschreibungVerfahrensschritt.AusId), nameof(TAusschreibungVerfahrensschritt.AvsAusId))]
    [MapProperty(nameof(AusschreibungVerfahrensschritt.VesId), nameof(TAusschreibungVerfahrensschritt.AvsVesId))]
    [MapProperty(nameof(AusschreibungVerfahrensschritt.DatumVon), nameof(TAusschreibungVerfahrensschritt.AvsDatumVon))]
    [MapProperty(nameof(AusschreibungVerfahrensschritt.DatumBis), nameof(TAusschreibungVerfahrensschritt.AvsDatumBis))]
    [MapProperty(nameof(AusschreibungVerfahrensschritt.NaturalId), nameof(TAusschreibungVerfahrensschritt.AvsNaturalId))]
    [MapProperty(nameof(AusschreibungVerfahrensschritt.Aktiv), nameof(TAusschreibungVerfahrensschritt.AvsAktiv))]
    [MapProperty(nameof(AusschreibungVerfahrensschritt.InsertedDate), nameof(TAusschreibungVerfahrensschritt.AvsInsertedDate))]
    [MapProperty(nameof(AusschreibungVerfahrensschritt.InsertedUser), nameof(TAusschreibungVerfahrensschritt.AvsInsertedUser))]
    [MapProperty(nameof(AusschreibungVerfahrensschritt.ChangedDate), nameof(TAusschreibungVerfahrensschritt.AvsChangedDate))]
    [MapProperty(nameof(AusschreibungVerfahrensschritt.ChangedUser), nameof(TAusschreibungVerfahrensschritt.AvsChangedUser))]
    public partial TAusschreibungVerfahrensschritt ZuEntity(AusschreibungVerfahrensschritt model);
}
