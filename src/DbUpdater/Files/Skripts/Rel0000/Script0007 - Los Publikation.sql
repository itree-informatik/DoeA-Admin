-- SPDX-License-Identifier: AGPL-3.0-or-later
-- Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

-- Los: Publikation des Abbruchs, Verzögerung durch Auftraggeber und Datum «an DöA zugestellt» als Beginn
-- der Frist bis zur Publikation (analog Ausschreibung)
alter table dbo.t_los add column if not exists lospublikation date null;
alter table dbo.t_los add column if not exists losverzoegerungdurchauftraggeber boolean not null default false;
alter table dbo.t_los add column if not exists loseingereicht date null;
