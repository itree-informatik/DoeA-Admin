// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DTO.Exportgruppen;

namespace DoeA.Interfaces;

/// <summary>
/// Verwaltet die gespeicherten Filterdefinitionen (Exportgruppen) und führt sie gegen den
/// Ausschreibungsbestand aus.
/// </summary>
public interface IExportgruppeService
{
    /// <summary>Liefert alle Exportgruppen als Listeneinträge, alphabetisch nach Name.</summary>
    Task<List<ExportgruppeListItem>?> GetAllAsync();

    /// <summary>Lädt eine Exportgruppe samt den Ids ihrer n:m-Filterlisten; null, wenn es sie nicht (mehr) gibt.</summary>
    Task<Exportgruppe?> GetAsync(Guid expId);

    /// <summary>Speichert eine Exportgruppe (neu oder bestehend) inkl. Abgleich der n:m-Filterlisten.</summary>
    Task<Exportgruppe> SaveAsync(Exportgruppe model);

    /// <summary>Speichert gezielt nur die Spaltensichtbarkeit, ohne die übrigen Formularwerte anzufassen.</summary>
    Task<Exportgruppe> SaveSpaltenAsync(Exportgruppe model);

    /// <summary>Speichert gezielt nur den Namen, ohne die übrigen Formularwerte anzufassen.</summary>
    Task<Exportgruppe> SaveNameAsync(Exportgruppe model);

    /// <summary>Löscht eine Exportgruppe samt ihren Verknüpfungszeilen.</summary>
    Task<Exportgruppe> DeleteAsync(Guid expId);

    /// <summary>Führt die Filterdefinition aus: eine Zeile pro Ausschreibung für die Ergebnisliste.</summary>
    Task<List<Ergebnis>?> GetErgebnisAsync(Exportgruppe model);

    /// <summary>Führt die Filterdefinition für den erweiterten Export aus: eine Zeile pro Offertzustand.</summary>
    Task<List<ErgebnisAlleDaten>?> GetErgebnisAlleDatenAsync(Exportgruppe model);
}
