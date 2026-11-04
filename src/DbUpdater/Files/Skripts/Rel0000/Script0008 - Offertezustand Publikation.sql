-- SPDX-License-Identifier: AGPL-3.0-or-later
-- Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

-- Offertezustand: Flag, ob auf den Zustand eine Publikation folgt (blendet bei den Offerten
-- Publikation, Anzahl Tage und Verzögerung ein)
alter table dbo.t_offertezustand add column if not exists ofzpublikation boolean not null default false;

-- Wirkt nur auf bestehende Datenbanken; bei einem Neuaufbau ist die Tabelle hier noch leer,
-- dort setzt die DataMigration das Flag
update dbo.t_offertezustand
set ofzpublikation = true
where ofznaturalid in ('widerrufen', 'widerrufpraequali', 'zuschlag', 'zuschlagpraequali');

-- Zustand einer Offerte: Publikation, Verzögerung durch Auftraggeber und Datum «an DöA zugestellt» als Beginn
-- der Frist bis zur Publikation (analog Ausschreibung)
alter table dbo.t_offerteoffertezustand add column if not exists oozpublikation date null;
alter table dbo.t_offerteoffertezustand add column if not exists oozverzoegerungdurchauftraggeber boolean not null default false;
alter table dbo.t_offerteoffertezustand add column if not exists oozeingereicht date null;

do $$
declare
    v_relid uuid;
begin

    v_relid := dbo.f_getlatestreleaseid();

    insert into dbo.t_translation values (gen_random_uuid(), 'STAMMDATEN_OFFERTEZUSTAND_PUBLIKATION', 'Publikation', 'Publication', 'Pubblicazione', v_relid)
    on conflict (trlkey) do nothing;

end
$$;
