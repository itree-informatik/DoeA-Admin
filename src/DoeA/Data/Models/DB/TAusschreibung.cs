// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoeA.Data.Models.DB;

/// <summary>Ausschreibung des BBL auf SIMAP mit Grunddaten, Fristen und Stammdaten-Zuordnungen; Wurzel für Lose, Berichtigungen, Sprachenmatrix, CPV-Nummern und Verfahrensschritte sowie Verweise auf Hauptprojekt, Vorankündigung und RFI.</summary>
[Table("T_Ausschreibung")]
public partial class TAusschreibung
{
    [Key]
    public Guid AusId { get; set; }

    // Amt, Leistungsart und Rolle sind bis zur Publikation optional (siehe AusschreibungValidator).
    public Guid? AusAmtId { get; set; }

    public Guid? AusLeaId { get; set; }

    public Guid? AusRiaId { get; set; }

    public Guid AusPraId { get; set; }

    public Guid AusVeaId { get; set; }

    public Guid? AusAusId { get; set; }

    public Guid? AusVorankuendigungId { get; set; }

    public Guid? AusRfiId { get; set; }

    public Guid? AusResId { get; set; }

    public Guid? AusSacDoeAId { get; set; }

    public Guid? AusSacRdId { get; set; }

    public Guid? AusSacBblId { get; set; }

    [StringLength(1000)]
    public string? AusBezeichnung { get; set; }

    [StringLength(255)]
    public string? AusNummerStr { get; set; }

    public int? AusNummer { get; set; }

    public int? AusLaufnummer { get; set; }

    public int AusJahr { get; set; }

    public int? AusAbschlussJahr { get; set; }

    [StringLength(255)]
    public string? AusExtern { get; set; }

    public bool AusPersonalverleih { get; set; }

    [StringLength(255)]
    public string? AusSimapNr { get; set; }

    public DateOnly? AusEingereicht { get; set; }
    public DateOnly? AusPublikation { get; set; }
    public bool AusVerzoegerungDurchAuftraggeber { get; set; }
    public DateOnly? AusEingereichtWiderruf { get; set; }
    public DateOnly? AusPublikationWiderruf { get; set; }
    public bool AusVerzoegerungDurchAuftraggeberWiderruf { get; set; }
    public int? AusPflichtenhefterPraequali { get; set; }

    public DateOnly? AusEingabefristTeilnahme { get; set; }

    public int? AusAnzahlTeilnehmer { get; set; }

    public DateOnly? AusVersandPh2 { get; set; }

    public int? AusPflichtenhefter { get; set; }

    public DateOnly? AusOffertenFrist { get; set; }

    [StringLength(255)]
    public string? AusStandort { get; set; }

    [StringLength(4000)]
    public string? AusBemerkung { get; set; }

    public bool AusAbgeschlossen { get; set; }

    public bool AusBeschaffungDezentral { get; set; }

    [StringLength(255)]
    public string? AusNaturalId { get; set; }

    public bool AusAktiv { get; set; }

    public long AusInsertedDate { get; set; }

    [StringLength(255)]
    public string AusInsertedUser { get; set; } = null!;

    public long AusChangedDate { get; set; }

    [StringLength(255)]
    public string AusChangedUser { get; set; } = null!;

    [ForeignKey("AusAmtId")]
    [InverseProperty("TAusschreibung")]
    public virtual TAmt? AusAmt { get; set; }

    [ForeignKey("AusAusId")]
    [InverseProperty("InverseAusAus")]
    public virtual TAusschreibung? AusAus { get; set; }

    [ForeignKey("AusVorankuendigungId")]
    [InverseProperty("InverseAusVorankuendigung")]
    public virtual TAusschreibung? AusVorankuendigung { get; set; }

    [ForeignKey("AusRfiId")]
    [InverseProperty("InverseAusRfi")]
    public virtual TAusschreibung? AusRfi { get; set; }

    [ForeignKey("AusLeaId")]
    [InverseProperty("TAusschreibung")]
    public virtual TLeistungsart? AusLea { get; set; }

    [ForeignKey("AusPraId")]
    [InverseProperty("TAusschreibung")]
    public virtual TProjektart AusPra { get; set; } = null!;

    [ForeignKey("AusResId")]
    [InverseProperty("TAusschreibung")]
    public virtual TRessort? AusRes { get; set; }

    [ForeignKey("AusRiaId")]
    [InverseProperty("TAusschreibung")]
    public virtual TRolleInAusschreibung? AusRia { get; set; }

    [ForeignKey("AusSacBblId")]
    [InverseProperty("TAusschreibungAusSacBbl")]
    public virtual TSachbearbeiter? AusSacBbl { get; set; }

    [ForeignKey("AusSacDoeAId")]
    [InverseProperty("TAusschreibungAusSacDoeA")]
    public virtual TSachbearbeiter? AusSacDoeA { get; set; }

    [ForeignKey("AusSacRdId")]
    [InverseProperty("TAusschreibungAusSacRd")]
    public virtual TSachbearbeiter? AusSacRd { get; set; }

    [ForeignKey("AusVeaId")]
    [InverseProperty("TAusschreibung")]
    public virtual TVerfahrensart AusVea { get; set; } = null!;

    [InverseProperty("AusAus")]
    public virtual ICollection<TAusschreibung> InverseAusAus { get; set; } = new List<TAusschreibung>();

    [InverseProperty("AusVorankuendigung")]
    public virtual ICollection<TAusschreibung> InverseAusVorankuendigung { get; set; } = new List<TAusschreibung>();

    [InverseProperty("AusRfi")]
    public virtual ICollection<TAusschreibung> InverseAusRfi { get; set; } = new List<TAusschreibung>();

    [InverseProperty("AcnAus")]
    public virtual ICollection<TAusschreibungCpvNummer> TAusschreibungCpvNummer { get; set; } = new List<TAusschreibungCpvNummer>();

    [InverseProperty("AspAus")]
    public virtual ICollection<TAusschreibungSprache> TAusschreibungSprache { get; set; } = new List<TAusschreibungSprache>();

    [InverseProperty("AvsAus")]
    public virtual ICollection<TAusschreibungVerfahrensschritt> TAusschreibungVerfahrensschritt { get; set; } = new List<TAusschreibungVerfahrensschritt>();

    [InverseProperty("BerAus")]
    public virtual ICollection<TBerichtigung> TBerichtigung { get; set; } = new List<TBerichtigung>();

    [InverseProperty("LosAus")]
    public virtual ICollection<TLos> TLos { get; set; } = new List<TLos>();
}
