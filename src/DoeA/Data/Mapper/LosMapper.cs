// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Enums;
using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO;
using DoeA.Interfaces;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper;

/// <summary>Mapping zwischen TLos und dem DTO Los; der Zustand steht in der DB als Text.</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target, EnumMappingIgnoreCase = true)]
[UseStaticMapper(typeof(TextKonvertierung))]
public partial class LosMapper : IEntityMapper<Los, TLos>
{
    // Offerten und BKP-Nummern lädt der LosService nach.
    [MapperIgnoreTarget(nameof(Los.Offerten))]
    [MapperIgnoreTarget(nameof(Los.BkpNummern))]
    [MapProperty(nameof(TLos.LosId), nameof(Los.Id))]
    [MapProperty(nameof(TLos.LosAusId), nameof(Los.AusId))]
    [MapProperty(nameof(TLos.LosAbbruchdatum), nameof(Los.Abbruchdatum))]
    [MapProperty(nameof(TLos.LosEingereicht), nameof(Los.Eingereicht))]
    [MapProperty(nameof(TLos.LosVerzoegerungDurchAuftraggeber), nameof(Los.VerzoegerungDurchAuftraggeber))]
    [MapProperty(nameof(TLos.LosBezeichnung), nameof(Los.Bezeichnung))]
    [MapProperty(nameof(TLos.LosZustand), nameof(Los.Zustand), Use = nameof(ZuZustand))]
    [MapProperty(nameof(TLos.LosNaturalId), nameof(Los.NaturalId))]
    [MapProperty(nameof(TLos.LosAktiv), nameof(Los.Aktiv))]
    [MapProperty(nameof(TLos.LosInsertedDate), nameof(Los.InsertedDate))]
    [MapProperty(nameof(TLos.LosInsertedUser), nameof(Los.InsertedUser))]
    [MapProperty(nameof(TLos.LosChangedDate), nameof(Los.ChangedDate))]
    [MapProperty(nameof(TLos.LosChangedUser), nameof(Los.ChangedUser))]
    public partial Los ZuModel(TLos entity);

    [MapperRequiredMapping(RequiredMappingStrategy.Both)]
    [MapperIgnoreSource(nameof(Los.InsertedDateDateOnly))]
    [MapperIgnoreSource(nameof(Los.ChangedDateDateOnly))]
    // Navigationen bleiben aussen vor: EF verwaltet sie über die Fremdschlüssel, und der Rückweg würde referenzierte Entities beim Create erneut einfügen.
    [MapperIgnoreTarget(nameof(TLos.LosAus))]
    [MapperIgnoreTarget(nameof(TLos.TLosBkpNummer))]
    [MapperIgnoreTarget(nameof(TLos.TOfferte))]
    [MapperIgnoreSource(nameof(Los.Offerten))]
    [MapperIgnoreSource(nameof(Los.BkpNummern))]
    [MapProperty(nameof(Los.Id), nameof(TLos.LosId))]
    [MapProperty(nameof(Los.AusId), nameof(TLos.LosAusId))]
    [MapProperty(nameof(Los.Abbruchdatum), nameof(TLos.LosAbbruchdatum))]
    [MapProperty(nameof(Los.Eingereicht), nameof(TLos.LosEingereicht))]
    [MapProperty(nameof(Los.VerzoegerungDurchAuftraggeber), nameof(TLos.LosVerzoegerungDurchAuftraggeber))]
    [MapProperty(nameof(Los.Bezeichnung), nameof(TLos.LosBezeichnung))]
    [MapProperty(nameof(Los.Zustand), nameof(TLos.LosZustand))]
    [MapProperty(nameof(Los.NaturalId), nameof(TLos.LosNaturalId))]
    [MapProperty(nameof(Los.Aktiv), nameof(TLos.LosAktiv))]
    [MapProperty(nameof(Los.InsertedDate), nameof(TLos.LosInsertedDate))]
    [MapProperty(nameof(Los.InsertedUser), nameof(TLos.LosInsertedUser))]
    [MapProperty(nameof(Los.ChangedDate), nameof(TLos.LosChangedDate))]
    [MapProperty(nameof(Los.ChangedUser), nameof(TLos.LosChangedUser))]
    public partial TLos ZuEntity(Los model);

    // loszustand ist eine freie Textspalte: Ein unbekannter Wert aus den übernommenen Altdaten darf
    // nicht das Laden aller Lose scheitern lassen — Enum.Parse würde werfen.
    private EnumLosZustand ZuZustand(string zustand) =>
        Enum.TryParse<EnumLosZustand>(zustand, ignoreCase: true, out var wert) ? wert : EnumLosZustand.Offen;
}
