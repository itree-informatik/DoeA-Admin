-- SPDX-License-Identifier: AGPL-3.0-or-later
-- Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

-------------------------------------------------------------------------------
-- indexes
-- PostgreSQL indexiert Fremdschlüssel nicht automatisch. Ohne diese Indexe
-- laufen die Auswertungen (z.B. Zielerreichung) bei grösserem Datenbestand
-- in einen Timeout, weil die korrelierte Subquery für den letzten
-- Offertzustand (max(oozdatum) pro Offerte) pro Zeile einen Seq-Scan macht.
-------------------------------------------------------------------------------

create index ix_los_losausid on dbo.t_los (losausid);

create index ix_offerte_offlosid on dbo.t_offerte (offlosid);

-- inkl. oozdatum: deckt die Ermittlung des letzten Offertzustands pro Offerte
-- als Index-Only-Scan ab
create index ix_offerteoffertezustand_oozoffid_oozdatum on dbo.t_offerteoffertezustand (oozoffid, oozdatum);

create index ix_berichtigung_berausid on dbo.t_berichtigung (berausid);

-- Login-Lookup über den eIAM-Identifikator (Claim "sub"); unique verhindert Doppelanlagen
-- beim parallelen Erstlogin
create unique index ix_user_usrextid on dbo.t_user (usrextid);
