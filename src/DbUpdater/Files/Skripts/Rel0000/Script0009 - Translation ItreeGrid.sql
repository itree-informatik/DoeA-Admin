-- SPDX-License-Identifier: AGPL-3.0-or-later
-- Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

do $$
declare
    v_relid uuid;
begin

    v_relid := dbo.f_getlatestreleaseid();

    -- ItreeGrid (ItreeMud): Text ohne Datensätze, Standard-Filtermenü und die Buttons der Auswahlfilter (ItreeFilterButtons)
    insert into dbo.t_translation values (gen_random_uuid(), 'ITREEGRID_KEINE_DATEN', 'Keine Daten vorhanden.', 'Aucune donnée disponible.', 'Nessun dato disponibile.', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'ITREEGRID_FILTER_FILTERN', 'Filtern', 'Filtrer', 'Filtra', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'ITREEGRID_FILTER_ZURUECKSETZEN', 'Zurücksetzen', 'Réinitialiser', 'Reimposta', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'ITREEGRID_FILTER_UND', 'Und', 'Et', 'E', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'ITREEGRID_FILTER_ODER', 'Oder', 'Ou', 'O', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'ITREEGRID_FILTER_JA', 'Ja', 'Oui', 'Sì', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'ITREEGRID_FILTER_NEIN', 'Nein', 'Non', 'No', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'ITREEGRID_FILTER_WERT', 'Wert', 'Valeur', 'Valore', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'ITREEGRID_FILTER_OPERATOR_ISEQUALTO', 'Ist gleich', 'Est égal à', 'È uguale a', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'ITREEGRID_FILTER_OPERATOR_ISNOTEQUALTO', 'Ist nicht gleich', 'N''est pas égal à', 'Non è uguale a', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'ITREEGRID_FILTER_OPERATOR_ISGREATERTHAN', 'Ist grösser als', 'Est supérieur à', 'È maggiore di', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'ITREEGRID_FILTER_OPERATOR_ISGREATERTHANOREQUALTO', 'Ist grösser oder gleich', 'Est supérieur ou égal à', 'È maggiore o uguale a', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'ITREEGRID_FILTER_OPERATOR_ISLESSTHAN', 'Ist kleiner als', 'Est inférieur à', 'È minore di', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'ITREEGRID_FILTER_OPERATOR_ISLESSTHANOREQUALTO', 'Ist kleiner oder gleich', 'Est inférieur ou égal à', 'È minore o uguale a', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'ITREEGRID_FILTER_OPERATOR_STARTSWITH', 'Beginnt mit', 'Commence par', 'Inizia con', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'ITREEGRID_FILTER_OPERATOR_ENDSWITH', 'Endet mit', 'Se termine par', 'Finisce con', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'ITREEGRID_FILTER_OPERATOR_CONTAINS', 'Enthält', 'Contient', 'Contiene', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'ITREEGRID_FILTER_OPERATOR_DOESNOTCONTAIN', 'Enthält nicht', 'Ne contient pas', 'Non contiene', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'ITREEGRID_FILTER_OPERATOR_ISNULL', 'Hat keinen Wert', 'N''a pas de valeur', 'Non ha valore', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'ITREEGRID_FILTER_OPERATOR_ISNOTNULL', 'Hat einen Wert', 'A une valeur', 'Ha un valore', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'ITREEGRID_FILTER_OPERATOR_ISEMPTY', 'Ist leer', 'Est vide', 'È vuoto', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'ITREEGRID_FILTER_OPERATOR_ISNOTEMPTY', 'Ist nicht leer', 'N''est pas vide', 'Non è vuoto', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'ITREEGRID_FILTER_OPERATOR_DATE_ISGREATERTHAN', 'Ist nach', 'Est après', 'È dopo', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'ITREEGRID_FILTER_OPERATOR_DATE_ISGREATERTHANOREQUALTO', 'Ist am oder nach', 'Est le ou après', 'È il o dopo', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'ITREEGRID_FILTER_OPERATOR_DATE_ISLESSTHAN', 'Ist vor', 'Est avant', 'È prima', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'ITREEGRID_FILTER_OPERATOR_DATE_ISLESSTHANOREQUALTO', 'Ist am oder vor', 'Est le ou avant', 'È il o prima', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'BUTTON_FILTERSETZEN', 'Filtern', 'Filtrer', 'Filtra', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'BUTTON_FILTERLEEREN', 'Leeren', 'Effacer', 'Cancella', v_relid)
    on conflict (trlkey) do nothing;

end
$$;
