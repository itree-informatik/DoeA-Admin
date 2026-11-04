-- SPDX-License-Identifier: AGPL-3.0-or-later
-- Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

do $$
declare
    v_relid uuid;
begin

    v_relid := dbo.f_getlatestreleaseid();

    -- Alle-Daten-Export: Widerruf einer RFI oder Vorankündigung
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_EINGEREICHTWIDERRUF', 'Widerruf Eingereicht', 'Révocation soumis', 'Revoca presentato', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_PUBLIKATIONWIDERRUF', 'Widerruf Publikation', 'Révocation publication', 'Revoca pubblicazione', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_VERZOEGERUNGDURCHAUFTRAGGEBERWIDERRUF', 'Widerruf Verzögerung durch Auftraggeber', 'Révocation retard imputable au donneur d''ordre', 'Revoca ritardo causato dal committente', v_relid)
    on conflict (trlkey) do nothing;
    insert into dbo.t_translation values (gen_random_uuid(), 'EXPORTGRUPPE_ERGEBNISALLEDATEN_ANZAHLTAGEWIDERRUF', 'Widerruf Anzahl Tage', 'Révocation nombre de jours', 'Revoca numero di giorni', v_relid)
    on conflict (trlkey) do nothing;

    -- Der 5-Jahresvergleich zählt die Widerrufe von RFI und Vorankündigungen nicht (nur Beschaffungscontrolling und Zielerreichung)
    update dbo.t_translation
    set trldeutsch = 'Abbrüche und Widerrufe (5 Jahresvergleich) ohne RFI und Vorankündigungen',
        trlfranzoesisch = 'Interruptions et révocations (comparaison sur 5 ans) sans RFI ni annonces préalables',
        trlitalienisch = 'Interruzioni e revoche (confronto su 5 anni) senza RFI e preavvisi'
    where trlkey = 'AUSWERTUNG_TILE_ABBRUCHWIDERRUF';

end
$$;
