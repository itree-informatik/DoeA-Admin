// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO.Exportgruppen;
using DoeA.Interfaces;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper.Exportgruppen;

/// <summary>Mapping zwischen TExportgruppe und dem DTO Exportgruppe; die Id-Listen der Filter befüllt der ExportgruppeService.</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(TextKonvertierung))]
public partial class ExportgruppeMapper : IEntityMapper<Exportgruppe, TExportgruppe>
{
    // Die Id-Listen der n:m-Filter befüllt und gleicht ausschliesslich der ExportgruppeService ab.
    [MapperIgnoreTarget(nameof(Exportgruppe.AmtIds))]
    [MapperIgnoreTarget(nameof(Exportgruppe.KantonIds))]
    [MapperIgnoreTarget(nameof(Exportgruppe.LeistungsartIds))]
    [MapperIgnoreTarget(nameof(Exportgruppe.ProjektartIds))]
    [MapperIgnoreTarget(nameof(Exportgruppe.RessortIds))]
    [MapperIgnoreTarget(nameof(Exportgruppe.RolleInAusschreibungIds))]
    [MapperIgnoreTarget(nameof(Exportgruppe.SachbearbeiterDoeAIds))]
    [MapperIgnoreTarget(nameof(Exportgruppe.SachbearbeiterRdIds))]
    [MapperIgnoreTarget(nameof(Exportgruppe.SachbearbeiterBblIds))]
    [MapperIgnoreTarget(nameof(Exportgruppe.VerfahrensartIds))]
    [MapProperty(nameof(TExportgruppe.ExpId), nameof(Exportgruppe.Id))]
    [MapProperty(nameof(TExportgruppe.ExpName), nameof(Exportgruppe.Name))]
    [MapProperty(nameof(TExportgruppe.ExpJahr), nameof(Exportgruppe.Jahr))]
    [MapProperty(nameof(TExportgruppe.ExpAbschlussJahr), nameof(Exportgruppe.AbschlussJahr))]
    [MapProperty(nameof(TExportgruppe.ExpPersonalverleih), nameof(Exportgruppe.Personalverleih))]
    [MapProperty(nameof(TExportgruppe.ExpAbgeschlossen), nameof(Exportgruppe.Abgeschlossen))]
    [MapProperty(nameof(TExportgruppe.ExpBeschaffungDezentral), nameof(Exportgruppe.BeschaffungDezentral))]
    [MapProperty(nameof(TExportgruppe.ExpPublikationVon), nameof(Exportgruppe.PublikationVon))]
    [MapProperty(nameof(TExportgruppe.ExpPublikationBis), nameof(Exportgruppe.PublikationBis))]
    [MapProperty(nameof(TExportgruppe.ExpSpalten), nameof(Exportgruppe.Spalten))]
    [MapProperty(nameof(TExportgruppe.ExpNaturalId), nameof(Exportgruppe.NaturalId))]
    [MapProperty(nameof(TExportgruppe.ExpAktiv), nameof(Exportgruppe.Aktiv))]
    [MapProperty(nameof(TExportgruppe.ExpInsertedDate), nameof(Exportgruppe.InsertedDate))]
    [MapProperty(nameof(TExportgruppe.ExpInsertedUser), nameof(Exportgruppe.InsertedUser))]
    [MapProperty(nameof(TExportgruppe.ExpChangedDate), nameof(Exportgruppe.ChangedDate))]
    [MapProperty(nameof(TExportgruppe.ExpChangedUser), nameof(Exportgruppe.ChangedUser))]
    public partial Exportgruppe ZuModel(TExportgruppe entity);

    [MapperRequiredMapping(RequiredMappingStrategy.Both)]
    [MapperIgnoreSource(nameof(Exportgruppe.InsertedDateDateOnly))]
    [MapperIgnoreSource(nameof(Exportgruppe.ChangedDateDateOnly))]
    // Navigationen bleiben aussen vor: EF verwaltet sie über die Fremdschlüssel, und der Rückweg würde referenzierte Entities beim Create erneut einfügen.
    [MapperIgnoreTarget(nameof(TExportgruppe.TExportgruppeAmt))]
    [MapperIgnoreTarget(nameof(TExportgruppe.TExportgruppeKanton))]
    [MapperIgnoreTarget(nameof(TExportgruppe.TExportgruppeLeistungsart))]
    [MapperIgnoreTarget(nameof(TExportgruppe.TExportgruppeProjektart))]
    [MapperIgnoreTarget(nameof(TExportgruppe.TExportgruppeRessort))]
    [MapperIgnoreTarget(nameof(TExportgruppe.TExportgruppeRolleInAusschreibung))]
    [MapperIgnoreTarget(nameof(TExportgruppe.TExportgruppeSachbearbeiter))]
    [MapperIgnoreTarget(nameof(TExportgruppe.TExportgruppeVerfahrensart))]
    [MapperIgnoreSource(nameof(Exportgruppe.AmtIds))]
    [MapperIgnoreSource(nameof(Exportgruppe.KantonIds))]
    [MapperIgnoreSource(nameof(Exportgruppe.LeistungsartIds))]
    [MapperIgnoreSource(nameof(Exportgruppe.ProjektartIds))]
    [MapperIgnoreSource(nameof(Exportgruppe.RessortIds))]
    [MapperIgnoreSource(nameof(Exportgruppe.RolleInAusschreibungIds))]
    [MapperIgnoreSource(nameof(Exportgruppe.SachbearbeiterDoeAIds))]
    [MapperIgnoreSource(nameof(Exportgruppe.SachbearbeiterRdIds))]
    [MapperIgnoreSource(nameof(Exportgruppe.SachbearbeiterBblIds))]
    [MapperIgnoreSource(nameof(Exportgruppe.VerfahrensartIds))]
    [MapProperty(nameof(Exportgruppe.Id), nameof(TExportgruppe.ExpId))]
    [MapProperty(nameof(Exportgruppe.Name), nameof(TExportgruppe.ExpName))]
    [MapProperty(nameof(Exportgruppe.Jahr), nameof(TExportgruppe.ExpJahr))]
    [MapProperty(nameof(Exportgruppe.AbschlussJahr), nameof(TExportgruppe.ExpAbschlussJahr))]
    [MapProperty(nameof(Exportgruppe.Personalverleih), nameof(TExportgruppe.ExpPersonalverleih))]
    [MapProperty(nameof(Exportgruppe.Abgeschlossen), nameof(TExportgruppe.ExpAbgeschlossen))]
    [MapProperty(nameof(Exportgruppe.BeschaffungDezentral), nameof(TExportgruppe.ExpBeschaffungDezentral))]
    [MapProperty(nameof(Exportgruppe.PublikationVon), nameof(TExportgruppe.ExpPublikationVon))]
    [MapProperty(nameof(Exportgruppe.PublikationBis), nameof(TExportgruppe.ExpPublikationBis))]
    [MapProperty(nameof(Exportgruppe.Spalten), nameof(TExportgruppe.ExpSpalten))]
    [MapProperty(nameof(Exportgruppe.NaturalId), nameof(TExportgruppe.ExpNaturalId))]
    [MapProperty(nameof(Exportgruppe.Aktiv), nameof(TExportgruppe.ExpAktiv))]
    [MapProperty(nameof(Exportgruppe.InsertedDate), nameof(TExportgruppe.ExpInsertedDate))]
    [MapProperty(nameof(Exportgruppe.InsertedUser), nameof(TExportgruppe.ExpInsertedUser))]
    [MapProperty(nameof(Exportgruppe.ChangedDate), nameof(TExportgruppe.ExpChangedDate))]
    [MapProperty(nameof(Exportgruppe.ChangedUser), nameof(TExportgruppe.ExpChangedUser))]
    public partial TExportgruppe ZuEntity(Exportgruppe model);
}
