-- SPDX-License-Identifier: AGPL-3.0-or-later
-- Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

-- Offertezustände für die Anbieter der Phase 1 des selektiven Verfahrens (nur bei Selektiv wählbar): Ein erfasster
-- Teilnahmeantrag läuft weiter, ein abgelehnter ist abschliessend; auf beide folgt keine Publikation.
-- Wirkt nur auf bestehende Datenbanken (Audit-Felder aus einem vorhandenen Zustand); bei einem Neuaufbau ist die Tabelle
-- hier noch leer, dort legt die DataMigration die Zustände an.
insert into dbo.t_offertezustand
    (ofzid, ofzbezeichnung, ofzkurzbezeichnung, ofzschlusszustand, ofzpublikation, ofznaturalid, ofzaktiv,
     ofzinserteddate, ofzinserteduser, ofzchangeddate, ofzchangeduser)
select gen_random_uuid(), neu.bezeichnung, neu.kurzbezeichnung, neu.schlusszustand, false, neu.naturalid, true,
       vorlage.ofzinserteddate, vorlage.ofzinserteduser, vorlage.ofzchangeddate, vorlage.ofzchangeduser
from (values ('Teilnahmeantrag erfasst', 'TA erfasst', false, 'teilnahmeantragerfasst'),
             ('Teilnahmeantrag abgelehnt', 'TA abgelehnt', true, 'teilnahmeantragabgelehnt'))
         as neu (bezeichnung, kurzbezeichnung, schlusszustand, naturalid)
cross join (select * from dbo.t_offertezustand order by ofzid limit 1) as vorlage
where not exists (select 1 from dbo.t_offertezustand z where z.ofznaturalid = neu.naturalid);
