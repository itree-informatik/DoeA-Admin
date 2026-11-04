-- SPDX-License-Identifier: AGPL-3.0-or-later
-- Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

do $$
declare
    v_relid uuid;
begin

    v_relid := dbo.f_getlatestreleaseid();

    -- Ausschreibungs-Suche: leerer Eintrag der Auswahl Offertzustand
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_OFFERTEZUSTAND_EGAL', 'Offertzustand egal', 'État de l''offre indifférent', 'Stato dell''offerta indifferente', v_relid)
    on conflict (trlkey) do nothing;

end
$$;
