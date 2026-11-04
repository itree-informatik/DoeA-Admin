-- SPDX-License-Identifier: AGPL-3.0-or-later
-- Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

-- Zustand einer Offerte: Bei Zuständen mit Publikation (ofzpublikation) ist oozdatum das Publikationsdatum,
-- eine eigene Spalte dafür entfällt. Ein erfasstes Publikationsdatum geht vorher ins Datum über.
do $$
begin
    if exists (select 1 from information_schema.columns
               where table_schema = 'dbo' and table_name = 't_offerteoffertezustand' and column_name = 'oozpublikation') then
        update dbo.t_offerteoffertezustand
        set oozdatum = oozpublikation
        where oozpublikation is not null and oozpublikation <> oozdatum;
    end if;
end
$$;

alter table dbo.t_offerteoffertezustand drop column if exists oozpublikation;

do $$
declare
    v_relid uuid;
begin

    v_relid := dbo.f_getlatestreleaseid();

    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_OFFERTE_ZUSTAND_PUBLIZIERT', 'Publiziert', 'Publié', 'Pubblicato', v_relid)
    on conflict (trlkey) do nothing;

    -- Spalte des Alle-Daten-Exports, die mit oozpublikation entfällt
    delete from dbo.t_translation where trlkey = 'EXPORTGRUPPE_ERGEBNISALLEDATEN_OFFERTEZUSTANDPUBLIKATION';

end
$$;
