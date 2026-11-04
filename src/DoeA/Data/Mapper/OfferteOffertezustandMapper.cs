// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO;
using DoeA.Interfaces;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper;

/// <summary>Mapping zwischen TOfferteOffertezustand und dem DTO OfferteOffertezustand inkl. Umrechnung des Datums zwischen DateOnly und DateTime.</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(TextKonvertierung))]
public partial class OfferteOffertezustandMapper : IEntityMapper<OfferteOffertezustand, TOfferteOffertezustand>
{
    // Bezeichnung ist der Anzeigetext des Zustands, den der Service aus dem Offertezustand setzt.
    [MapperIgnoreTarget(nameof(OfferteOffertezustand.Bezeichnung))]
    [MapProperty(nameof(TOfferteOffertezustand.OozId), nameof(OfferteOffertezustand.Id))]
    [MapProperty(nameof(TOfferteOffertezustand.OozOffId), nameof(OfferteOffertezustand.OffId))]
    [MapProperty(nameof(TOfferteOffertezustand.OozOfzId), nameof(OfferteOffertezustand.OfzId))]
    [MapProperty(nameof(TOfferteOffertezustand.OozDatum), nameof(OfferteOffertezustand.Datum), Use = nameof(ZuDateTime))]
    [MapProperty(nameof(TOfferteOffertezustand.OozEingereicht), nameof(OfferteOffertezustand.Eingereicht))]
    [MapProperty(nameof(TOfferteOffertezustand.OozVerzoegerungDurchAuftraggeber), nameof(OfferteOffertezustand.VerzoegerungDurchAuftraggeber))]
    [MapProperty(nameof(TOfferteOffertezustand.OozNaturalId), nameof(OfferteOffertezustand.NaturalId))]
    [MapProperty(nameof(TOfferteOffertezustand.OozAktiv), nameof(OfferteOffertezustand.Aktiv))]
    [MapProperty(nameof(TOfferteOffertezustand.OozInsertedDate), nameof(OfferteOffertezustand.InsertedDate))]
    [MapProperty(nameof(TOfferteOffertezustand.OozInsertedUser), nameof(OfferteOffertezustand.InsertedUser))]
    [MapProperty(nameof(TOfferteOffertezustand.OozChangedDate), nameof(OfferteOffertezustand.ChangedDate))]
    [MapProperty(nameof(TOfferteOffertezustand.OozChangedUser), nameof(OfferteOffertezustand.ChangedUser))]
    public partial OfferteOffertezustand ZuModel(TOfferteOffertezustand entity);

    [MapperRequiredMapping(RequiredMappingStrategy.Both)]
    [MapperIgnoreSource(nameof(OfferteOffertezustand.InsertedDateDateOnly))]
    [MapperIgnoreSource(nameof(OfferteOffertezustand.ChangedDateDateOnly))]
    // Navigationen bleiben aussen vor: EF verwaltet sie über die Fremdschlüssel, und der Rückweg würde referenzierte Entities beim Create erneut einfügen.
    [MapperIgnoreTarget(nameof(TOfferteOffertezustand.OozOff))]
    [MapperIgnoreTarget(nameof(TOfferteOffertezustand.OozOfz))]
    [MapperIgnoreSource(nameof(OfferteOffertezustand.Bezeichnung))]
    [MapProperty(nameof(OfferteOffertezustand.Id), nameof(TOfferteOffertezustand.OozId))]
    [MapProperty(nameof(OfferteOffertezustand.OffId), nameof(TOfferteOffertezustand.OozOffId))]
    [MapProperty(nameof(OfferteOffertezustand.OfzId), nameof(TOfferteOffertezustand.OozOfzId))]
    [MapProperty(nameof(OfferteOffertezustand.Datum), nameof(TOfferteOffertezustand.OozDatum), Use = nameof(ZuDateOnly))]
    [MapProperty(nameof(OfferteOffertezustand.Eingereicht), nameof(TOfferteOffertezustand.OozEingereicht))]
    [MapProperty(nameof(OfferteOffertezustand.VerzoegerungDurchAuftraggeber), nameof(TOfferteOffertezustand.OozVerzoegerungDurchAuftraggeber))]
    [MapProperty(nameof(OfferteOffertezustand.NaturalId), nameof(TOfferteOffertezustand.OozNaturalId))]
    [MapProperty(nameof(OfferteOffertezustand.Aktiv), nameof(TOfferteOffertezustand.OozAktiv))]
    [MapProperty(nameof(OfferteOffertezustand.InsertedDate), nameof(TOfferteOffertezustand.OozInsertedDate))]
    [MapProperty(nameof(OfferteOffertezustand.InsertedUser), nameof(TOfferteOffertezustand.OozInsertedUser))]
    [MapProperty(nameof(OfferteOffertezustand.ChangedDate), nameof(TOfferteOffertezustand.OozChangedDate))]
    [MapProperty(nameof(OfferteOffertezustand.ChangedUser), nameof(TOfferteOffertezustand.OozChangedUser))]
    public partial TOfferteOffertezustand ZuEntity(OfferteOffertezustand model);

    // Die DB speichert ein reines Datum, das DTO ein DateTime? für den DatePicker.
    private DateTime? ZuDateTime(DateOnly datum) => datum.ToDateTime(TimeOnly.MinValue);

    // Ohne Datum gibt es keinen Zustandswechsel: default wäre der 01.01.0001 und würde die
    // chronologische Zustandshistorie verfälschen. Der OfferteOffertezustandValidator fängt den Fall vorher ab.
    private DateOnly ZuDateOnly(DateTime? datum) => datum.HasValue
        ? DateOnly.FromDateTime(datum.Value)
        : throw new ArgumentNullException(nameof(datum));
}
