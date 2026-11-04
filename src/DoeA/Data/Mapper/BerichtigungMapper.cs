// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO;
using DoeA.Interfaces;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper;

/// <summary>Mapping zwischen TBerichtigung und dem DTO Berichtigung.</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(TextKonvertierung))]
public partial class BerichtigungMapper : IEntityMapper<Berichtigung, TBerichtigung>
{
    [MapProperty(nameof(TBerichtigung.BerId), nameof(Berichtigung.Id))]
    [MapProperty(nameof(TBerichtigung.BerAusId), nameof(Berichtigung.AusId))]
    [MapProperty(nameof(TBerichtigung.BerEingereicht), nameof(Berichtigung.Eingereicht))]
    [MapProperty(nameof(TBerichtigung.BerPubliziert), nameof(Berichtigung.Publiziert))]
    [MapProperty(nameof(TBerichtigung.BerVerzoegerungDurchAuftraggeber), nameof(Berichtigung.VerzoegerungDurchAuftraggeber))]
    [MapProperty(nameof(TBerichtigung.BerBemerkungen), nameof(Berichtigung.Bemerkungen))]
    [MapProperty(nameof(TBerichtigung.BerNaturalId), nameof(Berichtigung.NaturalId))]
    [MapProperty(nameof(TBerichtigung.BerAktiv), nameof(Berichtigung.Aktiv))]
    [MapProperty(nameof(TBerichtigung.BerInsertedDate), nameof(Berichtigung.InsertedDate))]
    [MapProperty(nameof(TBerichtigung.BerInsertedUser), nameof(Berichtigung.InsertedUser))]
    [MapProperty(nameof(TBerichtigung.BerChangedDate), nameof(Berichtigung.ChangedDate))]
    [MapProperty(nameof(TBerichtigung.BerChangedUser), nameof(Berichtigung.ChangedUser))]
    public partial Berichtigung ZuModel(TBerichtigung entity);

    [MapperRequiredMapping(RequiredMappingStrategy.Both)]
    [MapperIgnoreSource(nameof(Berichtigung.InsertedDateDateOnly))]
    [MapperIgnoreSource(nameof(Berichtigung.ChangedDateDateOnly))]
    // Navigationen bleiben aussen vor: EF verwaltet sie über die Fremdschlüssel, und der Rückweg würde referenzierte Entities beim Create erneut einfügen.
    [MapperIgnoreTarget(nameof(TBerichtigung.BerAus))]
    [MapProperty(nameof(Berichtigung.Id), nameof(TBerichtigung.BerId))]
    [MapProperty(nameof(Berichtigung.AusId), nameof(TBerichtigung.BerAusId))]
    [MapProperty(nameof(Berichtigung.Eingereicht), nameof(TBerichtigung.BerEingereicht))]
    [MapProperty(nameof(Berichtigung.Publiziert), nameof(TBerichtigung.BerPubliziert))]
    [MapProperty(nameof(Berichtigung.VerzoegerungDurchAuftraggeber), nameof(TBerichtigung.BerVerzoegerungDurchAuftraggeber))]
    [MapProperty(nameof(Berichtigung.Bemerkungen), nameof(TBerichtigung.BerBemerkungen))]
    [MapProperty(nameof(Berichtigung.NaturalId), nameof(TBerichtigung.BerNaturalId))]
    [MapProperty(nameof(Berichtigung.Aktiv), nameof(TBerichtigung.BerAktiv))]
    [MapProperty(nameof(Berichtigung.InsertedDate), nameof(TBerichtigung.BerInsertedDate))]
    [MapProperty(nameof(Berichtigung.InsertedUser), nameof(TBerichtigung.BerInsertedUser))]
    [MapProperty(nameof(Berichtigung.ChangedDate), nameof(TBerichtigung.BerChangedDate))]
    [MapProperty(nameof(Berichtigung.ChangedUser), nameof(TBerichtigung.BerChangedUser))]
    public partial TBerichtigung ZuEntity(Berichtigung model);
}
