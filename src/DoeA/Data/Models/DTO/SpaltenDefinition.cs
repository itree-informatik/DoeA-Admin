// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

namespace DoeA.Data.Models.DTO;

/// <summary>
/// Definition einer ein-/ausblendbaren Ergebnisspalte eines Grids: Property-Name (Schlüssel der
/// Sichtbarkeit), Übersetzungskey des Titels und Default-Sichtbarkeit. Wird von den
/// Spaltenkatalogen (z.B. <see cref="Exportgruppen.ExportgruppeSpalten"/>,
/// <see cref="AusschreibungSucheSpalten"/>) und dem <c>SpaltenPopup</c> verwendet.
/// </summary>
public sealed record SpaltenDefinition(string Property, string TranslationKey, bool Default);
