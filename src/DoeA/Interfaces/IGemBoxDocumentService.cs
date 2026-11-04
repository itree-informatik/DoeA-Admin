// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using GemBox.Document;

namespace DoeA.Interfaces;

/// <summary>
/// Word-Erzeugung über GemBox: lädt eine <c>.dotx</c>-Vorlage, ersetzt Platzhalter und Tabellen
/// und liefert das Ergebnis als Stream. Tabellen werden über den Titel im Alternativtext
/// der Tabelleneigenschaften angesprochen.
/// </summary>
public interface IGemBoxDocumentService
{
    /// <summary>Das aktuell geladene Dokument.</summary>
    DocumentModel WordDocumentModel { get; }

    /// <summary>Lädt die Vorlage aus dem Dateisystem; false, wenn der Pfad leer ist oder die Datei fehlt.</summary>
    bool Load(string filename);

    /// <summary>Lädt die Vorlage aus einem Byte-Array.</summary>
    bool Load(byte[] content);

    /// <summary>Speichert das Dokument unter dem angegebenen Pfad.</summary>
    void Save(string filename);

    /// <summary>Ersetzt Platzhalter im gesamten Dokument (Schlüssel = Platzhalter, Wert = Text).</summary>
    void InsertValues(Dictionary<string, string> dictionary);

    /// <summary>Füllt eine Tabelle über Platzhalter und dupliziert dabei den Zeilenblock je Datensatz.</summary>
    bool InsertValuesInTable(string title, int rowStart, int rowEnd, IReadOnlyList<string> placeholders,
        IReadOnlyList<string[]> values);

    /// <summary>Speichert das Dokument im gewünschten Format in einen Stream (aktualisiert das Erstelldatum).</summary>
    MemoryStream GetStream(SaveOptions saveOption);

    /// <summary>Baut eine Tabelle aus Zeilenvorlagen auf; <c>definition</c> ordnet Zeilentyp und Vorlagenzeile zu.</summary>
    bool FillTable(string title, IList<object?[]> definition, IList<string[]> liste);

    /// <summary>Fasst Zellen einer Tabellenzeile horizontal zusammen.</summary>
    void ColumnSpan(string altText, int row, int cell, int span);

    /// <summary>Fasst Zellen einer Tabellenspalte vertikal zusammen.</summary>
    void RowSpan(string altText, int row, int cell, int span);

    /// <summary>Wandelt ein Word-Dokument aus dem Dateisystem in ein PDF um.</summary>
    MemoryStream WordToPdf(string path);

    /// <summary>Schreibt Werte in einen bestehenden Zeilenbereich; <c>null</c>-Werte lassen den Vorlagentext stehen.</summary>
    bool FillValuesInTable(string title, int rowStart, int rowEnd, List<string?[]> values);

    /// <summary>Setzt die Hintergrundfarbe einer Tabellenzelle.</summary>
    bool SetCellBackground(string title, int row, int cell, Color color);

    /// <summary>Entfernt die Tabelle mit dem angegebenen Titel aus dem Dokument.</summary>
    void DeleteTable(string title);
}