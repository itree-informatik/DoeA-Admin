// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO.Stammdaten;
using DoeA.Interfaces;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper.Stammdaten;

/// <summary>Mapping zwischen TOffertezustand und dem DTO Offertezustand.</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(TextKonvertierung))]
public partial class OffertezustandMapper : IEntityMapper<Offertezustand, TOffertezustand>
{
    [MapProperty(nameof(TOffertezustand.OfzId), nameof(Offertezustand.Id))]
    [MapProperty(nameof(TOffertezustand.OfzBezeichnung), nameof(Offertezustand.Bezeichnung))]
    [MapProperty(nameof(TOffertezustand.OfzKurzbezeichnung), nameof(Offertezustand.Kurzbezeichnung))]
    [MapProperty(nameof(TOffertezustand.OfzSchlusszustand), nameof(Offertezustand.Schlusszustand))]
    [MapProperty(nameof(TOffertezustand.OfzPublikation), nameof(Offertezustand.Publikation))]
    [MapProperty(nameof(TOffertezustand.OfzNaturalId), nameof(Offertezustand.NaturalId))]
    [MapProperty(nameof(TOffertezustand.OfzAktiv), nameof(Offertezustand.Aktiv))]
    [MapProperty(nameof(TOffertezustand.OfzInsertedDate), nameof(Offertezustand.InsertedDate))]
    [MapProperty(nameof(TOffertezustand.OfzInsertedUser), nameof(Offertezustand.InsertedUser))]
    [MapProperty(nameof(TOffertezustand.OfzChangedDate), nameof(Offertezustand.ChangedDate))]
    [MapProperty(nameof(TOffertezustand.OfzChangedUser), nameof(Offertezustand.ChangedUser))]
    public partial Offertezustand ZuModel(TOffertezustand entity);

    [MapperRequiredMapping(RequiredMappingStrategy.Both)]
    [MapperIgnoreSource(nameof(Offertezustand.InsertedDateDateOnly))]
    [MapperIgnoreSource(nameof(Offertezustand.ChangedDateDateOnly))]
    // Navigationen bleiben aussen vor: EF verwaltet sie über die Fremdschlüssel, und der Rückweg würde referenzierte Entities beim Create erneut einfügen.
    [MapperIgnoreTarget(nameof(TOffertezustand.TOfferteOffertezustand))]
    [MapProperty(nameof(Offertezustand.Id), nameof(TOffertezustand.OfzId))]
    [MapProperty(nameof(Offertezustand.Bezeichnung), nameof(TOffertezustand.OfzBezeichnung))]
    [MapProperty(nameof(Offertezustand.Kurzbezeichnung), nameof(TOffertezustand.OfzKurzbezeichnung))]
    [MapProperty(nameof(Offertezustand.Schlusszustand), nameof(TOffertezustand.OfzSchlusszustand))]
    [MapProperty(nameof(Offertezustand.Publikation), nameof(TOffertezustand.OfzPublikation))]
    [MapProperty(nameof(Offertezustand.NaturalId), nameof(TOffertezustand.OfzNaturalId))]
    [MapProperty(nameof(Offertezustand.Aktiv), nameof(TOffertezustand.OfzAktiv))]
    [MapProperty(nameof(Offertezustand.InsertedDate), nameof(TOffertezustand.OfzInsertedDate))]
    [MapProperty(nameof(Offertezustand.InsertedUser), nameof(TOffertezustand.OfzInsertedUser))]
    [MapProperty(nameof(Offertezustand.ChangedDate), nameof(TOffertezustand.OfzChangedDate))]
    [MapProperty(nameof(Offertezustand.ChangedUser), nameof(TOffertezustand.OfzChangedUser))]
    public partial TOffertezustand ZuEntity(Offertezustand model);
}
