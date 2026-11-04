-- SPDX-License-Identifier: AGPL-3.0-or-later
-- Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

-- Widerruf einer RFI oder Vorankündigung: Sie haben keine Lose, Zustellung, Publikation und Verzögerung des Widerrufs
-- stehen deshalb in der Ausschreibung.
alter table dbo.t_ausschreibung add column if not exists auseingereichtwiderruf date null;
alter table dbo.t_ausschreibung add column if not exists auspublikationwiderruf date null;
alter table dbo.t_ausschreibung add column if not exists ausverzoegerungdurchauftraggeberwiderruf boolean not null default false;

do $$
declare
    v_relid uuid;
begin

    v_relid := dbo.f_getlatestreleaseid();

    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_WIDERRUF', 'Widerruf', 'Révocation', 'Revoca', v_relid)
    on conflict (trlkey) do nothing;

end
$$;
