-- SPDX-License-Identifier: AGPL-3.0-or-later
-- Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

do $$
declare
    v_relid uuid;
begin

    v_relid := dbo.f_getlatestreleaseid();

    -- Grunddaten: Arbeitstage von Eingereicht bis Publikation
    insert into dbo.t_translation values (gen_random_uuid(), 'AUSSCHREIBUNG_ANZAHLTAGE', 'Anzahl Tage', 'Nombre de jours', 'Numero di giorni', v_relid)
    on conflict (trlkey) do nothing;

end
$$;
