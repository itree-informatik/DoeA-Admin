// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

namespace DoeA.Interfaces;

/// <summary>
/// Zwischenspeicher für erzeugte Dateien: Der Export legt die Datei ab, der Browser holt sie
/// anschliessend über den <c>/download/{id}</c>-Endpunkt.
/// </summary>
public interface IDownloadStorageService
{
    /// <summary>Legt die Datei ab und liefert die Id für den Download-Endpunkt.</summary>
    string StoreFile(byte[] content, string fileName);

    /// <summary>Holt die Datei einmalig ab (danach ist die Id ungültig); null, wenn unbekannt oder abgelaufen.</summary>
    (byte[] Content, string FileName)? RetrieveFile(string id);
}
