// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

namespace DoeA.Data.Enums;

/// <summary>Vordefinierte Filter der Benutzerliste; Inaktive und Neue unterscheiden sich über UsrLastLogin.</summary>
public enum EnumBenutzerFilter
{
    Aktive = 0,
    Inaktive = 1,
    Neue = 2
}
