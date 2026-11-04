// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using ItreeMud.Models;

namespace DoeA.Data.Models.DTO;

/// <summary>Ergebnis einer Service-Operation: Einzelobjekt oder Liste plus Statuscode (GeneralConstants).</summary>
public class ServiceResult<TModel> where TModel : Basis
{
    public TModel? Item { get; set; }
    public List<TModel>? Items { get; set; }
    public string Result { get; set; } = string.Empty;
}
