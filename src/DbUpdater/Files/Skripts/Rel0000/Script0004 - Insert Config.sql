-- SPDX-License-Identifier: AGPL-3.0-or-later
-- Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

do $$
declare
    v_ticks bigint;
    v_user varchar(255) := 'Migration';
begin

    v_ticks := dbo.f_sqldatetimetoticks(localtimestamp);

    insert into dbo.t_konfiguration values (gen_random_uuid(),
                                            'Anzahl Tage Logeinträge aufbewahren',
                                            '30',
                                            'int',
                                            'Anzahl Tage für die Logeinträge nicht gelöscht werden sollen',
                                            'AnzahlTageLog',
                                            true, v_ticks, v_user, v_ticks, v_user);
    
    insert into dbo.t_konfiguration values (gen_random_uuid(),
                                            'Frist Publikation',
                                            '3',
                                            'int',
                                            'Maximale Anzahl tolerierte Tage zwischen Auftragseingang und Publikation für die Zielerreichung',
                                            'FristPublikation',
                                            true, v_ticks, v_user, v_ticks, v_user);

    insert into dbo.t_konfiguration values (gen_random_uuid(),
                                            'Minimale Zielerreichung in Prozent',
                                            '95',
                                            'int',
                                            'Minimaler Prozentsatz zum Erfüllen der Zielerreichung von fristgerechten Publikationen',
                                            'MinimaleZielerreichung',
                                            true, v_ticks, v_user, v_ticks, v_user);

end
$$;
