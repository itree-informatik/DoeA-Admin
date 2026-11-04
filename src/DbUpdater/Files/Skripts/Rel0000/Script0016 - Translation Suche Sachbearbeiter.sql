-- SPDX-License-Identifier: AGPL-3.0-or-later
-- Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

do $$
declare
    v_relid uuid;
begin

    v_relid := dbo.f_getlatestreleaseid();

    -- Zuschaltbare Ergebnisspalten der Ausschreibungs-Suche
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_SACHBEARBEITER_RD', 'SB RD', 'Collab. DJ', 'Collab. DJ', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_SACHBEARBEITER_BBL', 'SB BBL', 'Collab. OFCL', 'Collab. UFCL', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNGEN_EXTERN', 'Extern', 'Externe', 'Esterno', v_relid)
    on conflict (trlkey) do nothing;

end
$$;
