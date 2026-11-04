// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

namespace DoeA.Services;

/// <summary>
/// Scoped-Service, über den der aktuell aktive Ausschreibung-Tab seine Dirty-Logik beim Layout anmeldet.
/// Das Layout fragt darüber ab, ob pendente (noch nicht gespeicherte) Änderungen vorliegen, und blockiert
/// bei Bedarf die Navigation (Tab-Wechsel oder Verlassen der Seite).
/// Es ist immer höchstens ein Tab aktiv; daher genügt ein einzelner registrierter Delegate.
/// </summary>
public class AusschreibungEditState
{
    private Func<bool>? _hasChanges;

    /// <summary>Meldet die Dirty-Prüfung des aktiven Tabs an (überschreibt eine vorherige Registrierung).</summary>
    public void Register(Func<bool> hasChanges) => _hasChanges = hasChanges;

    /// <summary>
    /// Entfernt die Registrierung (vom Tab in Dispose aufzurufen). Nur wenn noch der eigene Delegate
    /// registriert ist – beim Tab-Wechsel registriert sich der Folge-Tab u.U. bevor der alte freigegeben wird,
    /// und dessen Registrierung darf dann nicht versehentlich gelöscht werden.
    /// (Delegate-Vergleich über == prüft Target + Methode, nicht die Referenz.)
    /// </summary>
    public void Unregister(Func<bool> hasChanges)
    {
        if (_hasChanges == hasChanges)
        {
            _hasChanges = null;
        }
    }

    /// <summary>True, wenn der aktive Tab nicht gespeicherte Änderungen hat.</summary>
    public bool HasChanges() => _hasChanges?.Invoke() ?? false;
}
