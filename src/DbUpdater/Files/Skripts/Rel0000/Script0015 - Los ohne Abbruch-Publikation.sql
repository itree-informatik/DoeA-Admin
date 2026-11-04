-- SPDX-License-Identifier: AGPL-3.0-or-later
-- Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

-- Los: Das Abbruchdatum ist das Datum der Publikation des Abbruchs, die eigene Publikation entfällt. Eine erfasste
-- Publikation war in den Auswertungen massgebend und wird darum ins Abbruchdatum übernommen.
update dbo.t_los set losabbruchdatum = lospublikation where lospublikation is not null;
alter table dbo.t_los drop column if exists lospublikation;

do $$
declare
    v_relid uuid;
begin

    v_relid := dbo.f_getlatestreleaseid();

    insert into dbo.t_translation values (gen_random_uuid(), 'VALIDATOR_ABBRUCH_VOR_ZUGESTELLT', 'Das Abbruchdatum darf nicht vor dem Datum «an DöA zugestellt» liegen.', 'La date d''interruption ne peut pas être antérieure à la date «remis au SMP».', 'La data di interruzione non può essere anteriore alla data «consegnato al SMP».', v_relid)
    on conflict (trlkey) do nothing;

    delete from dbo.t_translation where trlkey = 'EXPORTGRUPPE_ERGEBNISALLEDATEN_LOSPUBLIKATION';

end
$$;
