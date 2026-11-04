// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DTO.Stammdaten;
using ItreeMud.Models;

namespace DoeA.Data.Models.DTO;

/// <summary>Ausschreibung mit Grunddaten, Fristen, Sprachen, CPV-Nummern und Berichtigungen — abgebildet auf dbo.t_ausschreibung (Präfix "Aus").</summary>
public class Ausschreibung : Basis
{

    public string Bezeichnung { get; set; } = string.Empty;
    public Projektart Projektart { get; set; } = new();
    public Ausschreibung? Hauptprojekt { get; set; }
    public Ausschreibung? Vorankuendigung { get; set; }
    public Ausschreibung? Rfi { get; set; }
    // Eingehende Verweise: andere Ausschreibungen, die diese hier als Vorankündigung oder RFI referenzieren.
    // Rein lesend (abgeleitet), wird nicht persistiert.
    public List<Ausschreibung> ReferenziertVon { get; set; } = new();
    public int? Jahr { get; set; }
    public int Nummer { get; set; }
    public string NummerStr { get; set; } = string.Empty;
    public Verfahrensart Verfahrensart { get; set; } = new();
    public RolleInAusschreibung RolleInAusschreibung { get; set; } = new();
    public Leistungsart Leistungsart { get; set; } = new();
    public int? AbschlussJahr { get; set; }
    public Guid? AmtId { get; set; }
    public string? SimapNr { get; set; }
    public Guid? ResId { get; set; }
    public Guid? LeaId { get; set; }

    public Guid? SacBblId { get; set; }
    public Guid? SacRdId { get; set; }
    public Guid? SacDoeAId { get; set; }
    public Guid? RiaId { get; set; }
    public string Standort { get; set; } = string.Empty;
    public bool Personalverleih { get; set; }
    public bool BeschaffungDezentral { get; set; }
    public string Extern { get; set; } = string.Empty;
    public List<AusschreibungSprache> AusschreibungSprache { get; set; } = new();
    public DateOnly? EingabefristTeilnahme { get; set; }
    public DateOnly? OffertenFrist { get; set; }
    public DateOnly? Eingereicht { get; set; }
    public DateOnly? Publikation { get; set; }
    public bool VerzoegerungDurchAuftraggeber { get; set; }
    public DateOnly? EingereichtWiderruf { get; set; }
    public DateOnly? PublikationWiderruf { get; set; }
    public bool VerzoegerungDurchAuftraggeberWiderruf { get; set; }
    public List<Berichtigung> Berichtigungen { get; set; } = new();
    public DateOnly? VersandPh2 { get; set; }
    public int PflichtenhefterPraequali { get; set; }
    public int Pflichtenhefter { get; set; }
    public int AnzahlTeilnehmer { get; set; }
    public string Bemerkung { get; set; } = string.Empty;
    public Guid VeaId { get; set; }
    public Guid PraId { get; set; }
    public List<AusschreibungCpvNummer> CpvNummern { get; set; } = new();
}
