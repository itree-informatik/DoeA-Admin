-- SPDX-License-Identifier: AGPL-3.0-or-later
-- Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

do $$
declare
    v_relid uuid;
begin

    insert into dbo.t_release values (gen_random_uuid(), '1.0.0', current_date);
    v_relid := dbo.f_getlatestreleaseid();

    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_AUSSCHREIBUNGEN', 'Ausschreibungen', 'Appels d''offres', 'Bandi di concorso', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_AUSWERTUNGEN', 'Auswertungen', 'Évaluations', 'Valutazioni', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_STAMMDATEN', 'Stammdaten', 'Données de base', 'Dati di base', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_STAMMDATEN_PROJEKT', 'Projekt', 'Projet', 'Progetto', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_STAMMDATEN_GEOGRAPHISCHE', 'Geographische', 'Géographiques', 'Geografici', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_STAMMDATEN_ORGANISATION', 'Organisation', 'Organisation', 'Organizzazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_STAMMDATEN_GENERELL', 'Generell', 'Général', 'Generale', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_STAMMDATEN_AMT', 'Amt', 'Office', 'Ufficio', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_STAMMDATEN_ANBIETER', 'Anbieter', 'Fournisseur', 'Fornitore', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_STAMMDATEN_BKPNUMMER', 'BKP-Nummer', 'Numéro CFC', 'Numero CFC', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_STAMMDATEN_CPVNUMMER', 'CPV-Nummer', 'Numéro CPV', 'Numero CPV', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_STAMMDATEN_DEPARTEMENT', 'Departement', 'Département', 'Dipartimento', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_STAMMDATEN_DOKUMENTART', 'Dokumentart', 'Type de document', 'Tipo di documento', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_STAMMDATEN_KANTON', 'Kanton', 'Canton', 'Cantone', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_STAMMDATEN_LAND', 'Land', 'Pays', 'Paese', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_STAMMDATEN_LEISTUNGSART', 'Leistungsart', 'Type de prestation', 'Tipo di prestazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_STAMMDATEN_OFFERTEZUSTAND', 'Offertezustand', 'État de l''offre', 'Stato dell''offerta', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_STAMMDATEN_ORGANISATIONSZUGEHOERIGKEIT', 'Organisationszugehörigkeit', 'Appartenance organisationnelle', 'Appartenenza organizzativa', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_STAMMDATEN_ORTSCHAFT', 'Ortschaft', 'Localité', 'Località', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_STAMMDATEN_PROJEKTART', 'Projektart', 'Type de projet', 'Tipo di progetto', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_STAMMDATEN_RESSORT', 'Ressort', 'Ressort', 'Settore', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_STAMMDATEN_ROLLEINAUSSCHREIBUNG', 'Rolle in Ausschreibung', 'Rôle dans l''appel d''offres', 'Ruolo nel bando', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_STAMMDATEN_SACHBEARBEITER', 'Sachbearbeiter', 'Collaborateur', 'Collaboratore', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_STAMMDATEN_SPRACHE', 'Sprache', 'Langue', 'Lingua', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_STAMMDATEN_VERFAHRENSART', 'Verfahrensart', 'Type de procédure', 'Tipo di procedura', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_STAMMDATEN_VERFAHRENSSCHRITT', 'Verfahrensschritt', 'Étape de procédure', 'Fase procedurale', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN', 'Stammdaten', 'Données de base', 'Dati di base', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_AMT_KURZBEZEICHNUNG', 'Kurzbezeichnung', 'Abréviation', 'Abbreviazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_AMT_BEZEICHNUNG', 'Bezeichnung', 'Désignation', 'Denominazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_AMT_DIENSTSTELLENNUMMER', 'Dienststellennummer', 'Numéro de service', 'Numero di servizio', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_AMT_DEPARTEMENT', 'Departement', 'Département', 'Dipartimento', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_AMT_BEARBEITEN', 'Bearbeiten', 'Modifier', 'Modificare', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_AMT_HINZUFUEGEN', 'Hinzufügen', 'Ajouter', 'Aggiungere', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_AMT_AKTIV', 'Aktiv', 'Actif', 'Attivo', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_ANBIETER_ORTSCHAFT', 'Ortschaft', 'Localité', 'Località', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_ANBIETER_NAME', 'Name', 'Nom', 'Nome', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_ANBIETER_ADRESSE', 'Adresszeile 1', 'Adresse ligne 1', 'Indirizzo riga 1', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_ANBIETER_ADRESSE2', 'Adresszeile 2', 'Adresse ligne 2', 'Indirizzo riga 2', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_ANBIETER_ADRESSE3', 'Adresszeile 3', 'Adresse ligne 3', 'Indirizzo riga 3', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_ANBIETER_BEARBEITEN', 'Bearbeiten', 'Modifier', 'Modificare', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_ANBIETER_HINZUFUEGEN', 'Hinzufügen', 'Ajouter', 'Aggiungere', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_ANBIETER_AKTIV', 'Aktiv', 'Actif', 'Attivo', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_BKPNUMMER_BEZEICHNUNG', 'Bezeichnung', 'Désignation', 'Denominazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_BKPNUMMER_NUMMER', 'Nummer', 'Numéro', 'Numero', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_BKPNUMMER_NUMMERSTR', 'Nummer formatiert', 'Numéro formaté', 'Numero formattato', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_BKPNUMMER_BEARBEITEN', 'Bearbeiten', 'Modifier', 'Modificare', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_BKPNUMMER_HINZUFUEGEN', 'Hinzufügen', 'Ajouter', 'Aggiungere', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_BKPNUMMER_AKTIV', 'Aktiv', 'Actif', 'Attivo', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_CPVNUMMER_BEZEICHNUNG', 'Bezeichnung', 'Désignation', 'Denominazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_CPVNUMMER_NUMMER', 'Nummer', 'Numéro', 'Numero', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_CPVNUMMER_NUMMERSTR', 'Nummer formatiert', 'Numéro formaté', 'Numero formattato', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_CPVNUMMER_BEARBEITEN', 'Bearbeiten', 'Modifier', 'Modificare', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_CPVNUMMER_HINZUFUEGEN', 'Hinzufügen', 'Ajouter', 'Aggiungere', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_CPVNUMMER_AKTIV', 'Aktiv', 'Actif', 'Attivo', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_DEPARTEMENT_KURZBEZEICHNUNG', 'Kurzbezeichnung', 'Abréviation', 'Abbreviazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_DEPARTEMENT_BEZEICHNUNG', 'Bezeichnung', 'Désignation', 'Denominazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_DEPARTEMENT_BEARBEITEN', 'Bearbeiten', 'Modifier', 'Modificare', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_DEPARTEMENT_HINZUFUEGEN', 'Hinzufügen', 'Ajouter', 'Aggiungere', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_DEPARTEMENT_AKTIV', 'Aktiv', 'Actif', 'Attivo', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_DOKUMENTART_BEZEICHNUNG', 'Bezeichnung', 'Désignation', 'Denominazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_DOKUMENTART_SORTIERUNG', 'Sortierung', 'Tri', 'Ordinamento', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_DOKUMENTART_BEARBEITEN', 'Bearbeiten', 'Modifier', 'Modificare', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_DOKUMENTART_HINZUFUEGEN', 'Hinzufügen', 'Ajouter', 'Aggiungere', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_DOKUMENTART_AKTIV', 'Aktiv', 'Actif', 'Attivo', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_KANTON_KURZBEZEICHNUNG', 'Kurzbezeichnung', 'Abréviation', 'Abbreviazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_KANTON_BEZEICHNUNG', 'Bezeichnung', 'Désignation', 'Denominazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_KANTON_SPRACHE', 'Sprache', 'Langue', 'Lingua', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_KANTON_BEARBEITEN', 'Bearbeiten', 'Modifier', 'Modificare', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_KANTON_HINZUFUEGEN', 'Hinzufügen', 'Ajouter', 'Aggiungere', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_KANTON_AKTIV', 'Aktiv', 'Actif', 'Attivo', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_LAND_KURZBEZEICHNUNG', 'Kurzbezeichnung', 'Abréviation', 'Abbreviazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_LAND_BEZEICHNUNG', 'Bezeichnung', 'Désignation', 'Denominazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_LAND_BEARBEITEN', 'Bearbeiten', 'Modifier', 'Modificare', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_LAND_HINZUFUEGEN', 'Hinzufügen', 'Ajouter', 'Aggiungere', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_LAND_AKTIV', 'Aktiv', 'Actif', 'Attivo', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_LEISTUNGSART_KURZBEZEICHNUNG', 'Kurzbezeichnung', 'Abréviation', 'Abbreviazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_LEISTUNGSART_BEZEICHNUNG', 'Bezeichnung', 'Désignation', 'Denominazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_LEISTUNGSART_PROJEKTART', 'Projektart', 'Type de projet', 'Tipo di progetto', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_LEISTUNGSART_BEARBEITEN', 'Bearbeiten', 'Modifier', 'Modificare', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_LEISTUNGSART_HINZUFUEGEN', 'Hinzufügen', 'Ajouter', 'Aggiungere', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_LEISTUNGSART_AKTIV', 'Aktiv', 'Actif', 'Attivo', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_OFFERTEZUSTAND_KURZBEZEICHNUNG', 'Kurzbezeichnung', 'Abréviation', 'Abbreviazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_OFFERTEZUSTAND_BEZEICHNUNG', 'Bezeichnung', 'Désignation', 'Denominazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_OFFERTEZUSTAND_SCHLUSSZUSTAND', 'Schlusszustand', 'État final', 'Stato finale', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_OFFERTEZUSTAND_BEARBEITEN', 'Bearbeiten', 'Modifier', 'Modificare', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_OFFERTEZUSTAND_HINZUFUEGEN', 'Hinzufügen', 'Ajouter', 'Aggiungere', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_OFFERTEZUSTAND_AKTIV', 'Aktiv', 'Actif', 'Attivo', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_ORGANISATIONSZUGEHOERIGKEIT_KURZBEZEICHNUNG', 'Kurzbezeichnung', 'Abréviation', 'Abbreviazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_ORGANISATIONSZUGEHOERIGKEIT_BEZEICHNUNG', 'Bezeichnung', 'Désignation', 'Denominazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_ORGANISATIONSZUGEHOERIGKEIT_BEARBEITEN', 'Bearbeiten', 'Modifier', 'Modificare', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_ORGANISATIONSZUGEHOERIGKEIT_HINZUFUEGEN', 'Hinzufügen', 'Ajouter', 'Aggiungere', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_ORGANISATIONSZUGEHOERIGKEIT_AKTIV', 'Aktiv', 'Actif', 'Attivo', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_ORTSCHAFT_ORT', 'Ort', 'Lieu', 'Luogo', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_ORTSCHAFT_PLZ', 'Postleitzahl', 'Code postal', 'Codice postale', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_ORTSCHAFT_LAND', 'Land', 'Pays', 'Paese', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_ORTSCHAFT_KANTON', 'Kanton', 'Canton', 'Cantone', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_ORTSCHAFT_BEARBEITEN', 'Bearbeiten', 'Modifier', 'Modificare', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_ORTSCHAFT_HINZUFUEGEN', 'Hinzufügen', 'Ajouter', 'Aggiungere', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_ORTSCHAFT_AKTIV', 'Aktiv', 'Actif', 'Attivo', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_PROJEKTART_KURZBEZEICHNUNG', 'Kurzbezeichnung', 'Abréviation', 'Abbreviazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_PROJEKTART_BEZEICHNUNG', 'Bezeichnung', 'Désignation', 'Denominazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_PROJEKTART_BEARBEITEN', 'Bearbeiten', 'Modifier', 'Modificare', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_PROJEKTART_HINZUFUEGEN', 'Hinzufügen', 'Ajouter', 'Aggiungere', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_PROJEKTART_AKTIV', 'Aktiv', 'Actif', 'Attivo', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_RESSORT_KURZBEZEICHNUNG', 'Kurzbezeichnung', 'Abréviation', 'Abbreviazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_RESSORT_BEZEICHNUNG', 'Bezeichnung', 'Désignation', 'Denominazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_RESSORT_AMT', 'Amt', 'Office', 'Ufficio', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_RESSORT_BEARBEITEN', 'Bearbeiten', 'Modifier', 'Modificare', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_RESSORT_HINZUFUEGEN', 'Hinzufügen', 'Ajouter', 'Aggiungere', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_RESSORT_AKTIV', 'Aktiv', 'Actif', 'Attivo', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_ROLLEINAUSSCHREIBUNG_KURZBEZEICHNUNG', 'Kurzbezeichnung', 'Abréviation', 'Abbreviazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_ROLLEINAUSSCHREIBUNG_BEZEICHNUNG', 'Bezeichnung', 'Désignation', 'Denominazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_ROLLEINAUSSCHREIBUNG_BEARBEITEN', 'Bearbeiten', 'Modifier', 'Modificare', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_ROLLEINAUSSCHREIBUNG_HINZUFUEGEN', 'Hinzufügen', 'Ajouter', 'Aggiungere', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_ROLLEINAUSSCHREIBUNG_AKTIV', 'Aktiv', 'Actif', 'Attivo', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_SACHBEARBEITER_KURZBEZEICHNUNG', 'Kürzel', 'Sigle', 'Sigla', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_SACHBEARBEITER_NACHNAME', 'Name', 'Nom', 'Cognome', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_SACHBEARBEITER_VORNAME', 'Vorname', 'Prénom', 'Nome', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_SACHBEARBEITER_ORGANISATIONSZUGEHOERIGKEIT', 'Organisation', 'Organisation', 'Organizzazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_SACHBEARBEITER_BEARBEITEN', 'Bearbeiten', 'Modifier', 'Modificare', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_SACHBEARBEITER_HINZUFUEGEN', 'Hinzufügen', 'Ajouter', 'Aggiungere', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_SACHBEARBEITER_AKTIV', 'Aktiv', 'Actif', 'Attivo', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_SPRACHE_CULTURE', 'Culture', 'Culture', 'Culture', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_SPRACHE_BEZEICHNUNG', 'Bezeichnung', 'Désignation', 'Denominazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_SPRACHE_SORTIERUNG', 'Sortierung', 'Tri', 'Ordinamento', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_SPRACHE_BEARBEITEN', 'Bearbeiten', 'Modifier', 'Modificare', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_SPRACHE_HINZUFUEGEN', 'Hinzufügen', 'Ajouter', 'Aggiungere', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_SPRACHE_AKTIV', 'Aktiv', 'Actif', 'Attivo', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_VERFAHRENSART_KURZBEZEICHNUNG', 'Kurzbezeichnung', 'Abréviation', 'Abbreviazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_VERFAHRENSART_BEZEICHNUNG', 'Bezeichnung', 'Désignation', 'Denominazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_VERFAHRENSART_SORTIERUNG', 'Sortierung', 'Tri', 'Ordinamento', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_VERFAHRENSART_BEARBEITEN', 'Bearbeiten', 'Modifier', 'Modificare', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_VERFAHRENSART_HINZUFUEGEN', 'Hinzufügen', 'Ajouter', 'Aggiungere', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_VERFAHRENSART_AKTIV', 'Aktiv', 'Actif', 'Attivo', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_VERFAHRENSSCHRITT_BEZEICHNUNG', 'Bezeichnung', 'Désignation', 'Denominazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_VERFAHRENSSCHRITT_SORTIERUNG', 'Sortierung', 'Tri', 'Ordinamento', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_VERFAHRENSSCHRITT_NURWERKTAGE', 'Nur Werktage', 'Jours ouvrables uniquement', 'Solo giorni lavorativi', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_VERFAHRENSSCHRITT_OFFEN', 'Offen', 'Ouvert', 'Aperto', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_VERFAHRENSSCHRITT_SELEKTIV', 'Selektiv', 'Sélectif', 'Selettivo', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_VERFAHRENSSCHRITT_BEARBEITEN', 'Bearbeiten', 'Modifier', 'Modificare', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_VERFAHRENSSCHRITT_HINZUFUEGEN', 'Hinzufügen', 'Ajouter', 'Aggiungere', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_VERFAHRENSSCHRITT_AKTIV', 'Aktiv', 'Actif', 'Attivo', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWAEHLEN', 'Auswählen', 'Sélectionner', 'Selezionare', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'DROPDOWN_DEFAULT', 'Auswählen', 'Sélectionner', 'Selezionare', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'BUTTON_JA', 'Ja', 'Oui', 'Sì', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'BUTTON_NEIN', 'Nein', 'Non', 'No', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'BUTTON_SCHLIESSEN', 'Schliessen', 'Fermer', 'Chiudere', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'BUTTON_SPEICHERN', 'Speichern', 'Enregistrer', 'Salvare', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'BUTTON_NEU', 'Neu', 'Nouveau', 'Nuovo', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'BUTTON_SUCHEN', 'Suchen', 'Rechercher', 'Cercare', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'BUTTON_ABBRECHEN', 'Abbrechen', 'Annuler', 'Annullare', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'BUTTON_HINZUFUEGEN', 'Hinzufügen', 'Ajouter', 'Aggiungere', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'BUTTON_KOPIEREN', 'Kopieren', 'Copier', 'Copiare', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'BUTTON_TITEL_KOPIEREN_ZWISCHENABLAGE', 'In Zwischenablage kopieren', 'Copier dans le presse-papiers', 'Copiare negli appunti', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'BUTTON_LOESCHEN', 'Löschen', 'Supprimer', 'Eliminare', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'WINDOW_SCHLIESSEN_TITEL', 'Bestätigung', 'Confirmation', 'Conferma', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'WINDOW_SCHLIESSEN', 'Möchten Sie Ihre Änderungen wirklich verwerfen?', 'Voulez-vous vraiment annuler vos modifications ?', 'Vuole davvero annullare le modifiche?', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'WINDOW_LOESCHEN', 'Möchten Sie das Objekt wirklich löschen?', 'Voulez-vous vraiment supprimer cet objet ?', 'Vuole davvero eliminare l''oggetto?', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'WINDOW_LOESCHEN_TITEL', 'Bestätigung', 'Confirmation', 'Conferma', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'SEARCHBOX_NURAKTIVE', 'Nur Aktive', 'Uniquement actifs', 'Solo attivi', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN', 'Ausschreibungen', 'Appels d''offres', 'Bandi di concorso', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_FILTER_LOESCHEN', 'Zurücksetzen', 'Réinitialiser', 'Reimposta', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_FILTER_LOESCHEN_TOOLTIP', 'Jahr, Suchbegriff und Filter zurücksetzen und das Ergebnis leeren.', 'Réinitialiser l''année, le terme de recherche et les filtres et vider le résultat.', 'Reimpostare anno, termine di ricerca e filtri e svuotare il risultato.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_BEZEICHNUNG', 'Bezeichnung', 'Désignation', 'Denominazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_ART', 'Art', 'Type', 'Tipo', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_JAHR', 'Jahr', 'Année', 'Anno', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_ALLE_JAHRE', 'Alle Jahre', 'Toutes les années', 'Tutti gli anni', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_PUBLIKATION_VON', 'Publikation von', 'Publication du', 'Pubblicazione dal', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_PUBLIKATION_BIS', 'Publikation bis', 'Publication au', 'Pubblicazione al', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_NUMMER', 'Nr.', 'N°', 'N.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_DIENSTSTELLENNUMMER', 'DST.', 'N° serv.', 'N. serv.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_ROLLE', 'Rolle', 'Rôle', 'Ruolo', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_SACHBEARBEITER', 'SB', 'Collab.', 'Collab.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_VERFAHRENSART', 'Verf.', 'Proc.', 'Proc.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_LEISTUNGSART', 'Gut', 'Bien', 'Bene', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_PERSONALVERLEIH', 'PV', 'LP', 'LP', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_PUBLIZIERT', 'Publiziert', 'Publié', 'Pubblicato', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_ABGESCHLOSSEN', 'Abg.', 'Term.', 'Conc.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_ABGESCHLOSSEN_TOOLTIP', 'Abgeschlossen', 'Terminé', 'Concluso', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_ABGEBROCHEN', 'Abbr.', 'Interr.', 'Interr.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_ABGEBROCHEN_TOOLTIP', 'Abgebrochen', 'Interrompu', 'Interrotto', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_WIDERRUFEN', 'Wid.', 'Rév.', 'Rev.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_WIDERRUFEN_TOOLTIP', 'Widerrufen', 'Révoqué', 'Revocato', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_WINDOW_NEU_TITEL', 'Neue Ausschreibung', 'Nouvel appel d''offres', 'Nuovo bando di concorso', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_WINDOW_NEU_BUTTON_ERSTELLEN', 'Erstellen', 'Créer', 'Creare', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_AUSSCHREIBUNG', 'Ausschreibung', 'Appels d''offres', 'Bandi di concorso', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_ZURUECK_ZUR_SUCHE', 'Zurück zur Suche', 'Retour à la recherche', 'Torna alla ricerca', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_JAHR', 'Jahr', 'Année', 'Anno', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_NUMMER', 'Nummer', 'Numéro', 'Numero', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_BEZEICHNUNG', 'Bezeichnung', 'Désignation', 'Denominazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_PROJEKTART', 'Projektart', 'Type de projet', 'Tipo di progetto', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_VERFAHRENSART', 'Verfahrensart', 'Type de procédure', 'Tipo di procedura', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_VERFAHRENSART_AENDERN_TITEL', 'Verfahrensart ändern', 'Modifier le type de procédure', 'Modifica del tipo di procedura', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_VERFAHRENSART_AENDERN_CONFIRM', 'Möchten Sie die Verfahrensart dieser bestehenden Ausschreibung wirklich ändern?', 'Voulez-vous vraiment modifier le type de procédure de cet appel d''offres existant ?', 'Vuole davvero modificare il tipo di procedura di questo bando esistente?', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_ROLLEBBL', 'Rolle BBL', 'Rôle OFCL', 'Ruolo UFCL', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_LEISTUNGSART', 'Leistungsart', 'Type de prestation', 'Tipo di prestazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_ABSCHLUSSJAHR', 'Abschlussjahr', 'Année de clôture', 'Anno di chiusura', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_AMT', 'Amt', 'Office', 'Ufficio', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_RESSORT', 'Ressort', 'Ressort', 'Settore', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_SIMAPNR', 'SIMAP Nr.', 'N° SIMAP', 'N. SIMAP', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_ROLLEINAUSSCHREIBUNG', 'Rolle BBL', 'Rôle OFCL', 'Ruolo UFCL', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_SACHBEARBEITER_DOEA', 'DöA', 'SMP', 'SMP', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_SACHBEARBEITER_RD', 'RD', 'DJ', 'DJ', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_SACHBEARBEITER_BBL', 'BBL', 'OFCL', 'UFCL', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_EXTERN', 'Extern', 'Externe', 'Esterno', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_VERANTWORTLICHE_PERSONEN', 'Verantwortliche Personen', 'Personnes responsables', 'Persone responsabili', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_PUBLIKATION', 'Publikation', 'Publication', 'Pubblicazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_EINGEREICHT', 'Eingereicht', 'Soumis', 'Presentato', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_VERZOEGERUNGDURCHAUFTRAGGEBER', 'Verzögerung durch Auftraggeber', 'Retard imputable au donneur d''ordre', 'Ritardo causato dal committente', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_EINGABEFRIST', 'Eingabefrist', 'Délai de soumission', 'Termine di presentazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_STANDORT', 'Standort', 'Emplacement', 'Ubicazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_PERSONALVERLEIH', 'Personalverleih', 'Location de personnel', 'Prestito di personale', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_BESCHAFFUNGDEZENTRAL', 'Beschaffung dezentral', 'Acquisition décentralisée', 'Acquisizione decentralizzata', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_CPVNUMMER_HINZUFUEGEN', 'Hinzufügen', 'Ajouter', 'Aggiungere', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_BEMERKUNGEN', 'Bemerkung', 'Remarque', 'Osservazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_CPVNUMMERN', 'CPV Nummer', 'Numéro CPV', 'Numero CPV', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_CPVNUMMER_AUSWAEHLEN', 'CPV Nummer auswählen', 'Sélectionner le numéro CPV', 'Selezionare il numero CPV', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_HAUPTPROJEKT', 'Hauptprojekt', 'Projet principal', 'Progetto principale', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_HAUPTPROJEKT_AUSWAEHLEN', 'Hauptprojekt auswählen', 'Sélectionner le projet principal', 'Selezionare il progetto principale', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_BEREICH_AUSSCHREIBUNG', 'Ausschreibung', 'Appel d''offres', 'Bando di concorso', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_PUBLIKATION_PH1', 'Publikation PH 1', 'Publication PH 1', 'Pubblicazione PH 1', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_EINGABEFRISTTEILNAHME', 'Eingabefrist Teilnahmeantrag', 'Délai de soumission demande de participation', 'Termine di presentazione domanda di partecipazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_PFLICHTENHEFTERPRAEQUALI', 'Anzahl Präqualifikation Pflichtenhefter', 'Nombre de cahiers des charges préqualification', 'Numero di capitolati prequalifica', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_PFLICHTENHEFTER', 'Anz. Pflichtenhefter', 'Nb cahiers des charges', 'N. capitolati d''oneri', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_PUBLIKATIONPRAEQUALI', 'Publikation Präqualifikation', 'Publication préqualification', 'Pubblicazione prequalifica', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_ANZAHLTEILNEHMER', 'Anzahl Teilnehmer', 'Nombre de participants', 'Numero di partecipanti', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_VERSANDPH2', 'Versand Phase 2', 'Envoi phase 2', 'Invio fase 2', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_EINGEREICHTPRAEQUALI', 'Eingereicht Präqualifikation', 'Soumis préqualification', 'Presentato prequalifica', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_PUBLIKATION_PHASE1', 'Publikation Phase 1', 'Publication phase 1', 'Pubblicazione fase 1', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_OFFERTENFRIST', 'Eingabefrist Offerten', 'Délai de soumission offres', 'Termine di presentazione offerte', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_BEREICH_PUBLIKATION', 'Publikation', 'Publication', 'Pubblicazione', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_TERMINPLANUNG_VERFAHRENSSCHRITT', 'Verfahrensschritt', 'Étape de procédure', 'Fase procedurale', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_TERMINPLANUNG_DATUMVON', 'Datum von/am', 'Date du/le', 'Data dal/il', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_TERMINPLANUNG_DATUMBIS', 'Datum bis', 'Date au', 'Data al', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_TERMINPLANUNG_DAUER', 'Dauer', 'Durée', 'Durata', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_TERMINPLANUNG_HINZUFUEGEN', 'Termin hinzufügen', 'Ajouter une échéance', 'Aggiungere una scadenza', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_LOS', 'Lose', 'Lots', 'Lotti', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_LOS_BEZEICHNUNG', 'Bezeichnung', 'Désignation', 'Denominazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_LOS_BEZEICHNUNG_EINGEBEN', 'Bezeichnung eingeben', 'Saisir la désignation', 'Inserire la denominazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_LOS_ZUSTAND', 'Zustand', 'État', 'Stato', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_LOS_ABBRUCHDATUM', 'Abbruchdatum', 'Date d''interruption', 'Data di interruzione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_LOS_OFFERTEN', 'Offerten', 'Offres', 'Offerte', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_LOS_BKPNUMMERN', 'BKP Nummer', 'Numéro CFC', 'Numero CCC', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_LOS_BKPNUMMER_HINZUFUEGEN', 'Hinzufügen', 'Ajouter', 'Aggiungere', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_LOS_BKPNUMMER_AUSWAEHLEN', 'BKP Nummer auswählen', 'Sélectionner le numéro CFC', 'Selezionare il numero CCC', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_OFFERTEN', 'Offerten', 'Offres', 'Offerte', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_OFFERTE_ANBIETER', 'Anbieter', 'Fournisseur', 'Fornitore', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_OFFERTE_ZUSCHLAGSWERT', 'Zuschlagswert (CHF)', 'Valeur d''adjudication (CHF)', 'Valore di aggiudicazione (CHF)', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_OFFERTE_ANZAHLMITARBEITER', 'Anzahl Mitarbeiter', 'Nombre de collaborateurs', 'Numero di collaboratori', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_OFFERTE_BEMERKUNG', 'Bemerkung', 'Remarque', 'Osservazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_OFFERTE_ZUSTAND', 'Zustand', 'État', 'Stato', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_OFFERTE_ZUSTAENDE', 'Zustände', 'États', 'Stati', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_LOS_HINZUFUEGEN', 'Los hinzufügen', 'Ajouter un lot', 'Aggiungere un lotto', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_LOS_ABSCHLIESSEN', 'Los abschliessen', 'Clôturer le lot', 'Chiudere il lotto', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_OFFERTE_HINZUFUEGEN', 'Offerte hinzufügen', 'Ajouter une offre', 'Aggiungere un''offerta', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_OFFERTE_ZUSTAND_HINZUFUEGEN', 'Zustand hinzufügen', 'Ajouter un état', 'Aggiungere uno stato', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_OFFERTE_ZUSTAND_DATUM', 'Datum', 'Date', 'Data', v_relid);




    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_MENU_GRUNDDATEN', 'Grunddaten', 'Données de base', 'Dati di base', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_MENU_TERMINPLANUNG', 'Terminplanung', 'Planification', 'Pianificazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_MENU_LOSEUNDOFFERTEN', 'Lose und Offerten', 'Lots et offres', 'Lotti e offerte', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_MENU_ZUSCHLAGSEMPFAENGER', 'Zuschlagsempfänger', 'Adjudicataire', 'Aggiudicatario', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_OFFERTE_FREIHAENDIG_NUR_EINE', 'Bei Verfahrensart «Freihändig» ist pro Zuschlagsempfänger nur eine Offerte erlaubt.', 'Pour la procédure « de gré à gré », une seule offre est autorisée par adjudicataire.', 'Per la procedura «per incarico diretto» è ammessa una sola offerta per aggiudicatario.', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_EINSTELLUNGEN', 'Einstellungen', 'Paramètres', 'Impostazioni', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_EINSTELLUNGEN_LOG', 'Log', 'Journal', 'Log', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_EINSTELLUNGEN_KONFIGURATION', 'Konfiguration', 'Configuration', 'Configurazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_EINSTELLUNGEN_TRANSLATIONS', 'Translations', 'Traductions', 'Traduzioni', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_KONFIGURATION', 'Konfiguration', 'Configuration', 'Configurazione', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_TRANSLATIONS', 'Translations', 'Traductions', 'Traduzioni', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_TRANSLATIONS_KEY', 'Translation Key', 'Clé de traduction', 'Chiave di traduzione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_TRANSLATIONS_DEUTSCH', 'Deutsch', 'Allemand', 'Tedesco', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_TRANSLATIONS_FRANZOESISCH', 'Französisch', 'Français', 'Francese', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_TRANSLATIONS_ITALIENISCH', 'Italienisch', 'Italien', 'Italiano', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_TRANSLATIONS_VERSION', 'Version', 'Version', 'Versione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_TRANSLATIONS_BEARBEITEN', 'Translation bearbeiten', 'Modifier la traduction', 'Modifica traduzione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'VALIDATOR_MAXLENGTH', 'Max. {0} Zeichen erlaubt', 'Max. {0} caractères autorisés', 'Max. {0} caratteri consentiti', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'VALIDATOR_MINLENGTH', 'Mind. {0} Zeichen erforderlich', 'Min. {0} caractères requis', 'Min. {0} caratteri richiesti', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'VALIDATOR_NOTEMPTY', 'Die Eigenschaft darf nicht leer sein', 'La propriété ne peut pas être vide', 'La proprietà non può essere vuota', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'VALIDATOR_BERICHTIGUNG_MINDESTENS_EIN_DATUM', 'Mindestens eines der beiden Daten (eingereicht/publiziert) muss erfasst sein', 'Au moins une des deux dates (soumise/publiée) doit être saisie', 'Almeno una delle due date (presentata/pubblicata) deve essere inserita', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'VALIDATOR_PUBLIKATION_VOR_ZUGESTELLT', 'Die Publikation darf nicht vor dem Datum «an DöA zugestellt» liegen.', 'La publication ne peut pas être antérieure à la date «remis au SMP».', 'La pubblicazione non può essere anteriore alla data «consegnato al SMP».', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'VALIDATOR_VERFAHRENSSCHRITT_BEREITS_VORHANDEN', 'Dieser Verfahrensschritt ist in der Terminplanung bereits vorhanden.', 'Cette étape de procédure figure déjà dans la planification.', 'Questa fase procedurale è già presente nella pianificazione.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'VALIDATOR_LESS_THAN', 'Die Eigenschaft muss kleiner sein als {0}', 'La propriété doit être inférieure à {0}', 'La proprietà deve essere inferiore a {0}', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'VALIDATOR_LESS_THAN_OR_EQUAL', 'Die Eigenschaft muss kleiner oder gleich {0} sein', 'La propriété doit être inférieure ou égale à {0}', 'La proprietà deve essere inferiore o uguale a {0}', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'VALIDATOR_DATE_INVALID', 'Datum ist nicht valid', 'La date n''est pas valide', 'La data non è valida', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'VALIDATOR_GREATER_THAN', 'Die Eigenschaft muss grösser sein als {0}', 'La propriété doit être supérieure à {0}', 'La proprietà deve essere superiore a {0}', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'VALIDATOR_GREATER_THAN_OR_EQUAL', 'Die Eigenschaft muss grösser oder gleich {0} sein', 'La propriété doit être supérieure ou égale à {0}', 'La proprietà deve essere superiore o uguale a {0}', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'VALIDATOR_DATE_CONFLICT_TO_OLD', 'Das Feld {0} liegt zu weit in der Vergangenheit.', 'Le champ {0} se situe trop loin dans le passé.', 'Il campo {0} è troppo lontano nel passato.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'VALIDATOR_DATE_CONFLICT_TO_YOUNG', 'Das Feld {0} liegt in der Zukunft.', 'Le champ {0} se situe dans le futur.', 'Il campo {0} è nel futuro.', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_LOG', 'Log', 'Journal', 'Log', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_LOG_CLEARBUTTON', 'Log leeren', 'Vider le journal', 'Svuotare il log', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_LOG_CURRENTLEVEL', 'Log-Stufe', 'Niveau de log', 'Livello di log', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_LOG_DELETE_CONFIRM', 'Wollen Sie wirklich alle Logeinträge in der Datenbank löschen, die vor der Aufbewahrungsfrist (siehe Konfiguration) liegen?', 'Voulez-vous vraiment supprimer toutes les entrées du journal antérieures à la période de conservation (voir configuration) ?', 'Vuole davvero eliminare tutte le voci di log precedenti al periodo di conservazione (vedi configurazione)?', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_LOG_EXCEPTION_TITLE', 'Exception', 'Exception', 'Eccezione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'ERROR_ITEM_WAS_NULL', 'Das zurückgegebene Item war "null"', 'L''élément retourné était "null"', 'L''elemento restituito era "null"', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'ERROR_UNHANDLED_EXCEPTION', 'Es ist ein unbekannter Fehler aufgetreten', 'Une erreur inconnue s''est produite', 'Si è verificato un errore sconosciuto', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'ERROR_DUPLICATE_KEY', 'Der Eintrag existiert bereits (doppelter Wert)', 'L''entrée existe déjà (valeur en double)', 'La voce esiste già (valore duplicato)', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'ERROR_NOT_FOUND', 'Der gesuchte Datensatz konnte nicht gefunden werden. Die Daten wurden eventuell unterdessen von jemanden anderes gelöscht. Ihre Änderungen können nicht mehr vorgenommen werden.', 'L''enregistrement recherché n''a pas pu être trouvé. Les données ont peut-être été supprimées entre-temps par quelqu''un d''autre.', 'Il record cercato non è stato trovato. I dati potrebbero essere stati nel frattempo eliminati da qualcun altro.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'ERROR_HANDLED_EXCEPTION', 'Beim Ausführen der Aktion ist ein Fehler aufgetreten. Details stehen im Log.', 'Une erreur est survenue lors de l''exécution de l''action. Les détails figurent dans le journal.', 'Si è verificato un errore durante l''esecuzione dell''azione. I dettagli sono riportati nel log.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'ERROR_CONCURRENCY_EXCEPTION', 'Der Datensatz wurde zwischenzeitlich von einer anderen Person geändert. Laden Sie die Daten neu und wiederholen Sie Ihre Änderung.', 'L''enregistrement a été modifié entre-temps par une autre personne. Rechargez les données et répétez votre modification.', 'Il record è stato modificato nel frattempo da un''altra persona. Ricaricare i dati e ripetere la modifica.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'ERROR_NULL_REFERENCE', 'Der Datensatz wurde nicht gefunden. Möglicherweise wurde er zwischenzeitlich gelöscht.', 'L''enregistrement est introuvable. Il a peut-être été supprimé entre-temps.', 'Il record non è stato trovato. Potrebbe essere stato eliminato nel frattempo.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'ERROR_VALIDATION', 'Die Eingaben sind ungültig: {0}', 'Les saisies ne sont pas valables : {0}', 'I dati inseriti non sono validi: {0}', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'ERROR_DOC_FILL_TABLE', 'Die Tabelle «{0}» wurde in der Word-Vorlage nicht gefunden.', 'Le tableau « {0} » est introuvable dans le modèle Word.', 'La tabella «{0}» non è stata trovata nel modello Word.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'ERROR_DOC_FILL_TABLE_COLUMNS', 'Die Tabelle «{0}» in der Word-Vorlage hat nicht die erwartete Spaltenanzahl.', 'Le tableau « {0} » du modèle Word n''a pas le nombre de colonnes attendu.', 'La tabella «{0}» nel modello Word non ha il numero di colonne previsto.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'ERROR_DOC_FILL_TABLE_ROWS', 'Die Tabelle «{0}» in der Word-Vorlage hat nicht die erwartete Zeilenanzahl.', 'Le tableau « {0} » du modèle Word n''a pas le nombre de lignes attendu.', 'La tabella «{0}» nel modello Word non ha il numero di righe previsto.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'ERROR', 'Error', 'Erreur', 'Errore', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'ERROR_AUSSCHREIBUNG_HAT_UNTERPROJEKTE', 'Diese Ausschreibung kann nicht gelöscht werden, da ihr Unterprojekte zugewiesen sind. Entfernen Sie zuerst die Zuordnung der Unterprojekte.', 'Cet appel d''offres ne peut pas être supprimé, car des sous-projets lui sont attribués. Supprimez d''abord l''attribution des sous-projets.', 'Questo bando non può essere eliminato perché gli sono assegnati dei sottoprogetti. Rimuovere prima l''assegnazione dei sottoprogetti.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'ERROR_AUSSCHREIBUNG_PUBLIZIERT_NICHT_LOESCHBAR', 'Diese Ausschreibung kann nicht gelöscht werden, da bereits ein Publikationsdatum eingetragen ist.', 'Cet appel d''offres ne peut pas être supprimé, car une date de publication est déjà saisie.', 'Questo bando non può essere eliminato perché è già stata inserita una data di pubblicazione.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'ERROR_AUSSCHREIBUNG_KEINE_VORANKUENDIGUNG', 'Die referenzierte Ausschreibung ist keine Vorankündigung.', 'L''appel d''offres référencé n''est pas une annonce préalable.', 'Il bando di concorso referenziato non è un preavviso.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'ERROR_AUSSCHREIBUNG_LOS_MINDESTENS_EINS', 'Eine Ausschreibung muss mindestens ein Los enthalten.', 'Un appel d''offres doit contenir au moins un lot.', 'Un bando di concorso deve contenere almeno un lotto.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'ERROR_AUSSCHREIBUNG_LOS_ABSCHLIESSEN_NUR_OFFEN', 'Das Los ist nicht mehr offen und kann nicht abgeschlossen werden. Laden Sie die Daten neu.', 'Le lot n''est plus ouvert et ne peut pas être clôturé. Rechargez les données.', 'Il lotto non è più aperto e non può essere chiuso. Ricaricare i dati.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'ERROR_AUSSCHREIBUNG_OFFERTE_FREIHAENDIG_NUR_EINE', 'Bei Verfahrensart «Freihändig» ist pro Zuschlagsempfänger nur eine Offerte erlaubt.', 'Pour la procédure « de gré à gré », une seule offre est autorisée par adjudicataire.', 'Per la procedura «per incarico diretto» è ammessa una sola offerta per aggiudicatario.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'ERROR_AUSSCHREIBUNG_OFFERTE_ANBIETER_DOPPELT', 'Dieser Anbieter ist in diesem Los bereits erfasst.', 'Ce fournisseur est déjà enregistré dans ce lot.', 'Questo fornitore è già registrato in questo lotto.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'ERROR_OFFERTEZUSTAND_ABGELEHNT_FEHLT', 'Der Offertezustand «Angebot abgelehnt» fehlt in den Stammdaten. Der Vorgang wurde abgebrochen.', 'L''état d''offre « Offre rejetée » manque dans les données de base. L''opération a été annulée.', 'Lo stato dell''offerta «Offerta respinta» manca nei dati di base. L''operazione è stata annullata.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'ERROR_TERMINPLANUNG_VERFAHRENSSCHRITT_DOPPELT', 'Ein Verfahrensschritt ist in der Terminplanung mehrfach erfasst.', 'Une étape de procédure est saisie plusieurs fois dans la planification.', 'Una fase procedurale è registrata più volte nella pianificazione.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_LOESCHEN_PUBLIZIERT_TOOLTIP', 'Löschen nicht möglich: Es ist bereits ein Publikationsdatum eingetragen.', 'Suppression impossible : une date de publication est déjà saisie.', 'Eliminazione non possibile: è già stata inserita una data di pubblicazione.', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_LOG_EXPORT', 'Export in Excel', 'Exporter en Excel', 'Esportare in Excel', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_LOG_EXPORT_TOOLTIP', 'Logeinträge, die den eingetragenen Kriterien entsprechen, werden in eine Excel-Datei geschreiben und heruntergeladen.', 'Les entrées correspondant aux critères saisis sont exportées dans un fichier Excel.', 'Le voci corrispondenti ai criteri inseriti vengono esportate in un file Excel.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_LOG_FILTER_LOESCHEN', 'Filter zurücksetzen', 'Réinitialiser les filtres', 'Reimpostare i filtri', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_LOG_FILTER_LOESCHEN_TOOLTIP', 'Alle Suchkriterien auf den Standardwert zurücksetzen.', 'Réinitialiser tous les critères de recherche aux valeurs par défaut.', 'Reimpostare tutti i criteri di ricerca ai valori predefiniti.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_LOG_LEVEL_SPEICHERN_TOOLTIP', 'Die gewählten Log-Stufe abspeichern. Bis zur nächsten Änderung oder Neustart der Webanwendung wird ab dieser Stufe ins Log geschreiben.', 'Enregistrer le niveau de log sélectionné.', 'Salvare il livello di log selezionato.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_LOG_LOESCHEN_TOOLTIP', 'Die Logeinträge werden unwiederruflich aus der Datenbank gelöscht. Beim Löschen wird die Aufbewahrungsfrist in der Konfiguration berücksichtigt.', 'Les entrées du journal seront définitivement supprimées de la base de données.', 'Le voci di log verranno eliminate definitivamente dal database.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_LOG_MAXZEILEN', 'Max. Anzahl Einträge', 'Nb max. d''entrées', 'N. max. di voci', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_LOG_SUCHE', 'Suchen', 'Rechercher', 'Cercare', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_LOG_SUCHE_BIS', 'bis', 'au', 'al', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_LOG_SUCHE_LEVEL', 'Log-Stufe', 'Niveau de log', 'Livello di log', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_LOG_SUCHE_TEXT', 'Suchtext', 'Texte de recherche', 'Testo di ricerca', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_LOG_SUCHE_VON', 'Zeitspanne von', 'Période du', 'Periodo dal', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_LOG_ID', 'Primärschlüssel', 'Clé primaire', 'Chiave primaria', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_LOG_LEVEL', 'Stufe', 'Niveau', 'Livello', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_LOG_MESSAGE', 'Meldung', 'Message', 'Messaggio', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_LOG_EXCEPTION', 'Fehler', 'Erreur', 'Errore', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_LOG_TIMESTAMP', 'Zeitstempel', 'Horodatage', 'Timestamp', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_LOG_DELETE', 'Logeinträge löschen', 'Supprimer les entrées', 'Eliminare le voci', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_LOG_DELETE_TOOLTIP', 'Logeinträge aus Datenbank löschen', 'Supprimer les entrées de la base de données', 'Eliminare le voci dal database', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_KONFIGURATION_BEZEICHNUNG', 'Bezeichnung', 'Désignation', 'Denominazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_KONFIGURATION_WERT', 'Wert', 'Valeur', 'Valore', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_KONFIGURATION_BESCHREIBUNG', 'Beschreibung', 'Description', 'Descrizione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_KONFIGURATION_BEARBEITEN', 'Konfiguration bearbeiten', 'Modifier la configuration', 'Modificare la configurazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_KONFIGURATION_TYP', 'Datentyp', 'Type de données', 'Tipo di dati', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_KONFIGURATION_AKTIV', 'Aktiv', 'Actif', 'Attivo', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNGEN', 'Auswertungen', 'Évaluations', 'Valutazioni', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'SELECT_AEMTER_DEPARTEMENT', 'Departement', 'Département', 'Dipartimento', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'SELECT_AEMTER_AMTER', 'Ämter', 'Offices', 'Uffici', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ABBRUCHWIDERRUF', 'Abbrüche und Widerrufe (5 Jahresvergleich)', 'Interruptions et révocations (comparaison sur 5 ans)', 'Interruzioni e revoche (confronto su 5 anni)', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ABBRUCHWIDERRUF_JAHR', 'Jahr von', 'Année à partir de', 'Anno dal', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ABBRUCHWIDERRUF_PDF_ERSTELLEN', 'Pdf erstellen', 'Créer PDF', 'Creare PDF', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ABBRUCHWIDERRUF_EXCEL_ERSTELLEN', 'Excel erstellen', 'Créer Excel', 'Creare Excel', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ZUSCHLAEGE', 'Zuschläge (gruppiert nach Kanton oder Sprachregion)', 'Adjudications', 'Aggiudicazioni', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ZUSCHLAEGE_NACH', 'Auswerten nach', 'Évaluer par', 'Valutare per', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ZUSCHLAEGE_NACH_KANTON', 'Kanton', 'Canton', 'Cantone', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ZUSCHLAEGE_NACH_SPRACHE', 'Sprachregion', 'Région linguistique', 'Regione linguistica', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ZUSCHLAEGE_ROLLE_BBL', 'Rolle BBL', 'Rôle OFCL', 'Ruolo UFCL', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ZUSCHLAEGE_ROLLE_BBL_MELDESTELLE', 'Ämter dezentral', 'Service de notification', 'Servizio di notifica', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ZUSCHLAEGE_ROLLE_BBL_VERGABESTELLE', 'Logistik zentral', 'Service adjudicateur', 'Servizio aggiudicatore', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ZUSCHLAEGE_PROJEKTART', 'Projektart', 'Type de projet', 'Tipo di progetto', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ZUSCHLAEGE_PROJEKTART_LOGISTIK', 'Logistik', 'Logistique', 'Logistica', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ZUSCHLAEGE_PROJEKTART_BAU', 'Bau', 'Construction', 'Costruzione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ZUSCHLAEGE_VERFAHRENSART', 'Verfahrensart', 'Type de procédure', 'Tipo di procedura', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ZUSCHLAEGE_VERFAHRENSART_OFFEN', 'Offen', 'Ouvert', 'Aperto', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ZUSCHLAEGE_VERFAHRENSART_SELEKTIV', 'Selektiv', 'Sélectif', 'Selettivo', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ZUSCHLAEGE_VERFAHRENSART_FREIHAENDIG', 'Freihändig', 'De gré à gré', 'A trattativa privata', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ZUSCHLAEGE_VERFAHRENSART_VORANKUENDIGUNG', 'Vorankündigung', 'Annonce préalable', 'Annuncio preliminare', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ZUSCHLAEGE_PUBLIKATIONSDATUM', 'Publikationsdatum', 'Date de publication', 'Data di pubblicazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ZUSCHLAEGE_CPVNUMMERN', 'CPV-Nummern', 'Numéros CPV', 'Numeri CPV', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ZUSCHLAEGE_CPVNUMMERN_PLACEHOLDER', 'CPV-Nummern durch Komma getrennt, ohne Leerzeichen eintragen', 'Saisir les numéros CPV séparés par des virgules, sans espaces', 'Inserire i numeri CPV separati da virgole, senza spazi', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ZUSCHLAEGE_BKPNUMMERN', 'BKP-Nummern', 'Numéros CFC', 'Numeri CFC', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ZUSCHLAEGE_BKPNUMMERN_PLACEHOLDER', 'BKP-Nummern durch Komma getrennt, ohne Leerzeichen eintragen', 'Saisir les numéros CFC séparés par des virgules, sans espaces', 'Inserire i numeri CFC separati da virgole, senza spazi', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ZUSCHLAEGE_MITBETRAG', 'Beträge anzeigen', 'Afficher les montants', 'Mostrare gli importi', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ZIELERREICHUNG', 'Zielerreichung', 'Atteinte des objectifs', 'Raggiungimento degli obiettivi', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ZIELERREICHUNG_PUBLIKATIONSDATUM', 'Publikationsdatum', 'Date de publication', 'Data di pubblicazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ZIELERREICHUNG_PDF_ERSTELLEN', 'Pdf erstellen', 'Créer PDF', 'Creare PDF', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_AUSSCHREIBUNGEN', 'Ausschreibungen', 'Appels d''offres', 'Bandi di concorso', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_AUSSCHREIBUNGEN_LEISTUNGSART', 'Leistungsart', 'Type de prestation', 'Tipo di prestazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_AUSSCHREIBUNGEN_NACH_JAHRQUARTAL', 'Jahr/Quartal', 'Année/trimestre', 'Anno/trimestre', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_AUSSCHREIBUNGEN_NACH_SPRACHE', 'Sprache', 'Langue', 'Lingua', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_AUSSCHREIBUNGEN_PERSONALVERLEIH', 'Personalverleih', 'Location de services', 'Fornitura di personale', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_AUSSCHREIBUNGEN_BESCHAFFUNGDEZENTRAL', 'Beschaffung dezentral', 'Acquisition décentralisée', 'Acquisizione decentralizzata', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_AUSSCHREIBUNGEN_NACH', 'Auswerten nach', 'Évaluer par', 'Valutare per', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_EXPORTGRUPPEN', 'Exportgruppen', 'Groupes d''exportation', 'Gruppi di esportazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_NEU', 'Neue Exportgruppe', 'Nouveau groupe d''exportation', 'Nuovo gruppo di esportazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPEN_NAME', 'Name', 'Nom', 'Nome', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_VORANKUENDIGUNG', 'Vorankündigung', 'Annonce préalable', 'Preavviso', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_VORANKUENDIGUNG_AUSWAEHLEN', 'Vorankündigung auswählen', 'Sélectionner l''annonce préalable', 'Selezionare il preavviso', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_RFI', 'RFI', 'RFI', 'RFI', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_RFI_AUSWAEHLEN', 'RFI auswählen', 'Sélectionner la RFI', 'Selezionare la RFI', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_REFERENZIERT_VON', 'Referenziert von', 'Référencé par', 'Referenziato da', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_FOLGEAUSSCHREIBUNG_ERSTELLEN', 'Folgeausschreibung erstellen', 'Créer l''appel d''offres subséquent', 'Creare il bando successivo', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_SIMAPNR', 'SIMAP Nr.', 'N° SIMAP', 'N. SIMAP', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_RFI', 'RFI Nr.', 'N° RFI', 'N. RFI', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_VORANKUENDIGUNG', 'Vorankündigung Nr.', 'N° annonce préalable', 'N. preavviso', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_PUBLIZIERT_MONAT', 'Monat', 'Mois', 'Mese', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_PUBLIZIERT_TAG', 'Tag', 'Jour', 'Giorno', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_BETRAG_VON', 'Betrag von', 'Montant de', 'Importo da', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_BETRAG_BIS', 'Betrag bis', 'Montant à', 'Importo fino a', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_SPALTENEINSTELLUNGEN', 'Spalteneinstellungen', 'Paramètres des colonnes', 'Impostazioni colonne', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_TERMINPLANUNG_PDF_ERSTELLEN', 'Pdf erstellen', 'Créer PDF', 'Creare PDF', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_EINSTELLUNGEN_SPRACHE', 'Sprachen', 'Langues', 'Lingue', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_SPRACHE', 'Sprachen', 'Langues', 'Lingue', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_BERICHTIGUNGEINGEREICHT', 'Berichtigung eingereicht', 'Rectification soumise', 'Rettifica presentata', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_BERICHTIGUNGPUBLIZIERT', 'Berichtigung publiziert', 'Rectification publiée', 'Rettifica pubblicata', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_VERZOEGERUNGBERICHTIGUNGDURCHAUFTRAGGEBER', 'Verzögerung bei Berichtigung durch Auftraggeber', 'Retard de rectification imputable au donneur d''ordre', 'Ritardo della rettifica causato dal committente', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_BERICHTIGUNGEN', 'Berichtigungen', 'Rectifications', 'Rettifiche', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_BERICHTIGUNG_HINZUFUEGEN', 'Berichtigung hinzufügen', 'Ajouter une rectification', 'Aggiungi rettifica', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_BERICHTIGUNGBEMERKUNGEN', 'Bemerkungen', 'Remarques', 'Osservazioni', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_FREIHAENDER', 'Zuschläge Freihänder (Liste)', 'Adjudications de gré à gré', 'Aggiudicazioni a trattativa privata', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_FREIHAENDER_PUBLIKATIONSDATUM', 'Publikationsdatum', 'Date de publication', 'Data di pubblicazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_FREIHAENDER_EXCEL_ERSTELLEN', 'Excel erstellen', 'Créer Excel', 'Creare Excel', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ZUSCHLAEGE_OHNE_FREIHAENDER', 'Zuschläge ohne Freihänder (Liste)', 'Adjudications hors gré à gré', 'Aggiudicazioni senza trattativa privata', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ZUSCHLAEGE_OHNE_FREIHAENDER_PUBLIKATIONSDATUM', 'Publikationsdatum des Zuschlags', 'Date de publication de l''adjudication', 'Data di pubblicazione dell''aggiudicazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_ZUSCHLAEGE_OHNE_FREIHAENDER_EXCEL_ERSTELLEN', 'Excel erstellen', 'Créer Excel', 'Creare Excel', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_LAUFENDE_WTO', 'Laufende WTO (Liste)', 'OMC en cours', 'OMC in corso', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_LAUFENDE_WTO_PUBLIKATIONSDATUM', 'Publikationsdatum', 'Date de publication', 'Data di pubblicazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_LAUFENDE_WTO_VERFAHRENSART', 'Verfahrensart', 'Type de procédure', 'Tipo di procedura', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_LAUFENDE_WTO_VERFAHRENSART_OFFEN', 'Offen', 'Ouverte', 'Aperta', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_LAUFENDE_WTO_VERFAHRENSART_SELEKTIV', 'Selektiv', 'Sélective', 'Selettiva', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_LAUFENDE_WTO_VERFAHRENSART_EINLADUNG', 'Einladung', 'Sur invitation', 'Su invito', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_LAUFENDE_WTO_EXCEL_ERSTELLEN', 'Excel erstellen', 'Créer Excel', 'Creare Excel', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_BESCHAFFUNGSCONTROLLING', 'Beschaffungscontrolling', 'Controlling des achats', 'Controlling degli acquisti', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_BESCHAFFUNGSCONTROLLING_JAHR', 'Jahr', 'Année', 'Anno', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_BESCHAFFUNGSCONTROLLING_PDF_ERSTELLEN', 'Pdf erstellen', 'Créer PDF', 'Creare PDF', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_DATUM_VON_BIS_FEHLER', 'Das Datum «von» darf nicht nach dem Datum «bis» liegen', 'La date «de» ne doit pas être postérieure à la date «à»', 'La data «da» non deve essere successiva alla data «a»', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_AUSSCHREIBUNGEN_ROLLE_BBL', 'Rolle BBL', 'Rôle OFCL', 'Ruolo UFCL', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_AUSSCHREIBUNGEN_ROLLE_BBL_MELDESTELLE', 'Ämter dezentral', 'Service de notification', 'Servizio di notifica', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSWERTUNG_TILE_AUSSCHREIBUNGEN_ROLLE_BBL_VERGABESTELLE', 'Logistik zentral', 'Service adjudicateur', 'Servizio aggiudicatore', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_FILTER_KANTON', 'Kantone', 'Cantons', 'Cantoni', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNIS_KANTON', 'Kanton', 'Canton', 'Cantone', v_relid);

    -- Detailseite: Formularfelder
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_JAHR', 'Jahr', 'Année', 'Anno', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ABSCHLUSSJAHR', 'Abschlussjahr', 'Année de clôture', 'Anno di chiusura', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_EXTERN', 'Extern', 'Externe', 'Esterno', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_PERSONALVERLEIH', 'Personalverleih', 'Location de personnel', 'Prestito di personale', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ABGESCHLOSSEN', 'Abgeschlossen', 'Terminé', 'Concluso', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_BESCHAFFUNGDEZENTRAL', 'Beschaffung dezentral', 'Acquisition décentralisée', 'Acquisizione decentralizzata', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_PUBLIKATIONVON', 'Publikationsdatum von', 'Date de publication du', 'Data di pubblicazione dal', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_PUBLIKATIONBIS', 'Publikationsdatum bis', 'Date de publication au', 'Data di pubblicazione al', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_EGAL', 'egal', 'indifférent', 'indifferente', v_relid);

    -- Detailseite: Mehrfachauswahl-Filter
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_FILTER_AMT', 'Ämter', 'Offices', 'Uffici', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_FILTER_LEISTUNGSART', 'Leistungsarten', 'Types de prestation', 'Tipi di prestazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_FILTER_ORTSCHAFT', 'Ortschaften', 'Localités', 'Località', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_FILTER_PROJEKTART', 'Projektarten', 'Types de projet', 'Tipi di progetto', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_FILTER_RESSORT', 'Ressorts', 'Ressorts', 'Settori', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_FILTER_ROLLEINAUSSCHREIBUNG', 'Rollen BBL', 'Rôles OFCL', 'Ruoli UFCL', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_FILTER_SACHBEARBEITER', 'Sachbearbeiter', 'Collaborateurs', 'Collaboratori', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_FILTER_SACHBEARBEITERDOEA', 'Sachbearbeiter DöA', 'Collaborateurs SMP', 'Collaboratori SMP', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_FILTER_SACHBEARBEITERRD', 'Sachbearbeiter RD', 'Collaborateurs DJ', 'Collaboratori DJ', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_FILTER_SACHBEARBEITERBBL', 'Sachbearbeiter BBL', 'Collaborateurs OFCL', 'Collaboratori UFCL', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_FILTER_VERFAHRENSART', 'Verfahrensarten', 'Types de procédure', 'Tipi di procedura', v_relid);

    -- Detailseite: Buttons / Aktionen
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_DUPLIZIEREN', 'Duplizieren', 'Dupliquer', 'Duplicare', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_TEXT_DUPLIKAT', 'Kopie', 'Copie', 'Copia', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_SPALTENEINSTELLUNGEN', 'Spalteneinstellungen', 'Paramètres des colonnes', 'Impostazioni colonne', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_EXCEL_EXPORT', 'Excel erstellen', 'Créer Excel', 'Creare Excel', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISSE', 'Ergebnisse', 'Résultats', 'Risultati', v_relid);

    -- Ergebnisspalten (Schema EXPORTGRUPPE_ERGEBNIS_{PROPERTY} — wird von Grid UND
    -- ExportToExcelService (trlPraefix + Typname 'Ergebnis' + Property) identisch verwendet)
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNIS_JAHR', 'Jahr', 'Année', 'Anno', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNIS_NUMMERSTR', 'Nummer', 'Numéro', 'Numero', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNIS_BEZEICHNUNG', 'Bezeichnung', 'Désignation', 'Denominazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNIS_SIMAPNR', 'SIMAP Nr.', 'N° SIMAP', 'N. SIMAP', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNIS_AMT', 'Amt', 'Office', 'Ufficio', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNIS_RESSORT', 'Ressort', 'Ressort', 'Settore', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNIS_LEISTUNGSART', 'Leistungsart', 'Type de prestation', 'Tipo di prestazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNIS_PROJEKTART', 'Projektart', 'Type de projet', 'Tipo di progetto', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNIS_VERFAHRENSART', 'Verfahrensart', 'Type de procédure', 'Tipo di procedura', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNIS_ROLLEINAUSSCHREIBUNG', 'Rolle BBL', 'Rôle OFCL', 'Ruolo UFCL', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNIS_SACHBEARBEITERDOEA', 'Sachbearbeiter DöA', 'Collaborateur SMP', 'Collaboratore SMP', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNIS_SACHBEARBEITERRD', 'Sachbearbeiter RD', 'Collaborateur DJ', 'Collaboratore DJ', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNIS_SACHBEARBEITERBBL', 'Sachbearbeiter BBL', 'Collaborateur OFCL', 'Collaboratore UFCL', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNIS_PUBLIKATION', 'Publikation', 'Publication', 'Pubblicazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNIS_OFFERTENFRIST', 'Eingabefrist Offerten', 'Délai de soumission offres', 'Termine di presentazione offerte', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNIS_ABGESCHLOSSEN', 'Abgeschlossen', 'Terminé', 'Concluso', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNIS_EXTERN', 'Extern', 'Externe', 'Esterno', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNIS_PERSONALVERLEIH', 'Personalverleih', 'Location de personnel', 'Prestito di personale', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNIS_BESCHAFFUNGDEZENTRAL', 'Beschaffung dezentral', 'Acquisition décentralisée', 'Acquisizione decentralizzata', v_relid);

    -- Vom ExportToExcelService für bool-Spalten verwendet (Schlüssel existierten bisher nicht)
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORT_JA', 'Ja', 'Oui', 'Sì', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORT_NEIN', 'Nein', 'Non', 'No', v_relid);

    -- Seitentitel der Exportgruppen-Detailansicht (Singular)
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE', 'Exportgruppe', 'Groupe d''exportation', 'Gruppo di esportazione', v_relid);

    -- Auf-/zuklappbarer Filterbereich und Zurück-Button auf der Exportgruppen-Detailseite
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_FILTER', 'Filter', 'Filtres', 'Filtri', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ZURUECK', 'Zurück', 'Retour', 'Indietro', v_relid);

    -- Tooltip des klickbaren Titels auf der Exportgruppen-Detailseite
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_NAME_BEARBEITEN', 'Namen bearbeiten', 'Modifier le nom', 'Modificare il nome', v_relid);

    -- Ergebnisliste: Checkbox für den erweiterten Excel-Export
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ALLE_DATEN', 'Alle Daten von Ausschreibung, Los, Offerte und Anbieter exportieren', 'Toutes les données', 'Tutti i dati', v_relid);

    -- Spalten des erweiterten Exports (Schema EXPORTGRUPPE_ERGEBNISALLEDATEN_{PROPERTY} —
    -- vom ExportToExcelService aus trlPraefix + Typname 'ErgebnisAlleDaten' + Property gebildet)

    -- Ausschreibung
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_JAHR', 'Jahr', 'Année', 'Anno', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_NUMMERSTR', 'Nummer', 'Numéro', 'Numero', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_BEZEICHNUNG', 'Bezeichnung', 'Désignation', 'Denominazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_SIMAPNR', 'SIMAP Nr.', 'N° SIMAP', 'N. SIMAP', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_AMT', 'Amt', 'Office', 'Ufficio', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_RESSORT', 'Ressort', 'Ressort', 'Settore', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_LEISTUNGSART', 'Leistungsart', 'Type de prestation', 'Tipo di prestazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_PROJEKTART', 'Projektart', 'Type de projet', 'Tipo di progetto', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_VERFAHRENSART', 'Verfahrensart', 'Type de procédure', 'Tipo di procedura', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_ROLLEINAUSSCHREIBUNG', 'Rolle BBL', 'Rôle OFCL', 'Ruolo UFCL', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_HAUPTPROJEKT', 'Hauptprojekt', 'Projet principal', 'Progetto principale', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_VORANKUENDIGUNG', 'Vorankündigung', 'Annonce préalable', 'Preavviso', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_RFI', 'RFI', 'RFI', 'RFI', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_SACHBEARBEITERDOEA', 'Sachbearbeiter DöA', 'Collaborateur SMP', 'Collaboratore SMP', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_SACHBEARBEITERRD', 'Sachbearbeiter RD', 'Collaborateur DJ', 'Collaboratore DJ', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_SACHBEARBEITERBBL', 'Sachbearbeiter BBL', 'Collaborateur OFCL', 'Collaboratore UFCL', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_EXTERN', 'Extern', 'Externe', 'Esterno', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_PERSONALVERLEIH', 'Personalverleih', 'Location de personnel', 'Prestito di personale', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_BESCHAFFUNGDEZENTRAL', 'Beschaffung dezentral', 'Acquisition décentralisée', 'Acquisizione decentralizzata', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_EINGEREICHT', 'Eingereicht', 'Soumis', 'Presentato', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_PUBLIKATION', 'Publikation', 'Publication', 'Pubblicazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_VERZOEGERUNGDURCHAUFTRAGGEBER', 'Verzögerung durch Auftraggeber', 'Retard imputable au donneur d''ordre', 'Ritardo causato dal committente', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_ANZAHLTAGE', 'Anzahl Tage', 'Nombre de jours', 'Numero di giorni', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_EINGEREICHTPRAEQUALI', 'Eingereicht Präqualifikation', 'Soumis préqualification', 'Presentato prequalifica', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_PUBLIKATIONPRAEQUALI', 'Publ. Präqualifikation', 'Publ. préqualification', 'Pubbl. prequalifica', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_VERZOEGERUNGDURCHAUFTRAGGEBERPRAEQUALI', 'Verzögerung durch Auftraggeber Präqualifikation', 'Retard imputable au donneur d''ordre préqualification', 'Ritardo causato dal committente prequalifica', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_ANZAHLTAGEPRAEQUALI', 'Anzahl Tage Präqualifikation', 'Nombre de jours préqualification', 'Numero di giorni prequalifica', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_EINGABEFRISTTEILNAHME', 'Eing. Teilnahmeanträge', 'Soum. demandes participation', 'Pres. domande di partecipazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_PFLICHTENHEFTERPRAEQUALI', 'Anz. Präq. Pflichtenhefter', 'Nb cahiers charges préqual.', 'N. capitolati prequalifica', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_ANZAHLTEILNEHMER', 'Anz. Teilnehmer', 'Nb participants', 'N. partecipanti', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_VERSANDPH2', 'Versand PH 2', 'Envoi PH 2', 'Invio PH 2', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_PFLICHTENHEFTER', 'Anz. Pflichtenhefter', 'Nb cahiers des charges', 'N. capitolati d''oneri', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_OFFERTENFRIST', 'Eingabefrist Offerten', 'Délai de soumission offres', 'Termine di presentazione offerte', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_ABSCHLUSSJAHR', 'Abschlussjahr', 'Année de clôture', 'Anno di chiusura', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_ABGESCHLOSSEN', 'Abgeschlossen', 'Terminé', 'Concluso', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_STANDORT', 'Standort', 'Emplacement', 'Ubicazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_BEMERKUNG', 'Bemerkung', 'Remarque', 'Osservazione', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_AKTIV', 'Aktiv', 'Actif', 'Attivo', v_relid);

    -- Los
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_LOSBEZEICHNUNG', 'Los Bezeichnung', 'Désignation du lot', 'Denominazione del lotto', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_LOSZUSTAND', 'Los Zustand', 'État du lot', 'Stato del lotto', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_LOSABBRUCHDATUM', 'Los Abbruchdatum', 'Date d''interruption du lot', 'Data di interruzione del lotto', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_LOSEINGEREICHT', 'Los Abbruch Eingereicht', 'Soumis interruption du lot', 'Presentato interruzione del lotto', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_LOSPUBLIKATION', 'Los Abbruch Publikation', 'Publication de l''interruption du lot', 'Pubblicazione dell''interruzione del lotto', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_LOSVERZOEGERUNGDURCHAUFTRAGGEBER', 'Los Abbruch Verzögerung durch Auftraggeber', 'Retard de l''interruption du lot imputable au donneur d''ordre', 'Ritardo dell''interruzione del lotto causato dal committente', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_LOSANZAHLTAGE', 'Los Abbruch Anzahl Tage', 'Interruption du lot nombre de jours', 'Interruzione del lotto numero di giorni', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_LOSAKTIV', 'Los aktiv', 'Lot actif', 'Lotto attivo', v_relid);

    -- Offerte
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_OFFERTEZUSCHLAGSWERTCHF', 'Zuschlagswert (CHF)', 'Valeur d''adjudication (CHF)', 'Valore di aggiudicazione (CHF)', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_OFFERTEANZAHLMITARBEITENDE', 'Anzahl Mitarbeitende', 'Nombre de collaborateurs', 'Numero di collaboratori', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_OFFERTEBEMERKUNG', 'Offerte Bemerkung', 'Remarque sur l''offre', 'Osservazione sull''offerta', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_OFFERTEZUSTAND', 'Offerte Zustand', 'État de l''offre', 'Stato dell''offerta', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_OFFERTEZUSTANDDATUM', 'Offerte Zustandsdatum', 'Date de l''état de l''offre', 'Data dello stato dell''offerta', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_OFFERTEZUSTANDEINGEREICHT', 'Offerte Zustand Eingereicht', 'Soumis état de l''offre', 'Presentato stato dell''offerta', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_OFFERTEZUSTANDPUBLIKATION', 'Offerte Zustand Publikation', 'Publication de l''état de l''offre', 'Pubblicazione dello stato dell''offerta', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_OFFERTEZUSTANDVERZOEGERUNGDURCHAUFTRAGGEBER', 'Offerte Zustand Verzögerung durch Auftraggeber', 'État de l''offre retard imputable au donneur d''ordre', 'Stato dell''offerta ritardo causato dal committente', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_OFFERTEZUSTANDANZAHLTAGE', 'Offerte Zustand Anzahl Tage', 'État de l''offre nombre de jours', 'Stato dell''offerta numero di giorni', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_OFFERTEZUSTANDAKTUELL', 'Aktueller Offertzustand', 'État actuel de l''offre', 'Stato attuale dell''offerta', v_relid);

    -- Anbieter
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_ANBIETERNAME', 'Anbieter', 'Fournisseur', 'Fornitore', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_ANBIETERADRESSE', 'Anbieter Adresszeile 1', 'Fournisseur adresse ligne 1', 'Fornitore indirizzo riga 1', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_ANBIETERADRESSE2', 'Anbieter Adresszeile 2', 'Fournisseur adresse ligne 2', 'Fornitore indirizzo riga 2', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_ANBIETERADRESSE3', 'Anbieter Adresszeile 3', 'Fournisseur adresse ligne 3', 'Fornitore indirizzo riga 3', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_ANBIETERPLZ', 'Anbieter PLZ', 'Fournisseur NPA', 'Fornitore NPA', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_ANBIETERORT', 'Anbieter Ortschaft', 'Fournisseur localité', 'Fornitore località', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_ANBIETERKANTON', 'Anbieter Kanton', 'Fournisseur canton', 'Fornitore cantone', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_ANBIETERLAND', 'Anbieter Land', 'Fournisseur pays', 'Fornitore paese', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_ANBIETERAKTIV', 'Anbieter aktiv', 'Fournisseur actif', 'Fornitore attivo', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_CPVNUMMERN', 'CPV-Nummern', 'Numéros CPV', 'Numeri CPV', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_PUBLIKATIONDE', 'Publikation DE', 'Publication DE', 'Pubblicazione DE', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_PUBLIKATIONFR', 'Publikation FR', 'Publication FR', 'Pubblicazione FR', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_PUBLIKATIONIT', 'Publikation IT', 'Publication IT', 'Pubblicazione IT', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_PUBLIKATIONEN', 'Publikation EN', 'Publication EN', 'Pubblicazione EN', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_UNTERLAGENDE', 'Unterlagen DE', 'Documents DE', 'Documentazione DE', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_UNTERLAGENFR', 'Unterlagen FR', 'Documents FR', 'Documentazione FR', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_UNTERLAGENIT', 'Unterlagen IT', 'Documents IT', 'Documentazione IT', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_UNTERLAGENEN', 'Unterlagen EN', 'Documents EN', 'Documentazione EN', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_OFFERTENDE', 'Offerten DE', 'Offres DE', 'Offerte DE', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_OFFERTENFR', 'Offerten FR', 'Offres FR', 'Offerte FR', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_OFFERTENIT', 'Offerten IT', 'Offres IT', 'Offerte IT', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_OFFERTENEN', 'Offerten EN', 'Offres EN', 'Offerte EN', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_BKPNUMMERN', 'BKP-Nummern', 'Numéros CFC', 'Numeri CCC', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_ANBIETERKANTONSPRACHE', 'Anbieter Kantonssprache', 'Fournisseur langue du canton', 'Fornitore lingua del cantone', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_ANZAHLOFFERTENDEUTSCH', 'Anzahl Offerten Deutsch', 'Nombre d''offres allemand', 'Numero di offerte tedesco', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_ANZAHLOFFERTENFRANZOESISCH', 'Anzahl Offerten Französisch', 'Nombre d''offres français', 'Numero di offerte francese', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_ANZAHLOFFERTENITALIENISCH', 'Anzahl Offerten Italienisch', 'Nombre d''offres italien', 'Numero di offerte italiano', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_ANZAHLOFFERTENAUSLAND', 'Anzahl Offerten Ausland', 'Nombre d''offres étranger', 'Numero di offerte estero', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_BERICHTIGUNGNEINGEREICHT', 'Berichtigung {0} eingereicht', 'Rectification {0} soumise', 'Rettifica {0} presentata', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_BERICHTIGUNGNPUBLIZIERT', 'Berichtigung {0} publiziert', 'Rectification {0} publiée', 'Rettifica {0} pubblicata', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_BERICHTIGUNGNVERZOEGERUNG', 'Verzögerung bei Berichtigung {0} durch Auftraggeber', 'Retard de rectification {0} imputable au donneur d''ordre', 'Ritardo della rettifica {0} causato dal committente', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_BERICHTIGUNGNANZAHLTAGE', 'Berichtigung {0} Anzahl Tage', 'Rectification {0} nombre de jours', 'Rettifica {0} numero di giorni', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_BERICHTIGUNGNBEMERKUNGEN', 'Berichtigung {0} Bemerkungen', 'Rectification {0} remarques', 'Rettifica {0} osservazioni', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_DIENSTSTELLENNUMMER', 'Dienststellennummer', 'Numéro de service', 'Numero di servizio', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'MENU_EINSTELLUNGEN_BENUTZER', 'Benutzer', 'Utilisateurs', 'Utenti', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_BENUTZER', 'Benutzer', 'Utilisateurs', 'Utenti', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_BENUTZER_EXTID', 'eIAM-ID', 'ID eIAM', 'ID eIAM', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_BENUTZER_VORNAME', 'Vorname', 'Prénom', 'Nome', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_BENUTZER_NACHNAME', 'Nachname', 'Nom', 'Cognome', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_BENUTZER_EMAIL', 'E-Mail', 'E-mail', 'E-mail', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_BENUTZER_SPRACHE', 'Sprache', 'Langue', 'Lingua', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_BENUTZER_LASTLOGIN', 'Letzte Anmeldung', 'Dernière connexion', 'Ultimo accesso', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_BENUTZER_AKTIV', 'Aktiv', 'Actif', 'Attivo', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_BENUTZER_FILTER_AKTIVE', 'Aktive Benutzer', 'Utilisateurs actifs', 'Utenti attivi', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_BENUTZER_FILTER_INAKTIVE', 'Inaktive Benutzer', 'Utilisateurs inactifs', 'Utenti inattivi', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_BENUTZER_FILTER_NEUE', 'Neue Benutzer', 'Nouveaux utilisateurs', 'Nuovi utenti', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_BENUTZER_AKTIVIEREN', 'Aktivieren', 'Activer', 'Attivare', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_BENUTZER_DEAKTIVIEREN', 'Deaktivieren', 'Désactiver', 'Disattivare', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_BENUTZER_EIGENER_BENUTZER', 'Der eigene Benutzer kann nicht deaktiviert werden', 'Votre propre utilisateur ne peut pas être désactivé', 'Il proprio utente non può essere disattivato', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'ANMELDUNG_TITEL', 'Anmeldung', 'Connexion', 'Accesso', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'ANMELDUNG_ANGEMELDET_ALS', 'Angemeldet als', 'Connecté en tant que', 'Connesso come', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'ANMELDUNG_NICHT_FREIGESCHALTET', 'Ihr Benutzerkonto ist für DoeA noch nicht freigeschaltet oder wurde deaktiviert. Bitte wenden Sie sich an die Administration.', 'Votre compte utilisateur n''est pas encore activé pour DoeA ou a été désactivé. Veuillez vous adresser à l''administration.', 'Il suo account utente non è ancora stato abilitato per DoeA o è stato disattivato. La preghiamo di rivolgersi all''amministrazione.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'ANMELDUNG_ERNEUT_PRUEFEN', 'Erneut prüfen', 'Vérifier à nouveau', 'Verificare di nuovo', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'ANMELDUNG_ABGEMELDET', 'Sie wurden abgemeldet.', 'Vous avez été déconnecté.', 'È stato disconnesso.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'ANMELDUNG_ABGEMELDET_INAKTIVITAET', 'Sie wurden nach {0} Minuten Inaktivität abgemeldet.', 'Vous avez été déconnecté après {0} minutes d''inactivité.', 'È stato disconnesso dopo {0} minuti di inattività.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'ANMELDUNG_FEHLGESCHLAGEN', 'Die Anmeldung ist fehlgeschlagen. Bitte versuchen Sie es erneut; bleibt der Fehler bestehen, wenden Sie sich an die Administration.', 'La connexion a échoué. Veuillez réessayer; si l''erreur persiste, adressez-vous à l''administration.', 'L''accesso non è riuscito. La preghiamo di riprovare; se l''errore persiste, si rivolga all''amministrazione.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'ANMELDUNG_ANMELDEN', 'Erneut anmelden', 'Se reconnecter', 'Accedere di nuovo', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'ANMELDUNG_ABMELDEN', 'Abmelden', 'Se déconnecter', 'Disconnettersi', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'ERROR_FORBIDDEN', 'Sie sind für diese Aktion nicht berechtigt.', 'Vous n''êtes pas autorisé à effectuer cette action.', 'Non è autorizzato a eseguire questa azione.', v_relid);

    insert into dbo.t_translation values (gen_random_uuid(), 'SITZUNG_WARNUNG_TITEL', 'Sitzung läuft ab', 'La session va expirer', 'La sessione sta per scadere', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'SITZUNG_WARNUNG_TEXT', 'Sie waren längere Zeit nicht aktiv. Ohne weitere Aktivität werden Sie automatisch abgemeldet.', 'Vous êtes inactif depuis un certain temps. Sans nouvelle activité, vous serez automatiquement déconnecté.', 'Non è attivo da un po'' di tempo. Senza ulteriore attività verrà disconnesso automaticamente.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'SITZUNG_ABLAUF_IN', 'Abmeldung in {0}', 'Déconnexion dans {0}', 'Disconnessione tra {0}', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'SITZUNG_VERLAENGERN', 'Sitzung verlängern', 'Prolonger la session', 'Prolungare la sessione', v_relid);

    -- Fehlerseite (/Error) und Blazor-Fehlerbanner
    insert into dbo.t_translation values (gen_random_uuid(), 'FEHLERSEITE_TITEL', 'Fehler', 'Erreur', 'Errore', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'FEHLERSEITE_TEXT', 'Bei der Verarbeitung Ihrer Anfrage ist ein Fehler aufgetreten.', 'Une erreur est survenue lors du traitement de votre demande.', 'Si è verificato un errore durante l''elaborazione della richiesta.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'FEHLERSEITE_ANFRAGE_ID', 'Anfrage-ID', 'ID de la requête', 'ID della richiesta', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'FEHLERSEITE_ZUR_STARTSEITE', 'Zur Startseite', 'Vers la page d''accueil', 'Alla pagina iniziale', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'FEHLERBANNER_TEXT', 'Ein unerwarteter Fehler ist aufgetreten.', 'Une erreur inattendue est survenue.', 'Si è verificato un errore imprevisto.', v_relid);
    insert into dbo.t_translation values (gen_random_uuid(), 'FEHLERBANNER_NEU_LADEN', 'Neu laden', 'Recharger', 'Ricaricare', v_relid);

    -- Log-Filter: Eintrag «Alle» der Level-Auswahl
    insert into dbo.t_translation values (gen_random_uuid(), 'EINSTELLUNGEN_LOG_FILTER_ALLE', 'Alle', 'Tous', 'Tutti', v_relid);

end
$$;
