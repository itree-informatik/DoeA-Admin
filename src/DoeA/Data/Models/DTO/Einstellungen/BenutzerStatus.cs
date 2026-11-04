// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

namespace DoeA.Data.Models.DTO.Einstellungen;

/// <summary>DB-Stand eines Benutzers für den Abgleich mit den Cookie-Claims (Autorisierung und Sprache).</summary>
public sealed record BenutzerStatus(Guid Id, bool Aktiv, string Sprache);
