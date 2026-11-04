// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using NpgsqlTypes;

namespace DataMigration;

/// <summary>
/// Eine Zielspalte mit ihrem PostgreSQL-Typ (für den Binary-COPY-Writer) und optionaler
/// Wert-Konvertierung (z. B. datetime -> .NET-Ticks).
/// </summary>
public record Col(string Name, NpgsqlDbType Type, Func<object, object>? Convert = null);

/// <summary>
/// Eine Tabellen-Migration: Zieltabelle (PG), Zielspalten und das Quell-SELECT (SQL Server).
/// Das SELECT liefert die Spalten in derselben Reihenfolge wie <see cref="Columns"/>.
/// Transformationen: datetime->Ticks in C# (siehe <see cref="Col.Convert"/>), restliche
/// (Casts, Sprach-Culture-Mapping, Renames) im SELECT.
/// </summary>
public record TableMigration(string TargetTable, IReadOnlyList<Col> Columns, string SourceSelect);

public static class Migrations
{
    // Kurzschreibweisen für die Spaltentypen
    private static Col Uuid(string n) => new(n, NpgsqlDbType.Uuid);
    private static Col Str(string n) => new(n, NpgsqlDbType.Varchar);
    private static Col Int(string n) => new(n, NpgsqlDbType.Integer);
    // datetime aus der Quelle -> .NET-Ticks (bigint). Ersetzt die in der Quell-DB fehlende Funktion F_SqlDateTimeToTicks.
    private static Col Ticks(string n) => new(n, NpgsqlDbType.Bigint, v => ((DateTime)v).Ticks);
    private static Col Bool(string n) => new(n, NpgsqlDbType.Boolean);
    private static Col Date(string n) => new(n, NpgsqlDbType.Date);
    private static Col Num(string n) => new(n, NpgsqlDbType.Numeric);

    /// <summary>Zieltabellen in FK-sicherer Reihenfolge zum Leeren (Kinder zuerst).</summary>
    public static readonly string[] DeleteOrder =
    {
        "dbo.t_offerteoffertezustand",
        "dbo.t_offerte",
        "dbo.t_losbkpnummer",
        "dbo.t_los",
        "dbo.t_ausschreibungverfahrensschritt",
        "dbo.t_ausschreibungsprache",
        "dbo.t_ausschreibungcpvnummer",
        "dbo.t_ausschreibung",
        "dbo.t_ressort",
        "dbo.t_amt",
        "dbo.t_departement",
        "dbo.t_anbieter",
        "dbo.t_ortschaft",
        "dbo.t_kanton",
        "dbo.t_land",
        "dbo.t_dokumentart",
        "dbo.t_leistungsart",
        "dbo.t_projektart",
        "dbo.t_offertezustand",
        "dbo.t_sachbearbeiter",
        "dbo.t_organisationszugehoerigkeit",
        "dbo.t_rolleinausschreibung",
        "dbo.t_verfahrensart",
        "dbo.t_verfahrensschritt",
        "dbo.t_cpvnummer",
        "dbo.t_bkpnummer",
        "dbo.t_sprache",
    };

    /// <summary>Tabellen-Migrationen in FK-sicherer Insert-Reihenfolge (Eltern zuerst).</summary>
    public static readonly TableMigration[] All =
    {
        new("dbo.t_bkpnummer",
            new[] { Uuid("bkpid"), Str("bkpnummerstr"), Str("bkpbezeichnung"), Int("bkpnummer"),
                    Str("bkpnaturalid"), Bool("bkpaktiv"), Ticks("bkpinserteddate"), Str("bkpinserteduser"),
                    Ticks("bkpchangeddate"), Str("bkpchangeduser") },
            @"select BkpId, BkpNummerStr, BkpBezeichnung, BkpNummer, BkpNaturalId, BkpAktiv,
                     BkpInsertedDate, BkpInsertedUser,
                     BkpChangedDate, BkpChangedUser
              from dbo.T_BkpNummer"),

        new("dbo.t_cpvnummer",
            new[] { Uuid("cpvid"), Str("cpvnummerstr"), Str("cpvbezeichnung"), Int("cpvnummer"),
                    Str("cpvnaturalid"), Bool("cpvaktiv"), Ticks("cpvinserteddate"), Str("cpvinserteduser"),
                    Ticks("cpvchangeddate"), Str("cpvchangeduser") },
            @"select CpvId, CpvNummerStr, CpvBezeichnung, CpvNummer, CpvNaturalId, CpvAktiv,
                     CpvInsertedDate, CpvInsertedUser,
                     CpvChangedDate, CpvChangedUser
              from dbo.T_CpvNummer"),

        new("dbo.t_dokumentart",
            new[] { Uuid("doaid"), Str("doabezeichnung"), Str("doanaturalid"), Int("doasortierung"), Bool("doaaktiv"),
                    Ticks("doainserteddate"), Str("doainserteduser"), Ticks("doachangeddate"), Str("doachangeduser") },
            @"select DoaId, DoaBezeichnung, DoaNaturalId,
                     case DoaNaturalId when 'Publikation' then 1
                                       when 'Unterlagen' then 2
                                       when 'Offerten' then 3
                                       else 99 end,
                     DoaAktiv,
                     DoaInsertedDate, DoaInsertedUser,
                     DoaChangedDate, DoaChangedUser
              from dbo.T_DokArt"),

        new("dbo.t_projektart",
            new[] { Uuid("praid"), Str("prabezeichnung"), Str("prakurzbezeichnung"), Str("pranaturalid"),
                    Bool("praaktiv"), Ticks("prainserteddate"), Str("prainserteduser"), Ticks("prachangeddate"),
                    Str("prachangeduser") },
            @"select PraId, PraBezeichnung, PraKurzbezeichnung, PraNaturalId, PraAktiv,
                     PraInsertedDate, PraInsertedUser,
                     PraChangedDate, PraChangedUser
              from dbo.T_Projektart"),

        new("dbo.t_leistungsart",
            new[] { Uuid("leaid"), Uuid("leapraid"), Str("leabezeichnung"), Str("leakurzbezeichnung"),
                    Str("leanaturalid"), Bool("leaaktiv"), Ticks("leainserteddate"), Str("leainserteduser"),
                    Ticks("leachangeddate"), Str("leachangeduser") },
            @"select LeaId, LeaPraId, LeaBezeichnung, LeaKurzbezeichnung, LeaNaturalId, LeaAktiv,
                     LeaInsertedDate, LeaInsertedUser,
                     LeaChangedDate, LeaChangedUser
              from dbo.T_Leistungsart"),

        new("dbo.t_departement",
            new[] { Uuid("depid"), Str("depbezeichnung"), Str("depkurzbezeichnung"), Str("depnaturalid"),
                    Bool("depaktiv"), Ticks("depinserteddate"), Str("depinserteduser"), Ticks("depchangeddate"),
                    Str("depchangeduser") },
            @"select DepId, DepBezeichnung, DepKurzbezeichnung, DepNaturalId, DepAktiv,
                     DepInsertedDate, DepInsertedUser,
                     DepChangedDate, DepChangedUser
              from dbo.T_Departement"),

        new("dbo.t_amt",
            new[] { Uuid("amtid"), Uuid("amtdepid"), Str("amtbezeichnung"), Str("amtkurzbezeichnung"),
                    Str("amtdienststellennummer"), Str("amtnaturalid"), Bool("amtaktiv"), Ticks("amtinserteddate"),
                    Str("amtinserteduser"), Ticks("amtchangeddate"), Str("amtchangeduser") },
            @"select AmtId, AmtDepId, AmtBezeichnung, AmtKurzbezeichnung, AmtDienststellennummer, AmtNaturalId, AmtAktiv,
                     AmtInsertedDate, AmtInsertedUser,
                     AmtChangedDate, AmtChangedUser
              from dbo.T_Amt"),

        new("dbo.t_ressort",
            new[] { Uuid("resid"), Uuid("resamtid"), Str("resbezeichnung"), Str("reskurzbezeichnung"),
                    Str("resnaturalid"), Bool("resaktiv"), Ticks("resinserteddate"), Str("resinserteduser"),
                    Ticks("reschangeddate"), Str("reschangeduser") },
            @"select ResId, ResAmtId, ResBezeichnung, ResKurzbezeichnung, ResNaturalId, ResAktiv,
                     ResInsertedDate, ResInsertedUser,
                     ResChangedDate, ResChangedUser
              from dbo.T_Ressort"),

        new("dbo.t_land",
            new[] { Uuid("lanid"), Str("lanbezeichnung"), Str("lankurzbezeichnung"), Str("lannaturalid"),
                    Bool("lanaktiv"), Ticks("laninserteddate"), Str("laninserteduser"), Ticks("lanchangeddate"),
                    Str("lanchangeduser") },
            @"select LanId, LanBezeichnung, LanKurzbezeichnung, LanNaturalId, LanAktiv,
                     LanInsertedDate, LanInsertedUser,
                     LanChangedDate, LanChangedUser
              from dbo.T_Land"),

        new("dbo.t_sprache",
            new[] { Uuid("sprid"), Str("sprculture"), Str("sprbezeichnung"), Str("sprnaturalid"),
                    Int("sprsortierung"), Bool("spraktiv"), Ticks("sprinserteddate"), Str("sprinserteduser"),
                    Ticks("sprchangeddate"), Str("sprchangeduser") },
            @"select SprId,
                     case when SprNaturalId = 'DE' then 'de-CH'
                          when SprNaturalId = 'FR' then 'fr-CH'
                          when SprNaturalId = 'IT' then 'it-CH'
                          when SprNaturalId = 'EN' then 'en-US' end,
                     SprBezeichnung, SprNaturalId,
                     case SprNaturalId when 'DE' then 1
                                       when 'FR' then 2
                                       when 'IT' then 3
                                       when 'EN' then 4
                                       else 99 end,
                     SprAktiv,
                     SprInsertedDate, SprInsertedUser,
                     SprChangedDate, SprChangedUser
              from dbo.T_Sprache"),

        new("dbo.t_kanton",
            new[] { Uuid("kanid"), Uuid("kansprid"), Str("kanbezeichnung"), Str("kankurzbezeichnung"),
                    Str("kannaturalid"), Bool("kanaktiv"), Ticks("kaninserteddate"), Str("kaninserteduser"),
                    Ticks("kanchangeddate"), Str("kanchangeduser") },
            @"select KanId, KanSprId, KanBezeichnung, KanKurzBezeichnung, KanNaturalId, KanAktiv,
                     KanInsertedDate, KanInsertedUser,
                     KanChangedDate, KanChangedUser
              from dbo.T_Kanton"),

        new("dbo.t_ortschaft",
            new[] { Uuid("ortid"), Uuid("ortlanid"), Uuid("ortkanid"), Str("ortort"), Str("ortplz"),
                    Str("ortnaturalid"), Bool("ortaktiv"), Ticks("ortinserteddate"), Str("ortinserteduser"),
                    Ticks("ortchangeddate"), Str("ortchangeduser") },
            @"select OrtId, OrtLanId, OrtKanId, OrtOrt, OrtPlz, OrtNaturalId, OrtAktiv,
                     OrtInsertedDate, OrtInsertedUser,
                     OrtChangedDate, OrtChangedUser
              from dbo.T_Ortschaft"),

        new("dbo.t_anbieter",
            new[] { Uuid("anbid"), Uuid("anbortid"), Str("anbname"), Str("anbadresse"), Str("anbadresse2"),
                    Str("anbadresse3"), Str("anbnaturalid"), Bool("anbaktiv"), Ticks("anbinserteddate"),
                    Str("anbinserteduser"), Ticks("anbchangeddate"), Str("anbchangeduser") },
            @"select AnbId, AnbOrtId, AnbName, AnbAdresse, AnbAdresse2, AnbAdresse3, AnbNaturalId, AnbAktiv,
                     AnbInsertedDate, AnbInsertedUser,
                     AnbChangedDate, AnbChangedUser
              from dbo.T_Anbieter"),

        new("dbo.t_offertezustand",
            new[] { Uuid("ofzid"), Str("ofzbezeichnung"), Str("ofzkurzbezeichnung"), Bool("ofzschlusszustand"),
                    Bool("ofzpublikation"), Str("ofznaturalid"), Bool("ofzaktiv"), Ticks("ofzinserteddate"),
                    Str("ofzinserteduser"), Ticks("ofzchangeddate"), Str("ofzchangeduser") },
            // Das Altsystem kennt kein Publikations-Flag: Auf Zuschlag und Widerruf folgt eine Publikation.
            @"select OfzId, OfzBezeichnung, OfzKurzbezeichnung, OfzSchlusszustand,
                     cast(case when OfzNaturalId in ('zuschlag', 'widerrufen') then 1 else 0 end as bit),
                     OfzNaturalId, OfzAktiv,
                     OfzInsertedDate, OfzInsertedUser,
                     OfzChangedDate, OfzChangedUser
              from dbo.T_Offertezustand
              where ofzNaturalid != 'aufgehoben'"),

        new("dbo.t_organisationszugehoerigkeit",
            new[] { Uuid("orgid"), Str("orgbezeichnung"), Str("orgkurzbezeichnung"), Str("orgnaturalid"),
                    Bool("orgaktiv"), Ticks("orginserteddate"), Str("orginserteduser"), Ticks("orgchangeddate"),
                    Str("orgchangeduser") },
            @"select OrgId, OrgBezeichnung, OrgKurzbezeichnung, OrgNaturalId, OrgAktiv,
                     OrgInsertedDate, OrgInsertedUser,
                     OrgChangedDate, OrgChangedUser
              from dbo.T_Organisationszugehoerigkeit"),

        new("dbo.t_rolleinausschreibung",
            new[] { Uuid("riaid"), Str("riabezeichnung"), Str("riakurzbezeichnung"), Str("rianaturalid"),
                    Bool("riaaktiv"), Ticks("riainserteddate"), Str("riainserteduser"), Ticks("riachangeddate"),
                    Str("riachangeduser") },
            @"select RolId,
                     case RolNaturalId
                         when 'Vergabestelle' then 'Logistik zentral'
                         when 'Meldestelle' then N'Ämter dezentral'
                         else RolBezeichnung
                     end as RolBezeichnung,
                     case RolNaturalId
                         when 'Vergabestelle' then 'Z'
                         when 'Meldestelle' then 'D'
                         else RolKurzbezeichnung
                     end as RolKurzbezeichnung,
                     case RolNaturalId
                         when 'Vergabestelle' then 'LogistikZentral'
                         when 'Meldestelle' then 'AemterDezentral'
                         else RolNaturalId
                     end as RolNaturalId, RolAktiv,
                     RolInsertedDate, RolInsertedUser,
                     RolChangedDate, RolChangedUser
              from dbo.T_Rolle"),

        new("dbo.t_sachbearbeiter",
            new[] { Uuid("sacid"), Uuid("sacorgid"), Str("sacnachname"), Str("sacvorname"),
                    Str("sackurzbezeichnung"), Str("sacnaturalid"), Bool("sacaktiv"), Ticks("sacinserteddate"),
                    Str("sacinserteduser"), Ticks("sacchangeddate"), Str("sacchangeduser") },
            @"select SacId, SacOrgId, SacNachname, SacVorname, SacKurzbezeichnung, SacNaturalId, SacAktiv,
                     SacInsertedDate, SacInsertedUser,
                     SacChangedDate, SacChangedUser
              from dbo.T_Sachbearbeiter"),

        new("dbo.t_verfahrensart",
            new[] { Uuid("veaid"), Str("veabezeichnung"), Str("veakurzbezeichnung"), Int("veasortierung"),
                    Str("veanaturalid"), Bool("veaaktiv"), Ticks("veainserteddate"), Str("veainserteduser"),
                    Ticks("veachangeddate"), Str("veachangeduser") },
            @"select VeaId,
                     case VeaNaturalId
                         when N'Vorankündigung' then N'Vorankündigung offen'
                         else VeaBezeichnung
                     end as VeaBezeichnung,
                     VeaKurzbezeichnung, VeaSortierung,
                     case VeaNaturalId
                         when N'Vorankündigung' then N'VorankündigungOffen'
                         else VeaNaturalId
                     end as VeaNaturalId,
                     VeaAktiv,
                     VeaInsertedDate, VeaInsertedUser,
                     VeaChangedDate, VeaChangedUser
              from dbo.T_Verfahrensart"),

        new("dbo.t_verfahrensschritt",
            new[] { Uuid("vesid"), Str("vesbezeichnung"), Int("vessortierung"), Bool("vesnurwerktage"),
                    Bool("vesoffen"), Bool("vesselektiv"), Str("vesnaturalid"), Bool("vesaktiv"),
                    Ticks("vesinserteddate"), Str("vesinserteduser"), Ticks("veschangeddate"), Str("veschangeduser") },
            @"select VesId, VesBezeichnung, VesSortierung, VesOnlyWorkingdays, VesOffen, VesSelektiv, VesNaturalId, VesAktiv,
                     VesInsertedDate, VesInsertedUser,
                     VesChangedDate, VesChangedUser
              from dbo.T_Verfahrensschritt"),

        // Eingereicht-/Verzögerungs-Felder sowie Vorankündigung/RFI existieren im Altsystem nicht (per INFORMATION_SCHEMA
        // verifiziert) — sie bleiben bewusst auf Default/NULL. AusPublikationPraequali wird nicht übernommen: Die
        // Präqualifikation wird als Offertzustand «Zuschlag Präqualifikation» erfasst (Kundenentscheid, manuell nachgetragen).
        new("dbo.t_ausschreibung",
            new[] { Uuid("ausid"), Uuid("ausamtid"), Uuid("ausleaid"), Uuid("ausriaid"), Uuid("auspraid"),
                    Uuid("ausveaid"), Uuid("ausausid"), Uuid("ausresid"), Uuid("aussacdoeaid"),
                    Uuid("aussacrdid"), Uuid("aussacbblid"), Str("ausbezeichnung"), Str("ausnummerstr"),
                    Int("ausnummer"), Int("auslaufnummer"), Int("ausjahr"), Int("ausabschlussjahr"), Str("ausextern"),
                    Bool("auspersonalverleih"), Str("aussimapnr"), Date("auspublikation"), Int("auspflichtenhefterpraequali"),
                    Date("auseingabefristteilnahme"), Int("ausanzahlteilnehmer"),
                    Date("ausversandph2"), Int("auspflichtenhefter"), Date("ausoffertenfrist"), Str("ausstandort"),
                    Str("ausbemerkung"), Bool("ausabgeschlossen"), Bool("ausbeschaffungdezentral"), Str("ausnaturalid"),
                    Bool("ausaktiv"), Ticks("ausinserteddate"), Str("ausinserteduser"), Ticks("auschangeddate"),
                    Str("auschangeduser") },
            @"select AusId, AusAmtId, AusLeaId, AusRolId, AusPraId, AusVeaId, AusAusId, AusResId,
                     AusSacDoeAId, AusSacRDId, AusSacBBLId, AusBezeichnung, AusNummerStr, AusNummer, AusLaufnummer,
                     AusJahr, AusAbschlussJahr, AusExtern, AusPersonalverleih, cast(AusSimapNr as varchar(255)), cast(AusPublikation as date),
                     AusPflichtenhefterPraequali, cast(AusEingabefristTeilnahme as date), AusAnzahlTeilnehmer,
                     cast(AusVersandPh2 as date), AusPflichtenhefter,
                     cast(AusOffertenFrist as date), AusStandort, AusBemerkung, AusAbgeschlossen,
                     cast(coalesce(AusKap3Voeb, 0) as bit), AusNaturalId, AusAktiv,
                     AusInsertedDate, AusInsertedUser,
                     AusChangedDate, AusChangedUser
              from dbo.T_Ausschreibung"),

        new("dbo.t_ausschreibungcpvnummer",
            new[] { Uuid("acnid"), Uuid("acnausid"), Uuid("acncpvid"), Str("acnnaturalid"), Bool("acnaktiv"),
                    Ticks("acninserteddate"), Str("acninserteduser"), Ticks("acnchangeddate"), Str("acnchangeduser") },
            @"select AcnId, AcnAusId, AcnCpvId, AcnNaturalId, AcnAktiv,
                     AcnInsertedDate, AcnInsertedUser,
                     AcnChangedDate, AcnChangedUser
              from dbo.T_AusschreibungCpvNummer"),

        new("dbo.t_ausschreibungsprache",
            new[] { Uuid("aspid"), Uuid("aspsprid"), Uuid("aspausid"), Uuid("aspdoaid"), Str("aspnaturalid"),
                    Bool("aspaktiv"), Ticks("aspinserteddate"), Str("aspinserteduser"), Ticks("aspchangeddate"),
                    Str("aspchangeduser") },
            @"select AspId, AspSprId, AspAusId, AspDoaId, AspNaturalId, AspAktiv,
                     AspInsertedDate, AspInsertedUser,
                     AspChangedDate, AspChangedUser
              from dbo.T_AusschreibungSprache"),

        new("dbo.t_ausschreibungverfahrensschritt",
            new[] { Uuid("avsid"), Uuid("avsausid"), Uuid("avsvesid"), Date("avsdatumvon"), Date("avsdatumbis"),
                    Str("avsnaturalid"), Bool("avsaktiv"), Ticks("avsinserteddate"), Str("avsinserteduser"),
                    Ticks("avschangeddate"), Str("avschangeduser") },
            @"select AvsId, AvsAusId, AvsVesId, cast(AvsDatumVon as date), cast(AvsDatumBis as date), AvsNaturalId, AvsAktiv,
                     AvsInsertedDate, AvsInsertedUser,
                     AvsChangedDate, AvsChangedUser
              from dbo.T_AusschreibungVerfahrenschritt"),

        new("dbo.t_los",
            new[] { Uuid("losid"), Uuid("losausid"), Date("losabbruchdatum"), Str("losbezeichnung"),
                    Str("loszustand"), Str("losnaturalid"), Bool("losaktiv"), Ticks("losinserteddate"),
                    Str("losinserteduser"), Ticks("loschangeddate"), Str("loschangeduser") },
            @"select LosId, LosAusId, cast(LosAbbruchdatum as date), LosBezeichnung, LosZustand, LosNaturalId, LosAktiv,
                     LosInsertedDate, LosInsertedUser,
                     LosChangedDate, LosChangedUser
              from dbo.T_Los"),

        new("dbo.t_losbkpnummer",
            new[] { Uuid("lbnid"), Uuid("lbnlosid"), Uuid("lbnbkpid"), Str("lbnnaturalid"), Bool("lbnaktiv"),
                    Ticks("lbninserteddate"), Str("lbninserteduser"), Ticks("lbnchangeddate"), Str("lbnchangeduser") },
            @"select LbnId, LbnLosId, LbnBkpId, LbnNaturalId, LbnAktiv,
                     LbnInsertedDate, LbnInsertedUser,
                     LbnChangedDate, LbnChangedUser
              from dbo.T_LosBkpNummer"),

        new("dbo.t_offerte",
            new[] { Uuid("offid"), Uuid("offlosid"), Uuid("offanbid"), Str("offbemerkung"), Num("offzuschlagswertchf"),
                    Str("offnaturalid"), Bool("offaktiv"), Ticks("offinserteddate"), Str("offinserteduser"),
                    Ticks("offchangeddate"), Str("offchangeduser") },
            @"select OffId, OffLosId, OffAnbId, OffBemerkung, cast(OffZuschlagswertChf as decimal(18,2)), OffNaturalId, OffAktiv,
                     OffInsertedDate, OffInsertedUser,
                     OffChangedDate, OffChangedUser
              from dbo.T_Offerte"),

        new("dbo.t_offerteoffertezustand",
            new[] { Uuid("oozid"), Uuid("oozoffid"), Uuid("oozofzid"), Date("oozdatum"), Str("ooznaturalid"),
                    Bool("oozaktiv"), Ticks("oozinserteddate"), Str("oozinserteduser"), Ticks("oozchangeddate"),
                    Str("oozchangeduser") },
            @"select OozId, OozOffId, OozOfzId, cast(OozDatum as date), OozNaturalId, OozAktiv,
                     OozInsertedDate, OozInsertedUser,
                     OozChangedDate, OozChangedUser
              from dbo.T_OfferteOffertezustand"),
    };
}
