// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO;
using DoeA.Interfaces;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper;

/// <summary>Mapping zwischen TOfferte und dem DTO Offerte; Anbieter und Zustandsverlauf lädt der Service nach.</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(TextKonvertierung))]
public partial class OfferteMapper : IEntityMapper<Offerte, TOfferte>
{
    [MapperIgnoreTarget(nameof(Offerte.Anbieter))]
    [MapperIgnoreTarget(nameof(Offerte.Offertenzustaende))]
    [MapProperty(nameof(TOfferte.OffId), nameof(Offerte.Id))]
    [MapProperty(nameof(TOfferte.OffLosId), nameof(Offerte.LosId))]
    [MapProperty(nameof(TOfferte.OffAnbId), nameof(Offerte.AnbId))]
    [MapProperty(nameof(TOfferte.OffBemerkung), nameof(Offerte.Bemerkung))]
    [MapProperty(nameof(TOfferte.OffZuschlagswertChf), nameof(Offerte.ZuschlagswertChf))]
    [MapProperty(nameof(TOfferte.OffAnzahlMitarbeiter), nameof(Offerte.AnzahlMitarbeiter))]
    [MapProperty(nameof(TOfferte.OffNaturalId), nameof(Offerte.NaturalId))]
    [MapProperty(nameof(TOfferte.OffAktiv), nameof(Offerte.Aktiv))]
    [MapProperty(nameof(TOfferte.OffInsertedDate), nameof(Offerte.InsertedDate))]
    [MapProperty(nameof(TOfferte.OffInsertedUser), nameof(Offerte.InsertedUser))]
    [MapProperty(nameof(TOfferte.OffChangedDate), nameof(Offerte.ChangedDate))]
    [MapProperty(nameof(TOfferte.OffChangedUser), nameof(Offerte.ChangedUser))]
    public partial Offerte ZuModel(TOfferte entity);

    [MapperRequiredMapping(RequiredMappingStrategy.Both)]
    [MapperIgnoreSource(nameof(Offerte.InsertedDateDateOnly))]
    [MapperIgnoreSource(nameof(Offerte.ChangedDateDateOnly))]
    // Navigationen bleiben aussen vor: EF verwaltet sie über die Fremdschlüssel, und der Rückweg würde referenzierte Entities beim Create erneut einfügen.
    [MapperIgnoreTarget(nameof(TOfferte.OffAnb))]
    [MapperIgnoreTarget(nameof(TOfferte.OffLos))]
    [MapperIgnoreTarget(nameof(TOfferte.TOfferteOffertezustand))]
    [MapperIgnoreSource(nameof(Offerte.Anbieter))]
    [MapperIgnoreSource(nameof(Offerte.Offertenzustaende))]
    [MapProperty(nameof(Offerte.Id), nameof(TOfferte.OffId))]
    [MapProperty(nameof(Offerte.LosId), nameof(TOfferte.OffLosId))]
    [MapProperty(nameof(Offerte.AnbId), nameof(TOfferte.OffAnbId))]
    [MapProperty(nameof(Offerte.Bemerkung), nameof(TOfferte.OffBemerkung))]
    [MapProperty(nameof(Offerte.ZuschlagswertChf), nameof(TOfferte.OffZuschlagswertChf))]
    [MapProperty(nameof(Offerte.AnzahlMitarbeiter), nameof(TOfferte.OffAnzahlMitarbeiter))]
    [MapProperty(nameof(Offerte.NaturalId), nameof(TOfferte.OffNaturalId))]
    [MapProperty(nameof(Offerte.Aktiv), nameof(TOfferte.OffAktiv))]
    [MapProperty(nameof(Offerte.InsertedDate), nameof(TOfferte.OffInsertedDate))]
    [MapProperty(nameof(Offerte.InsertedUser), nameof(TOfferte.OffInsertedUser))]
    [MapProperty(nameof(Offerte.ChangedDate), nameof(TOfferte.OffChangedDate))]
    [MapProperty(nameof(Offerte.ChangedUser), nameof(TOfferte.OffChangedUser))]
    public partial TOfferte ZuEntity(Offerte model);
}
