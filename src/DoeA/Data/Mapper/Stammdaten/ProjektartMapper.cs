// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO.Stammdaten;
using DoeA.Interfaces;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper.Stammdaten;

/// <summary>Mapping zwischen TProjektart und dem DTO Projektart.</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(TextKonvertierung))]
public partial class ProjektartMapper : IEntityMapper<Projektart, TProjektart>
{
    [MapProperty(nameof(TProjektart.PraId), nameof(Projektart.Id))]
    [MapProperty(nameof(TProjektart.PraBezeichnung), nameof(Projektart.Bezeichnung))]
    [MapProperty(nameof(TProjektart.PraKurzbezeichnung), nameof(Projektart.Kurzbezeichnung))]
    [MapProperty(nameof(TProjektart.PraNaturalId), nameof(Projektart.NaturalId))]
    [MapProperty(nameof(TProjektart.PraAktiv), nameof(Projektart.Aktiv))]
    [MapProperty(nameof(TProjektart.PraInsertedDate), nameof(Projektart.InsertedDate))]
    [MapProperty(nameof(TProjektart.PraInsertedUser), nameof(Projektart.InsertedUser))]
    [MapProperty(nameof(TProjektart.PraChangedDate), nameof(Projektart.ChangedDate))]
    [MapProperty(nameof(TProjektart.PraChangedUser), nameof(Projektart.ChangedUser))]
    public partial Projektart ZuModel(TProjektart entity);

    [MapperRequiredMapping(RequiredMappingStrategy.Both)]
    [MapperIgnoreSource(nameof(Projektart.InsertedDateDateOnly))]
    [MapperIgnoreSource(nameof(Projektart.ChangedDateDateOnly))]
    // Navigationen bleiben aussen vor: EF verwaltet sie über die Fremdschlüssel, und der Rückweg würde referenzierte Entities beim Create erneut einfügen.
    [MapperIgnoreTarget(nameof(TProjektart.TAusschreibung))]
    [MapperIgnoreTarget(nameof(TProjektart.TExportgruppeProjektart))]
    [MapperIgnoreTarget(nameof(TProjektart.TLeistungsart))]
    [MapProperty(nameof(Projektart.Id), nameof(TProjektart.PraId))]
    [MapProperty(nameof(Projektart.Bezeichnung), nameof(TProjektart.PraBezeichnung))]
    [MapProperty(nameof(Projektart.Kurzbezeichnung), nameof(TProjektart.PraKurzbezeichnung))]
    [MapProperty(nameof(Projektart.NaturalId), nameof(TProjektart.PraNaturalId))]
    [MapProperty(nameof(Projektart.Aktiv), nameof(TProjektart.PraAktiv))]
    [MapProperty(nameof(Projektart.InsertedDate), nameof(TProjektart.PraInsertedDate))]
    [MapProperty(nameof(Projektart.InsertedUser), nameof(TProjektart.PraInsertedUser))]
    [MapProperty(nameof(Projektart.ChangedDate), nameof(TProjektart.PraChangedDate))]
    [MapProperty(nameof(Projektart.ChangedUser), nameof(TProjektart.PraChangedUser))]
    public partial TProjektart ZuEntity(Projektart model);
}
