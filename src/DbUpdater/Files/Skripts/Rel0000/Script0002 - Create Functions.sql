-- SPDX-License-Identifier: AGPL-3.0-or-later
-- Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

create or replace function dbo.f_sqldatetimetoticks(p_date timestamp)
returns bigint
language sql
immutable
as $$
    select (p_date::date - date '0001-01-01')::bigint * 864000000000
         + extract(hour   from p_date)::bigint * 36000000000
         + extract(minute from p_date)::bigint * 600000000
         + floor(extract(second from p_date))::bigint * 10000000
         + mod(floor(extract(milliseconds from p_date))::bigint, 1000) * 10000;
$$;

create or replace function dbo.f_getlatestreleaseid()
returns uuid
language sql
stable
as $$
    select "relid"
    from dbo."t_release"
    order by
        coalesce(nullif(split_part(relversion, '.', 1), '')::int, 0) desc,
        coalesce(nullif(split_part(relversion, '.', 2), '')::int, 0) desc,
        coalesce(nullif(split_part(relversion, '.', 3), '')::int, 0) desc
    limit 1;
$$;