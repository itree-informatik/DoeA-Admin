-- SPDX-License-Identifier: AGPL-3.0-or-later
-- Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

-- Ausschreibung: Eingereicht, Publikation und Verzögerung der Präqualifikation entfallen ersatzlos. Die Präqualifikation
-- wird als Offertzustand «Zuschlag Präqualifikation» erfasst (Eingereicht, Datum = Publikation, Verzögerung); bestehende
-- Publikationsdaten trägt der Kunde nach der Einführung manuell als Offertzustände nach.
alter table dbo.t_ausschreibung drop column if exists auseingereichtpraequali;
alter table dbo.t_ausschreibung drop column if exists auspublikationpraequali;
alter table dbo.t_ausschreibung drop column if exists ausverzoegerungdurchauftraggeberpraequali;

do $$
declare
    v_relid uuid;
begin

    v_relid := dbo.f_getlatestreleaseid();

    insert into dbo.t_translation values (gen_random_uuid(), 'ERROR_AUSSCHREIBUNG_PRAEQUALI_DATEN_UNTERSCHIEDLICH', 'Alle Zuschläge Präqualifikation einer Ausschreibung müssen dasselbe Eingereicht- und Publikationsdatum haben.', 'Toutes les adjudications de préqualification d''un appel d''offres doivent avoir la même date de remise et de publication.', 'Tutte le aggiudicazioni di prequalifica di un bando devono avere la stessa data di inoltro e di pubblicazione.', v_relid)
    on conflict (trlkey) do nothing;

    -- Beschriftungen der entfallenen Felder (Grunddaten, Alle-Daten-Export)
    delete from dbo.t_translation where trlkey in (
        'AUSSCHREIBUNG_EINGEREICHTPRAEQUALI',
        'AUSSCHREIBUNG_PUBLIKATIONPRAEQUALI',
        'EXPORTGRUPPE_ERGEBNISALLEDATEN_EINGEREICHTPRAEQUALI',
        'EXPORTGRUPPE_ERGEBNISALLEDATEN_PUBLIKATIONPRAEQUALI',
        'EXPORTGRUPPE_ERGEBNISALLEDATEN_VERZOEGERUNGDURCHAUFTRAGGEBERPRAEQUALI',
        'EXPORTGRUPPE_ERGEBNISALLEDATEN_ANZAHLTAGEPRAEQUALI');

end
$$;
