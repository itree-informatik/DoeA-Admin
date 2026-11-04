// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DTO;
using System.Collections.ObjectModel;

namespace DoeA.Interfaces;

/// <summary>
/// Lädt, sucht und speichert Ausschreibungen samt ihrer direkt angehängten Daten
/// (CPV-Nummern, Sprachen/Dokumentarten, Berichtigungen).
/// </summary>
public interface IAusschreibungService : IBaseService<Ausschreibung>
{
    /// <summary>
    /// Sucht Ausschreibungen über Bezeichnung, Nummer, SIMAP-Nummer, Sachbearbeiter (DöA, RD, BBL), Extern und
    /// Anbietername; ein Offertzustand schränkt nur die Anbieter-Treffer auf Offerten ein, deren aktueller Zustand er ist.
    /// Alle übrigen Parameter wirken nur, wenn sie gesetzt sind.
    /// </summary>
    Task<ObservableCollection<AusschreibungSuche>> Suche(int? jahr, string searchValue, DateOnly? publikationVon = null, DateOnly? publikationBis = null, bool forHauptprojekt = false, string[]? verfahrensartFilter = null, decimal? betragVon = null, decimal? betragBis = null, Guid? offertezustandId = null);

    /// <summary>Alle Jahre, in denen Ausschreibungen erfasst sind, absteigend.</summary>
    Task<List<int>> GetAlleJahre();

    /// <summary>Alle vergebenen Abschlussjahre, absteigend.</summary>
    Task<List<int>> GetAlleAbschlussJahre();

    /// <summary>Lädt eine Ausschreibung mit Stammdaten-Bezeichnungen, Verweisen, CPV-Nummern, Sprachen und Berichtigungen.</summary>
    Task<Ausschreibung?> Get(Guid ausId);

    /// <summary>Speichert eine Ausschreibung (neu oder bestehend) samt CPV-Nummern, Sprachen und Berichtigungen.</summary>
    Task<Ausschreibung> Save(Ausschreibung aus);

    /// <summary>Bereitet zu einer Vorankündigung die Folgeausschreibung als ungespeicherte Neuanlage vor (gleiche Nummer beim Speichern).</summary>
    Task<Ausschreibung> BereiteFolgeausschreibungVor(Guid vorankuendigungId);
}
