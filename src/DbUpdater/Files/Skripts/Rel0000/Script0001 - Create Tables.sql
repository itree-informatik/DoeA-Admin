-- SPDX-License-Identifier: AGPL-3.0-or-later
-- Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

-------------------------------------------------------------------------------
-- tables
-------------------------------------------------------------------------------

create table dbo.t_amt
(
	amtid uuid not null,
	amtdepid uuid not null,
	amtbezeichnung varchar(255) not null,
	amtkurzbezeichnung varchar(255) not null,
	amtdienststellennummer varchar(255) not null,
	amtnaturalid varchar(255) null,
	amtaktiv boolean not null,
	amtinserteddate bigint not null,
	amtinserteduser varchar(255) not null,
	amtchangeddate bigint not null,
	amtchangeduser varchar(255) not null,
constraint pk_amt primary key (amtid));

create table dbo.t_anbieter
(
	anbid uuid not null,
	anbortid uuid not null,
	anbname varchar(255) not null,
	anbadresse varchar(255) not null,
	anbadresse2 varchar(255) null,
	anbadresse3 varchar(255) null,
	anbnaturalid varchar(255) null,
	anbaktiv boolean not null,
	anbinserteddate bigint not null,
	anbinserteduser varchar(255) not null,
	anbchangeddate bigint not null,
	anbchangeduser varchar(255) not null,
constraint pk_anbieter primary key (anbid));

create table dbo.t_ausschreibung(
	ausid uuid not null,
	ausamtid uuid null,
	ausleaid uuid null,
	ausriaid uuid null,
	auspraid uuid not null,
	ausveaid uuid not null,
	ausausid uuid null,
	ausresid uuid null,
	aussacdoeaid uuid null,
	aussacrdid uuid null,
	aussacbblid uuid null,
	ausvorankuendigungid uuid null,
	ausrfiid uuid null,
	ausbezeichnung varchar(1000) null,
	ausnummerstr varchar(255) null,
	ausnummer int null,
	auslaufnummer int null,
	ausjahr int not null,
	ausabschlussjahr int null,
	ausextern varchar(255) null,
	auspersonalverleih boolean not null,
	aussimapnr varchar(255) null,
	auseingereicht date null,
	auspublikation date null,
	ausverzoegerungdurchauftraggeber boolean not null,
	auspflichtenhefterpraequali int null,
	auseingabefristteilnahme date null,
	ausanzahlteilnehmer int null,
	auseingereichtpraequali date null,
	auspublikationpraequali date null,
	ausverzoegerungdurchauftraggeberpraequali boolean not null,
	ausversandph2 date null,
	auspflichtenhefter int null,
	ausoffertenfrist date null,
	ausstandort varchar(255) null,
	ausbemerkung varchar(4000) null,
	ausabgeschlossen boolean not null,
	ausbeschaffungdezentral boolean not null,
	ausnaturalid varchar(255) null,
	ausaktiv boolean not null,
	ausinserteddate bigint not null,
	ausinserteduser varchar(255) not null,
	auschangeddate bigint not null,
	auschangeduser varchar(255) not null,
constraint pk_ausschreibung primary key (ausid));

create table dbo.t_ausschreibungcpvnummer
(
	acnid uuid not null,
	acnausid uuid not null,
	acncpvid uuid not null,
	acnnaturalid varchar(255) null,
	acnaktiv boolean not null,
	acninserteddate bigint not null,
	acninserteduser varchar(255) not null,
	acnchangeddate bigint not null,
	acnchangeduser varchar(255) not null,
constraint pk_ausschreibungcpvnummer primary key (acnid));

create table dbo.t_ausschreibungsprache
(
	aspid uuid not null,
	aspsprid uuid not null,
	aspausid uuid not null,
	aspdoaid uuid not null,
	aspnaturalid varchar(255) null,
	aspaktiv boolean not null,
	aspinserteddate bigint not null,
	aspinserteduser varchar(255) not null,
	aspchangeddate bigint not null,
	aspchangeduser varchar(255) not null,
constraint pk_ausschreibungsprache primary key (aspid));

create table dbo.t_ausschreibungverfahrensschritt
(
	avsid uuid not null,
	avsausid uuid not null,
	avsvesid uuid not null,
	avsdatumvon date null,
	avsdatumbis date null,
	avsnaturalid varchar(255) null,
	avsaktiv boolean not null,
	avsinserteddate bigint not null,
	avsinserteduser varchar(255) not null,
	avschangeddate bigint not null,
	avschangeduser varchar(255) not null,
constraint pk_ausschreibungverfahrensschritt primary key (avsid));

create table dbo.t_berichtigung
(
	berid uuid not null,
	berausid uuid not null,
	bereingereicht date null,
	berpubliziert date null,
	berverzoegerungdurchauftraggeber boolean not null,
	berbemerkungen varchar(4000) null,
	bernaturalid varchar(255) null,
	beraktiv boolean not null,
	berinserteddate bigint not null,
	berinserteduser varchar(255) not null,
	berchangeddate bigint not null,
	berchangeduser varchar(255) not null,
constraint pk_berichtigung primary key (berid));

create table dbo.t_bkpnummer(
	bkpid uuid not null,
	bkpnummerstr varchar(255) not null,
	bkpbezeichnung varchar(255) not null,
	bkpnummer int not null,
	bkpnaturalid varchar(255) null,
	bkpaktiv boolean not null,
	bkpinserteddate bigint not null,
	bkpinserteduser varchar(255) not null,
	bkpchangeddate bigint not null,
	bkpchangeduser varchar(255) not null,
constraint pk_bkpnummer primary key (bkpid));

create table dbo.t_konfiguration(
	konid uuid not null,
	konbezeichnung varchar(255) not null,
	konwert text not null,
	kontyp varchar(50) not null,
	konbeschreibung varchar(1000) not null,
	konnaturalid varchar(255) not null,
	konaktiv boolean not null,
	koninserteddate bigint not null,
	koninserteduser varchar(255) not null,
	konchangeddate bigint not null,
	konchangeduser varchar(255) not null,
constraint pk_konfiguration primary key (konid));

create table dbo.t_cpvnummer(
	cpvid uuid not null,
	cpvnummerstr varchar(255) not null,
	cpvbezeichnung varchar(255) not null,
	cpvnummer int not null,
	cpvnaturalid varchar(255) null,
	cpvaktiv boolean not null,
	cpvinserteddate bigint not null,
	cpvinserteduser varchar(255) not null,
	cpvchangeddate bigint not null,
	cpvchangeduser varchar(255) not null,
constraint pk_cpvnummer primary key (cpvid));

create table dbo.t_departement(
	depid uuid not null,
	depbezeichnung varchar(255) not null,
	depkurzbezeichnung varchar(255) not null,
	depnaturalid varchar(255) null,
	depaktiv boolean not null,
	depinserteddate bigint not null,
	depinserteduser varchar(255) not null,
	depchangeddate bigint not null,
	depchangeduser varchar(255) not null,
constraint pk_department primary key (depid));

create table dbo.t_dokumentart(
	doaid uuid not null,
	doabezeichnung varchar(255) not null,
	doanaturalid varchar(255) null,
	doasortierung int not null,
	doaaktiv boolean not null,
	doainserteddate bigint not null,
	doainserteduser varchar(255) not null,
	doachangeddate bigint not null,
	doachangeduser varchar(255) not null,
constraint pk_dokumentart primary key (doaid));

create table dbo.t_kanton(
	kanid uuid not null,
	kansprid uuid not null,
	kanbezeichnung varchar(255) not null,
	kankurzbezeichnung varchar(255) not null,
	kannaturalid varchar(255) null,
	kanaktiv boolean not null,
	kaninserteddate bigint not null,
	kaninserteduser varchar(255) not null,
	kanchangeddate bigint not null,
	kanchangeduser varchar(255) not null,
constraint pk_kanton primary key (kanid));

create table dbo.t_land(
	lanid uuid not null,
	lanbezeichnung varchar(255) not null,
	lankurzbezeichnung varchar(255) not null,
	lannaturalid varchar(255) null,
	lanaktiv boolean not null,
	laninserteddate bigint not null,
	laninserteduser varchar(255) not null,
	lanchangeddate bigint not null,
	lanchangeduser varchar(255) not null,
constraint pk_land primary key (lanid));

create table dbo.t_leistungsart(
	leaid uuid not null,
	leapraid uuid not null,
	leabezeichnung varchar(255) not null,
	leakurzbezeichnung varchar(255) not null,
	leanaturalid varchar(255) null,
	leaaktiv boolean not null,
	leainserteddate bigint not null,
	leainserteduser varchar(255) not null,
	leachangeddate bigint not null,
	leachangeduser varchar(255) not null,
constraint pk_leistungsart primary key (leaid));

create table dbo.t_log(
	id integer generated by default as identity not null,
	message text null,
	messagetemplate text null,
	level text null,
	"timestamp" timestamp null,
	exception text null,
	properties text null,
constraint pk_log primary key (id));

create table dbo.t_los(
	losid uuid not null,
	losausid uuid not null,
	losabbruchdatum date null,
	losbezeichnung varchar(255) not null,
	loszustand varchar(255) not null,
	losnaturalid varchar(255) null,
	losaktiv boolean not null,
	losinserteddate bigint not null,
	losinserteduser varchar(255) not null,
	loschangeddate bigint not null,
	loschangeduser varchar(255) not null,
constraint pk_los primary key (losid));

create table dbo.t_losbkpnummer(
	lbnid uuid not null,
	lbnlosid uuid not null,
	lbnbkpid uuid not null,
	lbnnaturalid varchar(255) null,
	lbnaktiv boolean not null,
	lbninserteddate bigint not null,
	lbninserteduser varchar(255) not null,
	lbnchangeddate bigint not null,
	lbnchangeduser varchar(255) not null,
constraint pk_losbkpnummer primary key (lbnid));

create table dbo.t_offerte(
	offid uuid not null,
	offlosid uuid not null,
	offanbid uuid not null,
	offbemerkung varchar(4000) null,
	offzuschlagswertchf numeric(18,2) null,
	offanzahlmitarbeiter int null,
	offnaturalid varchar(255) null,
	offaktiv boolean not null,
	offinserteddate bigint not null,
	offinserteduser varchar(255) not null,
	offchangeddate bigint not null,
	offchangeduser varchar(255) not null,
constraint pk_t_offerte primary key (offid));

create table dbo.t_offerteoffertezustand(
	oozid uuid not null,
	oozoffid uuid not null,
	oozofzid uuid not null,
	oozdatum date not null,
	ooznaturalid varchar(255) null,
	oozaktiv boolean not null,
	oozinserteddate bigint not null,
	oozinserteduser varchar(255) not null,
	oozchangeddate bigint not null,
	oozchangeduser varchar(255) not null,
constraint pk_offerteoffertezustand primary key (oozid));

create table dbo.t_offertezustand(
	ofzid uuid not null,
	ofzbezeichnung varchar(255) not null,
	ofzkurzbezeichnung varchar(255) not null,
	ofzschlusszustand boolean not null,
	ofznaturalid varchar(255) null,
	ofzaktiv boolean not null,
	ofzinserteddate bigint not null,
	ofzinserteduser varchar(255) not null,
	ofzchangeddate bigint not null,
	ofzchangeduser varchar(255) not null,
constraint pk_offertezustand primary key (ofzid));

create table dbo.t_organisationszugehoerigkeit(
	orgid uuid not null,
	orgbezeichnung varchar(255) not null,
	orgkurzbezeichnung varchar(255) not null,
	orgnaturalid varchar(255) null,
	orgaktiv boolean not null,
	orginserteddate bigint not null,
	orginserteduser varchar(255) not null,
	orgchangeddate bigint not null,
	orgchangeduser varchar(255) not null,
constraint pk_organisationszugehoerigkeit primary key (orgid));

create table dbo.t_ortschaft(
	ortid uuid not null,
	ortlanid uuid not null,
	ortkanid uuid null,
	ortort varchar(255) not null,
	ortplz varchar(255) not null,
	ortnaturalid varchar(255) null,
	ortaktiv boolean not null,
	ortinserteddate bigint not null,
	ortinserteduser varchar(255) not null,
	ortchangeddate bigint not null,
	ortchangeduser varchar(255) not null,
constraint pk_ortschaft primary key (ortid));

create table dbo.t_projektart(
	praid uuid not null,
	prabezeichnung varchar(255) not null,
	prakurzbezeichnung varchar(255) not null,
	pranaturalid varchar(255) null,
	praaktiv boolean not null,
	prainserteddate bigint not null,
	prainserteduser varchar(255) not null,
	prachangeddate bigint not null,
	prachangeduser varchar(255) not null,
constraint pk_projektart primary key (praid));

create table dbo.t_release(
	relid uuid not null,
	relversion varchar(255) not null,
	reldatum date not null,
constraint pk_release primary key (relid));

create table dbo.t_releasecontent(
	recid uuid not null,
	recrelid uuid not null,
	rectyp varchar(255) not null,
	rectitel varchar(255) not null,
	rectext varchar(4000) not null,
constraint pk_releasecontent primary key (recid));

create table dbo.t_ressort(
	resid uuid not null,
	resamtid uuid not null,
	resbezeichnung varchar(255) not null,
	reskurzbezeichnung varchar(255) not null,
	resnaturalid varchar(255) null,
	resaktiv boolean not null,
	resinserteddate bigint not null,
	resinserteduser varchar(255) not null,
	reschangeddate bigint not null,
	reschangeduser varchar(255) not null,
constraint pk_ressort primary key (resid));

create table dbo.t_rolleinausschreibung(
	riaid uuid not null,
	riabezeichnung varchar(1000) not null,
	riakurzbezeichnung varchar(255) not null,
	rianaturalid varchar(255) null,
	riaaktiv boolean not null,
	riainserteddate bigint not null,
	riainserteduser varchar(255) not null,
	riachangeddate bigint not null,
	riachangeduser varchar(255) not null,
constraint pk_rolleinausschreibung primary key (riaid));

create table dbo.t_sachbearbeiter(
	sacid uuid not null,
	sacorgid uuid not null,
	sacnachname varchar(255) not null,
	sacvorname varchar(255) not null,
	sackurzbezeichnung varchar(255) not null,
	sacnaturalid varchar(255) null,
	sacaktiv boolean not null,
	sacinserteddate bigint not null,
	sacinserteduser varchar(255) not null,
	sacchangeddate bigint not null,
	sacchangeduser varchar(255) not null,
constraint pk_sachbearbeiter primary key (sacid));

create table dbo.t_sprache(
	sprid uuid not null,
	sprculture varchar(255) not null,
	sprbezeichnung varchar(255) not null,
	sprnaturalid varchar(255) not null,
	sprsortierung int not null,
	spraktiv boolean not null,
	sprinserteddate bigint not null,
	sprinserteduser varchar(255) not null,
	sprchangeddate bigint not null,
	sprchangeduser varchar(255) not null,
constraint pk_sprache primary key (sprid));

create table dbo.t_translation(
	trlid uuid not null,
	trlkey varchar(255) not null,
	trldeutsch varchar(4000) not null,
	trlfranzoesisch varchar(4000) null,
	trlitalienisch varchar(4000) null,
	trlrelid uuid not null,
constraint pk_translation primary key (trlid));

-- Benutzer aus dem eIAM: usrextid = Claim "sub"; usraktiv steuert die Autorisierung in DoeA
create table dbo.t_user(
	usrid uuid not null,
	usrextid varchar(255) not null,
	usrvorname varchar(255) not null,
	usrnachname varchar(255) not null,
	usremail varchar(255) not null,
	usrsprache varchar(10) not null,
	usrlastlogin bigint null,
	usraktiv boolean not null,
constraint pk_user primary key (usrid));

-- ASP.NET Data-Protection-Schlüssel (Verschlüsselung des Auth-Cookies), damit Cookies
-- Container-Neustarts und mehrere Instanzen überleben; Spalten gemäss DataProtectionKey-Entity
create table dbo.t_dataprotectionkey(
	id integer generated by default as identity not null,
	friendlyname text null,
	xml text null,
constraint pk_dataprotectionkey primary key (id));

create table dbo.t_verfahrensart(
	veaid uuid not null,
	veabezeichnung varchar(255) not null,
	veakurzbezeichnung varchar(255) not null,
	veasortierung int not null,
	veanaturalid varchar(255) null,
	veaaktiv boolean not null,
	veainserteddate bigint not null,
	veainserteduser varchar(255) not null,
	veachangeddate bigint not null,
	veachangeduser varchar(255) not null,
constraint pk_verfahrensart primary key (veaid));

create table dbo.t_verfahrensschritt(
	vesid uuid not null,
	vesbezeichnung varchar(255) not null,
	vessortierung int not null,
	vesnurwerktage boolean not null,
	vesoffen boolean not null,
	vesselektiv boolean not null,
	vesnaturalid varchar(255) null,
	vesaktiv boolean not null,
	vesinserteddate bigint not null,
	vesinserteduser varchar(255) not null,
	veschangeddate bigint not null,
	veschangeduser varchar(255) not null,
constraint pk_t_verfahrensschritt primary key (vesid));

create table dbo.t_exportgruppe(
	expid uuid not null,
	expname varchar(1000) not null,
	expjahr int null,
	expabschlussjahr int null,
	exppersonalverleih boolean null,
	expabgeschlossen boolean null,
	expbeschaffungdezentral boolean null,
	exppublikationvon date null,
	exppublikationbis date null,
	expnaturalid varchar(255) null,
	expspalten text null,
	expaktiv boolean not null,
	expinserteddate bigint not null,
	expinserteduser varchar(255) not null,
	expchangeddate bigint not null,
	expchangeduser varchar(255) not null,
constraint pk_exportgruppe primary key (expid));

create table dbo.t_exportgruppeamt(
	exaid uuid not null,
	exaexpid uuid not null,
	exaamtid uuid not null,
	exanaturalid varchar(255) null,
	exaaktiv boolean not null,
	exainserteddate bigint not null,
	exainserteduser varchar(255) not null,
	exachangeddate bigint not null,
	exachangeduser varchar(255) not null,
constraint pk_exportgruppeamt primary key (exaid));

create table dbo.t_exportgruppeleistungsart(
	exlid uuid not null,
	exlexpid uuid not null,
	exlleaid uuid not null,
	exlnaturalid varchar(255) null,
	exlaktiv boolean not null,
	exlinserteddate bigint not null,
	exlinserteduser varchar(255) not null,
	exlchangeddate bigint not null,
	exlchangeduser varchar(255) not null,
constraint pk_exportgruppeleistungsart primary key (exlid));

create table dbo.t_exportgrupperolleinausschreibung(
	exrid uuid not null,
	exrexpid uuid not null,
	exrriaid uuid not null,
	exrnaturalid varchar(255) null,
	exraktiv boolean not null,
	exrinserteddate bigint not null,
	exrinserteduser varchar(255) not null,
	exrchangeddate bigint not null,
	exrchangeduser varchar(255) not null,
constraint pk_exportgrupperolleinausschreibung primary key (exrid));

create table dbo.t_exportgruppeprojektart(
	eprid uuid not null,
	eprexpid uuid not null,
	eprpraid uuid not null,
	eprnaturalid varchar(255) null,
	epraktiv boolean not null,
	eprinserteddate bigint not null,
	eprinserteduser varchar(255) not null,
	eprchangeddate bigint not null,
	eprchangeduser varchar(255) not null,
constraint pk_exportgruppeprojektart primary key (eprid));

create table dbo.t_exportgruppeverfahrensart(
	evaid uuid not null,
	evaexpid uuid not null,
	evaveaid uuid not null,
	evanaturalid varchar(255) null,
	evaaktiv boolean not null,
	evainserteddate bigint not null,
	evainserteduser varchar(255) not null,
	evachangeddate bigint not null,
	evachangeduser varchar(255) not null,
constraint pk_exportgruppeverfahrensart primary key (evaid));

create table dbo.t_exportgrupperessort(
	ereid uuid not null,
	ereexpid uuid not null,
	ereresid uuid not null,
	erenaturalid varchar(255) null,
	ereaktiv boolean not null,
	ereinserteddate bigint not null,
	ereinserteduser varchar(255) not null,
	erechangeddate bigint not null,
	erechangeduser varchar(255) not null,
constraint pk_exportgrupperessort primary key (ereid));

-- exsrolle: Rolle des Sachbearbeiter-Filters (DoeA, RD, BBL), damit je Rolle getrennt gefiltert wird
create table dbo.t_exportgruppesachbearbeiter(
	exsid uuid not null,
	exsexpid uuid not null,
	exssacid uuid not null,
	exsnaturalid varchar(255) null,
	exsaktiv boolean not null,
	exsinserteddate bigint not null,
	exsinserteduser varchar(255) not null,
	exschangeddate bigint not null,
	exschangeduser varchar(255) not null,
	exsrolle varchar(10) not null,
constraint pk_exportgruppesachbearbeiter primary key (exsid));

create table dbo.t_exportgruppekanton(
    ekaid uuid not null,
    ekaexpid uuid not null,
    ekakanid uuid not null,
    ekanaturalid varchar(255) null,
    ekaaktiv boolean not null,
    ekainserteddate bigint not null,
    ekainserteduser varchar(255) not null,
    ekachangeddate bigint not null,
    ekachangeduser varchar(255) not null,
constraint pk_exportgruppekanton primary key (ekaid));

-------------------------------------------------------------------------------
-- unique keys
-------------------------------------------------------------------------------

create unique index uq_t_bkpnummer_bkpnummerstr on dbo.t_bkpnummer (bkpnummerstr);

create unique index uq_t_translation_trlkey on dbo.t_translation (trlkey);


-------------------------------------------------------------------------------
-- defaults
-------------------------------------------------------------------------------

alter table dbo.t_amt alter column amtaktiv set default false;

alter table dbo.t_anbieter alter column anbaktiv set default false;

alter table dbo.t_ausschreibung alter column ausaktiv set default false;
alter table dbo.t_ausschreibung alter column auspersonalverleih set default false;
alter table dbo.t_ausschreibung alter column ausverzoegerungdurchauftraggeber set default false;
alter table dbo.t_ausschreibung alter column ausverzoegerungdurchauftraggeberpraequali set default false;
alter table dbo.t_ausschreibung alter column ausabgeschlossen set default false;
alter table dbo.t_ausschreibung alter column ausbeschaffungdezentral set default false;

alter table dbo.t_ausschreibungcpvnummer alter column acnaktiv set default false;

alter table dbo.t_ausschreibungsprache alter column aspaktiv set default false;

alter table dbo.t_ausschreibungverfahrensschritt alter column avsaktiv set default false;

alter table dbo.t_berichtigung alter column beraktiv set default false;
alter table dbo.t_berichtigung alter column berverzoegerungdurchauftraggeber set default false;

alter table dbo.t_bkpnummer alter column bkpaktiv set default false;

alter table dbo.t_konfiguration alter column konaktiv set default false;

alter table dbo.t_cpvnummer alter column cpvaktiv set default false;

alter table dbo.t_departement alter column depaktiv set default false;

alter table dbo.t_dokumentart alter column doaaktiv set default false;
alter table dbo.t_dokumentart alter column doasortierung set default 0;

alter table dbo.t_kanton alter column kanaktiv set default false;

alter table dbo.t_land alter column lanaktiv set default false;

alter table dbo.t_leistungsart alter column leaaktiv set default false;

alter table dbo.t_los alter column losaktiv set default false;

alter table dbo.t_losbkpnummer alter column lbnaktiv set default false;

alter table dbo.t_offerte alter column offaktiv set default false;

alter table dbo.t_offerteoffertezustand alter column oozaktiv set default false;

alter table dbo.t_offertezustand alter column ofzaktiv set default false;
alter table dbo.t_offertezustand alter column ofzschlusszustand set default false;

alter table dbo.t_organisationszugehoerigkeit alter column orgaktiv set default false;

alter table dbo.t_ortschaft alter column ortaktiv set default false;

alter table dbo.t_projektart alter column praaktiv set default false;

alter table dbo.t_ressort alter column resaktiv set default false;

alter table dbo.t_rolleinausschreibung alter column riaaktiv set default false;

alter table dbo.t_sachbearbeiter alter column sacaktiv set default false;

alter table dbo.t_sprache alter column spraktiv set default false;
alter table dbo.t_sprache alter column sprsortierung set default 0;

alter table dbo.t_verfahrensart alter column veaaktiv set default false;
alter table dbo.t_verfahrensart alter column veasortierung set default 0;

alter table dbo.t_verfahrensschritt alter column vesaktiv set default false;
alter table dbo.t_verfahrensschritt alter column vesoffen set default false;
alter table dbo.t_verfahrensschritt alter column vesselektiv set default false;
alter table dbo.t_verfahrensschritt alter column vesnurwerktage set default false;
alter table dbo.t_verfahrensschritt alter column vessortierung set default 0;

-- Die Ja/Nein-Filter exppersonalverleih, expabgeschlossen und expbeschaffungdezentral bleiben ohne Default: NULL heisst «egal»
alter table dbo.t_exportgruppe alter column expaktiv set default false;

alter table dbo.t_user alter column usraktiv set default false;


-------------------------------------------------------------------------------
-- foreign keys
-------------------------------------------------------------------------------

alter table dbo.t_anbieter add constraint fk_anbieter_ortschaft foreign key(anbortid)
references dbo.t_ortschaft (ortid);

alter table dbo.t_amt add constraint fk_amt_departement foreign key(amtdepid)
references dbo.t_departement (depid);

alter table dbo.t_ausschreibung add constraint fk_ausschreibung_amt foreign key(ausamtid)
references dbo.t_amt (amtid);
alter table dbo.t_ausschreibung add constraint fk_ausschreibung_ausschreibung foreign key(ausausid)
references dbo.t_ausschreibung (ausid);
alter table dbo.t_ausschreibung add constraint fk_ausschreibung_leistungsart foreign key(ausleaid)
references dbo.t_leistungsart (leaid);
alter table dbo.t_ausschreibung add constraint fk_ausschreibung_projektart foreign key(auspraid)
references dbo.t_projektart (praid);
alter table dbo.t_ausschreibung add constraint fk_ausschreibung_ressort foreign key(ausresid)
references dbo.t_ressort (resid);
alter table dbo.t_ausschreibung add constraint fk_ausschreibung_rfi foreign key(ausrfiid)
references dbo.t_ausschreibung (ausid);
alter table dbo.t_ausschreibung add constraint fk_ausschreibung_rolle foreign key(ausriaid)
references dbo.t_rolleinausschreibung (riaid);
alter table dbo.t_ausschreibung add constraint fk_ausschreibung_sachbearbeiterbbl foreign key(aussacbblid)
references dbo.t_sachbearbeiter (sacid);
alter table dbo.t_ausschreibung add constraint fk_ausschreibung_sachbearbeiterdoea foreign key(aussacdoeaid)
references dbo.t_sachbearbeiter (sacid);
alter table dbo.t_ausschreibung add constraint fk_ausschreibung_sachbearbeiterrd foreign key(aussacrdid)
references dbo.t_sachbearbeiter (sacid);
alter table dbo.t_ausschreibung add constraint fk_ausschreibung_verfahrensart foreign key(ausveaid)
references dbo.t_verfahrensart (veaid);
alter table dbo.t_ausschreibung add constraint fk_ausschreibung_vorankuendigung foreign key(ausvorankuendigungid)
references dbo.t_ausschreibung (ausid);

alter table dbo.t_ausschreibungcpvnummer add constraint fk_ausschreibungcpvnummer_ausschreibung foreign key(acnausid)
references dbo.t_ausschreibung (ausid);
alter table dbo.t_ausschreibungcpvnummer add constraint fk_ausschreibungcpvnummer_cpvnummer foreign key(acncpvid)
references dbo.t_cpvnummer (cpvid);

alter table dbo.t_ausschreibungsprache add constraint fk_ausschreibungsprache_ausschreibung foreign key(aspausid)
references dbo.t_ausschreibung (ausid);

alter table dbo.t_ausschreibungsprache add constraint fk_ausschreibungsprache_dokumentart foreign key(aspdoaid)
references dbo.t_dokumentart (doaid);
alter table dbo.t_ausschreibungsprache add constraint fk_ausschreibungsprache_sprache foreign key(aspsprid)
references dbo.t_sprache (sprid);

alter table dbo.t_ausschreibungverfahrensschritt add constraint fk_ausschreibungverfahrensschritt_ausschreibung foreign key(avsausid)
references dbo.t_ausschreibung (ausid);
alter table dbo.t_ausschreibungverfahrensschritt add constraint fk_ausschreibungverfahrensschritt_verfahrensschritt foreign key(avsvesid)
references dbo.t_verfahrensschritt (vesid);

alter table dbo.t_berichtigung add constraint fk_berichtigung_ausschreibung foreign key(berausid)
references dbo.t_ausschreibung (ausid);

alter table dbo.t_kanton add constraint fk_kanton_sprache foreign key(kansprid)
references dbo.t_sprache (sprid);

alter table dbo.t_leistungsart add constraint fk_leistungsart_projektart foreign key(leapraid)
references dbo.t_projektart (praid);

alter table dbo.t_los add constraint fk_los_ausschreibung foreign key(losausid)
references dbo.t_ausschreibung (ausid);

alter table dbo.t_losbkpnummer add constraint fk_losbkpnummer_bkpnummer foreign key(lbnbkpid)
references dbo.t_bkpnummer (bkpid);
alter table dbo.t_losbkpnummer add constraint fk_losbkpnummer_los foreign key(lbnlosid)
references dbo.t_los (losid);

alter table dbo.t_offerte add constraint fk_offerte_anbieter foreign key(offanbid)
references dbo.t_anbieter (anbid);
alter table dbo.t_offerte add constraint fk_offerte_los foreign key(offlosid)
references dbo.t_los (losid);

alter table dbo.t_offerteoffertezustand add constraint fk_offerteoffertezustand_offerte foreign key(oozoffid)
references dbo.t_offerte (offid);
alter table dbo.t_offerteoffertezustand add constraint fk_offerteoffertezustand_offertezustand foreign key(oozofzid)
references dbo.t_offertezustand (ofzid);

alter table dbo.t_ortschaft add constraint fk_ortschaft_kanton foreign key(ortkanid)
references dbo.t_kanton (kanid);
alter table dbo.t_ortschaft add constraint fk_ortschaft_land foreign key(ortlanid)
references dbo.t_land (lanid);

alter table dbo.t_ressort add constraint fk_ressort_amt foreign key(resamtid)
references dbo.t_amt (amtid);

alter table dbo.t_sachbearbeiter add constraint fk_sachbearbeiter_organisationszugehoerigkeit foreign key(sacorgid)
references dbo.t_organisationszugehoerigkeit (orgid);

alter table dbo.t_translation add constraint fk_translation_release foreign key (trlrelid)
references dbo.t_release (relid);

alter table dbo.t_exportgruppeamt add constraint fk_exportgruppeamt_exportgruppe foreign key(exaexpid)
references dbo.t_exportgruppe (expid) on delete cascade;
alter table dbo.t_exportgruppeamt add constraint fk_exportgruppeamt_amt foreign key(exaamtid)
references dbo.t_amt (amtid) on delete cascade;

alter table dbo.t_exportgruppeleistungsart add constraint fk_exportgruppeleistungsart_exportgruppe foreign key(exlexpid)
references dbo.t_exportgruppe (expid) on delete cascade;
alter table dbo.t_exportgruppeleistungsart add constraint fk_exportgruppeleistungsart_leistungsart foreign key(exlleaid)
references dbo.t_leistungsart (leaid) on delete cascade;

alter table dbo.t_exportgrupperolleinausschreibung add constraint fk_exportgrupperolleinausschreibung_exportgruppe foreign key(exrexpid)
references dbo.t_exportgruppe (expid) on delete cascade;
alter table dbo.t_exportgrupperolleinausschreibung add constraint fk_exportgrupperolleinausschreibung_rolleinausschreibung foreign key(exrriaid)
references dbo.t_rolleinausschreibung (riaid) on delete cascade;

alter table dbo.t_exportgruppeprojektart add constraint fk_exportgruppeprojektart_exportgruppe foreign key(eprexpid)
references dbo.t_exportgruppe (expid) on delete cascade;
alter table dbo.t_exportgruppeprojektart add constraint fk_exportgruppeprojektart_projektart foreign key(eprpraid)
references dbo.t_projektart (praid) on delete cascade;

alter table dbo.t_exportgruppeverfahrensart add constraint fk_exportgruppeverfahrensart_exportgruppe foreign key(evaexpid)
references dbo.t_exportgruppe (expid) on delete cascade;
alter table dbo.t_exportgruppeverfahrensart add constraint fk_exportgruppeverfahrensart_verfahrensart foreign key(evaveaid)
references dbo.t_verfahrensart (veaid) on delete cascade;

alter table dbo.t_exportgrupperessort add constraint fk_exportgrupperessort_exportgruppe foreign key(ereexpid)
references dbo.t_exportgruppe (expid) on delete cascade;
alter table dbo.t_exportgrupperessort add constraint fk_exportgrupperessort_ressort foreign key(ereresid)
references dbo.t_ressort (resid) on delete cascade;

alter table dbo.t_exportgruppesachbearbeiter add constraint fk_exportgruppesachbearbeiter_exportgruppe foreign key(exsexpid)
references dbo.t_exportgruppe (expid) on delete cascade;
alter table dbo.t_exportgruppesachbearbeiter add constraint fk_exportgruppesachbearbeiter_sachbearbeiter foreign key(exssacid)
references dbo.t_sachbearbeiter (sacid) on delete cascade;

alter table dbo.t_exportgruppekanton add constraint fk_exportgruppekanton_exportgruppe foreign key(ekaexpid)
references dbo.t_exportgruppe (expid) on delete cascade;
alter table dbo.t_exportgruppekanton add constraint fk_exportgruppekanton_kanton foreign key(ekakanid)
references dbo.t_kanton (kanid);
