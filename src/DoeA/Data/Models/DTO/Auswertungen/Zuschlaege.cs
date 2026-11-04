// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using ItreeMud.Extensions;
namespace DoeA.Data.Models.DTO.Auswertungen;

/// <summary>Zeile der Auswertung «Zuschläge» (Zuschläge, Betrag, Offerten je Gruppe) mit den Zeilenarrays für Word- und Excel-Export.</summary>
public class Zuschlaege
{
    public string Bezeichnung { get; set; } = string.Empty;
    public int Zuschlag { get; set; }
    public decimal Betrag { get; set; }
    public int Offerten { get; set; }
    public int TotalZuschlaege { get; set;}
    public int TotalOfferten { get; set; }
    public decimal TotalBetrag { get; set; }

    public string?[] HeaderMitBetrag => new string?[8] { "H", null, null, null, null, null, null, null };
    public string?[] HeaderOhneBetrag => new string?[7] { "H", null, null, null, null, null, null };

    public string[] DetailMitBetrag => new string[8]
    {
        "D",
        Bezeichnung,
        Zuschlag.FormatNumber(),
        Betrag.FormatNumber(),
        ZuschlagProzent,
        Offerten.FormatNumber(),
        OffertenProzent,
        ErfolgreichProzent
    };

    public string[] DetailOhneBetrag => new string[7]
    {
        "D",
        Bezeichnung,
        Zuschlag.FormatNumber(),
        ZuschlagProzent,
        Offerten.FormatNumber(),
        OffertenProzent,
        ErfolgreichProzent
    };

    public string?[] TotalMitBetrag => new string?[8]
    {
        "T",
        "Total",
        TotalZuschlaege.FormatNumber(),
        TotalBetrag.FormatNumber(),
        "100%",
        TotalOfferten.FormatNumber(),
        "100%",
        null
    };

    public string?[] TotalOhneBetrag => new string?[7]
    {
        "T",
        "Total",
        TotalZuschlaege.FormatNumber(),
        "100%",
        TotalOfferten.FormatNumber(),
        "100%",
        null
    };

    public double ExcelBetrag => (double)Betrag;

    /// <summary>Anteil der Zeile am Total aller Zuschläge; "-" ohne Zuschläge im Total.</summary>
    public string ZuschlagProzent => Prozent(Zuschlag, TotalZuschlaege);

    /// <summary>Anteil der Zeile am Total aller Offerten; "-" ohne Offerten im Total.</summary>
    public string OffertenProzent => Prozent(Offerten, TotalOfferten);

    /// <summary>Erfolgsquote der Zeile (Zuschläge pro Offerte); "-" ohne Offerten in der Zeile.</summary>
    public string ErfolgreichProzent => Prozent(Zuschlag, Offerten);

    // Nenner 0 ist durch die heutige Datenherkunft ausgeschlossen (Zeilen entstehen nur aus Zuschlägen),
    // der Guard schützt künftige Erweiterungen (z.B. Zeilen ohne Zuschlag) vor der decimal-DivideByZeroException.
    private static string Prozent(int anteil, int total)
        => total == 0 ? "-" : $"{Math.Round((decimal)anteil / total * 100, 2).FormatNumber()}%";
}
