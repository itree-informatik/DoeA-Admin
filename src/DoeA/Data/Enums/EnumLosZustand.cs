// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

namespace DoeA.Data.Enums;

/// <summary>Zustand eines Loses; wird aus Abbruchdatum und Schlusszuständen der Offerten ermittelt (LosService).</summary>
public enum EnumLosZustand
{
    Offen = 1,
    Abgebrochen = 2,
    Abgeschlossen = 3,
    Widerrufen = 4
}
