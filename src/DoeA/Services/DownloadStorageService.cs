// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Interfaces;

namespace DoeA.Services;

/// <inheritdoc cref="IDownloadStorageService"/>
/// <remarks>
/// Als Singleton registriert und deshalb über mehrere Circuits hinweg zugreifbar – alle Zugriffe
/// laufen darum über <c>_lock</c>. Ein Timer räumt Dateien weg, die nie abgeholt wurden.
/// </remarks>
public class DownloadStorageService : IDownloadStorageService, IDisposable
{
    /// <summary>Solange bleibt eine erzeugte Datei abholbar.</summary>
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(10);

    private readonly Timer _cleanupTimer;
    private readonly object _lock = new();
    private readonly Dictionary<string, (byte[] Content, string FileName, DateTime StoredAt)> _store = new();

    public DownloadStorageService()
    {
        _cleanupTimer = new Timer(Cleanup, null, Ttl, Ttl);
    }

    /// <summary>Legt die Datei ab und liefert die Id für den Download-Endpunkt.</summary>
    public string StoreFile(byte[] content, string fileName)
    {
        var id = Guid.NewGuid().ToString();
        lock (_lock)
        {
            _store[id] = (content, fileName, DateTime.UtcNow);
        }

        return id;
    }

    /// <summary>Holt die Datei ab und entfernt sie dabei; jede Id ist also nur einmal einlösbar.</summary>
    public (byte[] Content, string FileName)? RetrieveFile(string id)
    {
        lock (_lock)
        {
            if (!_store.Remove(id, out var data))
            {
                return null;
            }

            return (data.Content, data.FileName);
        }
    }

    /// <summary>Stoppt den Aufräum-Timer.</summary>
    public void Dispose() => _cleanupTimer.Dispose();

    private void Cleanup(object? _)
    {
        var cutoff = DateTime.UtcNow - Ttl;
        lock (_lock)
        {
            var abgelaufen = _store.Where(kv => kv.Value.StoredAt < cutoff)
                                   .Select(kv => kv.Key)
                                   .ToList();
            foreach (var key in abgelaufen)
            {
                _store.Remove(key);
            }
        }
    }
}
