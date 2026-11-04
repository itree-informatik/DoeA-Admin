// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO.Stammdaten;
using DoeA.Interfaces;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper.Stammdaten;

/// <summary>Mapping zwischen TAmt und dem DTO Amt.</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(TextKonvertierung))]
public partial class AmtMapper : IEntityMapper<Amt, TAmt>
{
    private readonly DepartementMapper _departementMapper;

    public AmtMapper(DepartementMapper departementMapper)
    {
        _departementMapper = departementMapper;
    }

    [MapProperty(nameof(TAmt.AmtId), nameof(Amt.Id))]
    [MapProperty(nameof(TAmt.AmtDepId), nameof(Amt.DepId))]
    [MapProperty(nameof(TAmt.AmtBezeichnung), nameof(Amt.Bezeichnung))]
    [MapProperty(nameof(TAmt.AmtKurzbezeichnung), nameof(Amt.Kurzbezeichnung))]
    [MapProperty(nameof(TAmt.AmtDienststellennummer), nameof(Amt.Dienststellennummer))]
    [MapProperty(nameof(TAmt.AmtNaturalId), nameof(Amt.NaturalId))]
    [MapProperty(nameof(TAmt.AmtAktiv), nameof(Amt.Aktiv))]
    [MapProperty(nameof(TAmt.AmtInsertedDate), nameof(Amt.InsertedDate))]
    [MapProperty(nameof(TAmt.AmtInsertedUser), nameof(Amt.InsertedUser))]
    [MapProperty(nameof(TAmt.AmtChangedDate), nameof(Amt.ChangedDate))]
    [MapProperty(nameof(TAmt.AmtChangedUser), nameof(Amt.ChangedUser))]
    [MapProperty(nameof(TAmt.AmtDep), nameof(Amt.Departement), Use = nameof(ZuDepartement))]
    public partial Amt ZuModel(TAmt entity);

    [MapperRequiredMapping(RequiredMappingStrategy.Both)]
    [MapperIgnoreSource(nameof(Amt.InsertedDateDateOnly))]
    [MapperIgnoreSource(nameof(Amt.ChangedDateDateOnly))]
    // Navigationen bleiben aussen vor: EF verwaltet sie über die Fremdschlüssel, und der Rückweg würde referenzierte Entities beim Create erneut einfügen.
    [MapperIgnoreTarget(nameof(TAmt.AmtDep))]
    [MapperIgnoreTarget(nameof(TAmt.TAusschreibung))]
    [MapperIgnoreTarget(nameof(TAmt.TExportgruppeAmt))]
    [MapperIgnoreTarget(nameof(TAmt.TRessort))]
    [MapperIgnoreSource(nameof(Amt.Departement))]
    [MapProperty(nameof(Amt.Id), nameof(TAmt.AmtId))]
    [MapProperty(nameof(Amt.DepId), nameof(TAmt.AmtDepId))]
    [MapProperty(nameof(Amt.Bezeichnung), nameof(TAmt.AmtBezeichnung))]
    [MapProperty(nameof(Amt.Kurzbezeichnung), nameof(TAmt.AmtKurzbezeichnung))]
    [MapProperty(nameof(Amt.Dienststellennummer), nameof(TAmt.AmtDienststellennummer))]
    [MapProperty(nameof(Amt.NaturalId), nameof(TAmt.AmtNaturalId))]
    [MapProperty(nameof(Amt.Aktiv), nameof(TAmt.AmtAktiv))]
    [MapProperty(nameof(Amt.InsertedDate), nameof(TAmt.AmtInsertedDate))]
    [MapProperty(nameof(Amt.InsertedUser), nameof(TAmt.AmtInsertedUser))]
    [MapProperty(nameof(Amt.ChangedDate), nameof(TAmt.AmtChangedDate))]
    [MapProperty(nameof(Amt.ChangedUser), nameof(TAmt.AmtChangedUser))]
    public partial TAmt ZuEntity(Amt model);

    // EF liefert Navigationen ohne Include als null, obwohl sie non-nullable deklariert sind.
    private Departement? ZuDepartement(TDepartement? departement) => departement == null ? null : _departementMapper.ZuModel(departement);
}
