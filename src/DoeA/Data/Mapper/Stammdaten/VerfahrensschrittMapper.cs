// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO.Stammdaten;
using DoeA.Interfaces;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper.Stammdaten;

/// <summary>Mapping zwischen TVerfahrensschritt und dem DTO Verfahrensschritt.</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(TextKonvertierung))]
public partial class VerfahrensschrittMapper : IEntityMapper<Verfahrensschritt, TVerfahrensschritt>
{
    [MapProperty(nameof(TVerfahrensschritt.VesId), nameof(Verfahrensschritt.Id))]
    [MapProperty(nameof(TVerfahrensschritt.VesBezeichnung), nameof(Verfahrensschritt.Bezeichnung))]
    [MapProperty(nameof(TVerfahrensschritt.VesSortierung), nameof(Verfahrensschritt.Sortierung))]
    [MapProperty(nameof(TVerfahrensschritt.VesNurWerktage), nameof(Verfahrensschritt.NurWerktage))]
    [MapProperty(nameof(TVerfahrensschritt.VesOffen), nameof(Verfahrensschritt.Offen))]
    [MapProperty(nameof(TVerfahrensschritt.VesSelektiv), nameof(Verfahrensschritt.Selektiv))]
    [MapProperty(nameof(TVerfahrensschritt.VesNaturalId), nameof(Verfahrensschritt.NaturalId))]
    [MapProperty(nameof(TVerfahrensschritt.VesAktiv), nameof(Verfahrensschritt.Aktiv))]
    [MapProperty(nameof(TVerfahrensschritt.VesInsertedDate), nameof(Verfahrensschritt.InsertedDate))]
    [MapProperty(nameof(TVerfahrensschritt.VesInsertedUser), nameof(Verfahrensschritt.InsertedUser))]
    [MapProperty(nameof(TVerfahrensschritt.VesChangedDate), nameof(Verfahrensschritt.ChangedDate))]
    [MapProperty(nameof(TVerfahrensschritt.VesChangedUser), nameof(Verfahrensschritt.ChangedUser))]
    public partial Verfahrensschritt ZuModel(TVerfahrensschritt entity);

    [MapperRequiredMapping(RequiredMappingStrategy.Both)]
    [MapperIgnoreSource(nameof(Verfahrensschritt.InsertedDateDateOnly))]
    [MapperIgnoreSource(nameof(Verfahrensschritt.ChangedDateDateOnly))]
    // Navigationen bleiben aussen vor: EF verwaltet sie über die Fremdschlüssel, und der Rückweg würde referenzierte Entities beim Create erneut einfügen.
    [MapperIgnoreTarget(nameof(TVerfahrensschritt.TAusschreibungVerfahrensschritt))]
    [MapProperty(nameof(Verfahrensschritt.Id), nameof(TVerfahrensschritt.VesId))]
    [MapProperty(nameof(Verfahrensschritt.Bezeichnung), nameof(TVerfahrensschritt.VesBezeichnung))]
    [MapProperty(nameof(Verfahrensschritt.Sortierung), nameof(TVerfahrensschritt.VesSortierung))]
    [MapProperty(nameof(Verfahrensschritt.NurWerktage), nameof(TVerfahrensschritt.VesNurWerktage))]
    [MapProperty(nameof(Verfahrensschritt.Offen), nameof(TVerfahrensschritt.VesOffen))]
    [MapProperty(nameof(Verfahrensschritt.Selektiv), nameof(TVerfahrensschritt.VesSelektiv))]
    [MapProperty(nameof(Verfahrensschritt.NaturalId), nameof(TVerfahrensschritt.VesNaturalId))]
    [MapProperty(nameof(Verfahrensschritt.Aktiv), nameof(TVerfahrensschritt.VesAktiv))]
    [MapProperty(nameof(Verfahrensschritt.InsertedDate), nameof(TVerfahrensschritt.VesInsertedDate))]
    [MapProperty(nameof(Verfahrensschritt.InsertedUser), nameof(TVerfahrensschritt.VesInsertedUser))]
    [MapProperty(nameof(Verfahrensschritt.ChangedDate), nameof(TVerfahrensschritt.VesChangedDate))]
    [MapProperty(nameof(Verfahrensschritt.ChangedUser), nameof(TVerfahrensschritt.VesChangedUser))]
    public partial TVerfahrensschritt ZuEntity(Verfahrensschritt model);
}
