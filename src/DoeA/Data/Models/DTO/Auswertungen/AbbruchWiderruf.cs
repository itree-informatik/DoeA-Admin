// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

namespace DoeA.Data.Models.DTO.Auswertungen;

/// <summary>Projektionszeile der Auswertung «Abbruch und Widerruf» — eine Ausschreibung mit den Merkmalen für die Zählung je Jahr/Projektart.</summary>
public class AbbruchWiderruf
{
    public string ProjektArt { get; set; } = string.Empty;
    public int Jahr { get; set; }
    public bool AbbruchGesamt { get; set; }
    public bool AbbruchNachEvaluation { get; set; }
    public bool Widerrufen { get; set; }
}
