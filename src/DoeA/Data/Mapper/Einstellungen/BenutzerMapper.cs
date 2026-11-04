// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO.Einstellungen;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper.Einstellungen;

/// <summary>Mapping von TUser auf das DTO Benutzer (nur lesend, geschrieben wird über den BenutzerService).</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class BenutzerMapper
{
    [MapProperty(nameof(TUser.UsrId), nameof(Benutzer.Id))]
    [MapProperty(nameof(TUser.UsrExtId), nameof(Benutzer.ExtId))]
    [MapProperty(nameof(TUser.UsrVorname), nameof(Benutzer.Vorname))]
    [MapProperty(nameof(TUser.UsrNachname), nameof(Benutzer.Nachname))]
    [MapProperty(nameof(TUser.UsrEMail), nameof(Benutzer.EMail))]
    [MapProperty(nameof(TUser.UsrSprache), nameof(Benutzer.Sprache))]
    [MapProperty(nameof(TUser.UsrLastLogin), nameof(Benutzer.LastLogin))]
    [MapProperty(nameof(TUser.UsrAktiv), nameof(Benutzer.Aktiv))]
    public partial Benutzer ZuModel(TUser entity);
}
