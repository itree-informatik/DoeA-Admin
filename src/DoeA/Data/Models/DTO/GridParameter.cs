// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

namespace DoeA.Data.Models.DTO;

/// <summary>Such-, Filter- und Paging-Parameter der Ausschreibungs-Suchliste.</summary>
public class GridParameter
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? SearchValue { get; set; }
    public int SkipItems { get; set; }
    public string? Resultset { get; set; }
    public bool Export { get; set; } = false;
    public int Jahr { get; set; }
    public DateTime? PublikationVon { get; set; }
    public DateTime? PublikationBis { get; set; }
    public decimal? BetragVon { get; set; }
    public decimal? BetragBis { get; set; }
    public Guid? OffertezustandId { get; set; }
    public Guid Id { get; set; }
}
