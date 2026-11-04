// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO.Einstellungen;
using DoeA.Interfaces;
using Riok.Mapperly.Abstractions;

namespace DoeA.Data.Mapper.Einstellungen;

/// <summary>Mapping von TLog zum DTO Log; kein Rückweg, weil t_log ausschliesslich Serilog schreibt.</summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class LogMapper
{
    public partial Log ZuModel(TLog entity);
}
