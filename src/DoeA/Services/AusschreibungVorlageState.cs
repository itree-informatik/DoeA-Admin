// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DTO;

namespace DoeA.Services;

/// <summary>
/// Vorbefüllte, noch nicht gespeicherte Neuanlage (Folgeausschreibung), die das AusschreibungLayout auf
/// ausschreibung/{Guid.Empty} statt einer leeren Ausschreibung öffnet. Scoped -> gilt für den Blazor-Circuit.
/// </summary>
public class AusschreibungVorlageState
{
    private Ausschreibung? _vorlage;

    /// <summary>Merkt die Vorlage für die nächste Neuanlage.</summary>
    public void Setzen(Ausschreibung vorlage) => _vorlage = vorlage;

    /// <summary>Liefert die Vorlage genau einmal; danach öffnet die Neuanlage wieder leer.</summary>
    public Ausschreibung? Abholen()
    {
        var vorlage = _vorlage;
        _vorlage = null;
        return vorlage;
    }
}
