// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO.Stammdaten;
using DoeA.Interfaces;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper.Stammdaten;

/// <summary>Mapping zwischen TRessort und dem DTO Ressort.</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(TextKonvertierung))]
public partial class RessortMapper : IEntityMapper<Ressort, TRessort>
{
    private readonly AmtMapper _amtMapper;

    public RessortMapper(AmtMapper amtMapper)
    {
        _amtMapper = amtMapper;
    }

    [MapProperty(nameof(TRessort.ResId), nameof(Ressort.Id))]
    [MapProperty(nameof(TRessort.ResAmtId), nameof(Ressort.AmtId))]
    [MapProperty(nameof(TRessort.ResBezeichnung), nameof(Ressort.Bezeichnung))]
    [MapProperty(nameof(TRessort.ResKurzbezeichnung), nameof(Ressort.Kurzbezeichnung))]
    [MapProperty(nameof(TRessort.ResNaturalId), nameof(Ressort.NaturalId))]
    [MapProperty(nameof(TRessort.ResAktiv), nameof(Ressort.Aktiv))]
    [MapProperty(nameof(TRessort.ResInsertedDate), nameof(Ressort.InsertedDate))]
    [MapProperty(nameof(TRessort.ResInsertedUser), nameof(Ressort.InsertedUser))]
    [MapProperty(nameof(TRessort.ResChangedDate), nameof(Ressort.ChangedDate))]
    [MapProperty(nameof(TRessort.ResChangedUser), nameof(Ressort.ChangedUser))]
    [MapProperty(nameof(TRessort.ResAmt), nameof(Ressort.Amt), Use = nameof(ZuAmt))]
    public partial Ressort ZuModel(TRessort entity);

    [MapperRequiredMapping(RequiredMappingStrategy.Both)]
    [MapperIgnoreSource(nameof(Ressort.InsertedDateDateOnly))]
    [MapperIgnoreSource(nameof(Ressort.ChangedDateDateOnly))]
    // Navigationen bleiben aussen vor: EF verwaltet sie über die Fremdschlüssel, und der Rückweg würde referenzierte Entities beim Create erneut einfügen.
    [MapperIgnoreTarget(nameof(TRessort.ResAmt))]
    [MapperIgnoreTarget(nameof(TRessort.TAusschreibung))]
    [MapperIgnoreTarget(nameof(TRessort.TExportgruppeRessort))]
    [MapperIgnoreSource(nameof(Ressort.Amt))]
    [MapProperty(nameof(Ressort.Id), nameof(TRessort.ResId))]
    [MapProperty(nameof(Ressort.AmtId), nameof(TRessort.ResAmtId))]
    [MapProperty(nameof(Ressort.Bezeichnung), nameof(TRessort.ResBezeichnung))]
    [MapProperty(nameof(Ressort.Kurzbezeichnung), nameof(TRessort.ResKurzbezeichnung))]
    [MapProperty(nameof(Ressort.NaturalId), nameof(TRessort.ResNaturalId))]
    [MapProperty(nameof(Ressort.Aktiv), nameof(TRessort.ResAktiv))]
    [MapProperty(nameof(Ressort.InsertedDate), nameof(TRessort.ResInsertedDate))]
    [MapProperty(nameof(Ressort.InsertedUser), nameof(TRessort.ResInsertedUser))]
    [MapProperty(nameof(Ressort.ChangedDate), nameof(TRessort.ResChangedDate))]
    [MapProperty(nameof(Ressort.ChangedUser), nameof(TRessort.ResChangedUser))]
    public partial TRessort ZuEntity(Ressort model);

    // EF liefert Navigationen ohne Include als null, obwohl sie non-nullable deklariert sind.
    private Amt? ZuAmt(TAmt? amt) => amt == null ? null : _amtMapper.ZuModel(amt);
}
