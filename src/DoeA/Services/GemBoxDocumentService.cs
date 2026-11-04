// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Constants;
using DoeA.Interfaces;
using GemBox.Document;
using GemBox.Document.Tables;
using ILogger = Serilog.ILogger;
using SaveOptions = GemBox.Document.SaveOptions;

namespace DoeA.Services;

/// <inheritdoc cref="IGemBoxDocumentService"/>
public class GemBoxDocumentService : IGemBoxDocumentService
{
    /// <summary>Konfigurationsschlüssel des GemBox-Lizenzschlüssels (Container: ENV LicenseKeys__GemBox).</summary>
    private const string LizenzKonfigSchluessel = "LicenseKeys:GemBox";

    /// <summary>Pseudo-Lizenz für den Free-Modus; ohne SetLicense-Aufruf wirft GemBox «License not set».</summary>
    private const string FreieLizenz = "FREE-LIMITED-KEY";

    /// <summary>Platzhalter, der in jeder Vorlage ersatzlos entfernt wird.</summary>
    private const string PlatzhalterInitial = "[%initial%]";

    /// <summary>Markiert paarweise den Anfang und das Ende eines Bereichs, der gelöscht werden soll.</summary>
    private const string PlatzhalterBereich = "[%none%]";

    /// <summary>Zeilentyp in <see cref="FillTable"/>, der einen Seitenumbruch mit neuer Tabelle auslöst.</summary>
    private const string ZeilentypSeitenumbruch = "PB";

    private readonly ILogger _logger;

    public GemBoxDocumentService(ILogger logger, IConfiguration configuration)
    {
        _logger = logger;

        // Ohne Schlüssel läuft GemBox im Free-Modus und schneidet Dokumente ab —
        // deshalb die Warnung statt stiller Degradierung.
        var licenseKey = configuration.GetValue<string>(LizenzKonfigSchluessel);
        if (string.IsNullOrWhiteSpace(licenseKey))
        {
            _logger.Warning($"{LizenzKonfigSchluessel} ist nicht gesetzt — GemBox läuft im eingeschränkten Free-Modus.");
            ComponentInfo.SetLicense(FreieLizenz);
        }
        else
        {
            ComponentInfo.SetLicense(licenseKey);
        }
    }

    /// <summary>Das aktuell geladene Dokument.</summary>
    public DocumentModel WordDocumentModel { get; private set; } = null!;

    /// <summary>Lädt die Vorlage aus dem Dateisystem; false, wenn der Pfad leer ist oder die Datei fehlt.</summary>
    public bool Load(string filename)
    {
        if (string.IsNullOrWhiteSpace(filename) || !File.Exists(filename))
        {
            return false;
        }

        WordDocumentModel = DocumentModel.Load(filename);
        return true;
    }

    /// <summary>Lädt die Vorlage aus einem Byte-Array.</summary>
    public bool Load(byte[] content)
    {
        WordDocumentModel = DocumentModel.Load(new MemoryStream(content));
        return true;
    }

    /// <summary>Speichert das Dokument unter dem angegebenen Pfad.</summary>
    public void Save(string documentname)
    {
        WordDocumentModel.Save(documentname);
    }

    /// <summary>Speichert das Dokument im gewünschten Format in einen Stream (aktualisiert das Erstelldatum).</summary>
    public MemoryStream GetStream(SaveOptions saveOption)
    {
        var stream = new MemoryStream();
        try
        {
            UpdateErstellungsdatum();
            WordDocumentModel.Save(stream, saveOption);
        }
        catch (Exception ex)
        {
            var message = ex.InnerException != null
                ? $"{ex.Message} {Environment.NewLine}InnerException: {ex.InnerException.Message}"
                : ex.Message;
            _logger.Error(GeneralConstants.MessageTemplate, nameof(GemBoxDocumentService), nameof(GetStream), message);
            throw new InvalidOperationException(GeneralConstants.HandledException);
        }

        return stream;
    }

    /// <summary>Ersetzt Platzhalter im gesamten Dokument (Schlüssel = Platzhalter, Wert = Text).</summary>
    public void InsertValues(Dictionary<string, string> dictionary)
    {
        // Steuerplatzhalter vor den Werten auflösen: Benutzertext darf keinen Bereich markieren oder löschen.
        ErsetzeAlle(PlatzhalterInitial, string.Empty);
        EntferneBereiche();

        // Zweistufig über Marken, die kein Benutzer kennt: Enthält ein Wert einen Platzhalter, würde ihn die Suche sonst
        // wiederfinden — beim eigenen endlos (Endlosschleife), bei einem anderen als dessen Wert.
        var marken = dictionary.Keys.ToDictionary(platzhalter => platzhalter, _ => $"[%{Guid.NewGuid():N}%]");
        foreach (var (platzhalter, marke) in marken)
        {
            ErsetzeAlle(platzhalter, marke);
        }

        foreach (var entry in dictionary)
        {
            ErsetzeAlle(marken[entry.Key], entry.Value);
        }
    }

    /// <summary>Schreibt Werte in einen bestehenden Zeilenbereich; <c>null</c>-Werte lassen den Vorlagentext stehen.</summary>
    public bool FillValuesInTable(string title, int rowStart, int rowEnd, List<string?[]> values)
    {
        var table = FindeTabelle(title);
        if (table == null)
        {
            return false;
        }

        for (var rowcount = rowStart; rowcount <= rowEnd; rowcount++)
        {
            var zeilenwerte = values[rowcount - rowStart];
            for (var colcount = 0; colcount < table.Columns.Count; colcount++)
            {
                if (zeilenwerte[colcount] != null)
                {
                    table.Rows[rowcount].Cells[colcount].Content.LoadText(zeilenwerte[colcount]!);
                }
            }
        }

        return true;
    }

    /// <summary>Setzt die Hintergrundfarbe einer Tabellenzelle.</summary>
    public bool SetCellBackground(string title, int row, int cell, Color color)
    {
        var table = FindeTabelle(title);
        if (table == null)
        {
            return false;
        }

        table.Rows[row].Cells[cell].CellFormat.BackgroundColor = color;
        return true;
    }

    /// <summary>Füllt eine Tabelle über Platzhalter und dupliziert dabei den Zeilenblock je Datensatz.</summary>
    public bool InsertValuesInTable(string title, int rowStart, int rowEnd, IReadOnlyList<string> placeholders,
        IReadOnlyList<string[]> values)
    {
        var table = FindeTabelle(title);
        if (table == null)
        {
            return false;
        }

        var zeilen = new List<TableRow>();
        // Zeilen unterhalb des Blocks (z.B. Total) bleiben stehen; neue Blöcke werden davor eingefügt.
        var trailingrows = table.Rows.Count - rowEnd - 1;
        for (var rowcount = rowStart; rowcount <= rowEnd; rowcount++)
        {
            zeilen.Add(table.Rows[rowcount].Clone(true));
        }

        for (var rowcount = 0; rowcount < values.Count; rowcount++)
        {
            for (var cellcount = 0; cellcount < placeholders.Count; cellcount++)
            {
                var cell = WordDocumentModel.Content.Find(placeholders[cellcount]).FirstOrDefault();
                if (cell != null)
                {
                    cell.LoadText(values[rowcount][cellcount]);
                }
            }

            if (rowcount < values.Count - 1)
            {
                foreach (var zeile in zeilen)
                {
                    table.Rows.Insert(table.Rows.Count - trailingrows, zeile.Clone(true));
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Baut eine Tabelle aus Zeilenvorlagen neu auf: <paramref name="definition"/> hält je Zeilentyp
    /// [Typ, Index der Vorlagenzeile, geklonte Vorlagenzeile], <paramref name="liste"/> die Daten
    /// mit dem Zeilentyp an Position 0.
    /// </summary>
    public bool FillTable(string title, IList<object?[]> definition, IList<string[]> liste)
    {
        var table = FindeTabelleOderFehler(title);

        // Anzahl Tabellenspalten muss zur breitesten Datenzeile passen (ohne die Typ-Spalte).
        var columnCountListe = 0;
        foreach (var entry in liste)
        {
            if (entry.Length > columnCountListe)
            {
                columnCountListe = entry.Length - 1;
            }
        }

        if (table.Columns.Count != columnCountListe)
        {
            throw new InvalidOperationException($"{GeneralConstants.DocTemplateTableColumns}?{title}");
        }

        // Die Vorlage muss für jeden definierten Zeilentyp genau eine Zeile mitbringen.
        if (table.Rows.Count != definition.Count)
        {
            throw new InvalidOperationException($"{GeneralConstants.DocTemplateTableRows}?{title}");
        }

        // Vorlagenzeilen sichern, danach die Tabelle leeren und Zeile für Zeile neu aufbauen.
        foreach (var row in definition)
        {
            var rowString = row[1]?.ToString();
            if (!string.IsNullOrEmpty(rowString))
            {
                row[2] = table.Rows[int.Parse(rowString)].Clone(true);
            }
        }

        while (table.Rows.Count > 0)
        {
            table.Rows.RemoveAt(0);
        }

        var newTable = table.Clone(true);

        foreach (var row in liste)
        {
            if (row[0] == ZeilentypSeitenumbruch)
            {
                if (table.Parent != null)
                {
                    var index = WordDocumentModel.Sections.IndexOf(table.Parent);
                    var indexBlock = WordDocumentModel.Sections[index].Blocks.IndexOf(table);
                    var pageBreak = new SpecialCharacter(WordDocumentModel, SpecialCharacterType.PageBreak);
                    var paragraph = new Paragraph(WordDocumentModel, new Run(WordDocumentModel, string.Empty), pageBreak);
                    WordDocumentModel.Sections[index].Blocks.Insert(indexBlock + 1, paragraph);
                    WordDocumentModel.Sections[index].Blocks.Insert(indexBlock + 2, newTable);
                }

                table = newTable;
                continue;
            }

            foreach (var obj in definition)
            {
                if (obj[0]?.ToString() != row[0])
                {
                    continue;
                }

                table.Rows.Add(((TableRow)obj[2]!).Clone(true));
                for (var counter = 1; counter < row.Length; counter++)
                {
                    // Trotz nicht-nullable Signatur nötig: Aufrufer übergeben für leere Zellen null! (z.B. die Kopfzeile des Terminplans).
                    if (row[counter] != null)
                    {
                        var par = table.Rows[^1].Cells[counter - 1].Blocks[0].Content
                            .GetChildElements(ElementType.Paragraph).Cast<Paragraph>().FirstOrDefault();
                        par?.Content.LoadText(row[counter]);
                    }
                }

                break;
            }
        }

        return true;
    }

    /// <summary>Fasst Zellen einer Tabellenzeile horizontal zusammen.</summary>
    public void ColumnSpan(string altText, int row, int cell, int span)
    {
        FindeTabelleOderFehler(altText).Rows[row].Cells[cell].ColumnSpan = span;
    }

    /// <summary>Fasst Zellen einer Tabellenspalte vertikal zusammen.</summary>
    public void RowSpan(string altText, int row, int cell, int span)
    {
        FindeTabelleOderFehler(altText).Rows[row].Cells[cell].RowSpan = span;
    }

    /// <summary>Entfernt die Tabelle mit dem angegebenen Titel aus dem Dokument.</summary>
    public void DeleteTable(string title)
    {
        foreach (var section in WordDocumentModel.Sections)
        {
            for (var i = 0; i < section.Blocks.Count; i++)
            {
                if (section.Blocks[i] is Table table && table.Metadata.Title == title)
                {
                    section.Blocks.RemoveAt(i);
                    break;
                }
            }
        }
    }

    /// <summary>Wandelt ein Word-Dokument aus dem Dateisystem in ein PDF um.</summary>
    public MemoryStream WordToPdf(string path)
    {
        var doc = DocumentModel.Load(path);
        var stream = new MemoryStream();
        doc.Save(stream, SaveOptions.PdfDefault);
        return stream;
    }

    /// <summary>Ersetzt jedes Vorkommen von <paramref name="suchtext"/>; der Text darf den Suchtext nicht enthalten.</summary>
    private void ErsetzeAlle(string suchtext, string text)
    {
        // Nach jedem LoadText neu suchen: Der ersetzte Bereich ist danach nicht mehr gültig.
        var treffer = WordDocumentModel.Content.Find(suchtext).FirstOrDefault();
        while (treffer != null)
        {
            treffer.LoadText(text);
            treffer = WordDocumentModel.Content.Find(suchtext).FirstOrDefault();
        }
    }

    /// <summary>Löscht jeden Bereich zwischen zwei <see cref="PlatzhalterBereich"/> samt den Platzhaltern.</summary>
    private void EntferneBereiche()
    {
        var start = WordDocumentModel.Content.Find(PlatzhalterBereich).FirstOrDefault();
        while (start != null)
        {
            var end = WordDocumentModel.Content.Find(PlatzhalterBereich).Skip(1).FirstOrDefault();
            if (end == null)
            {
                // Ohne schliessenden Platzhalter ist der Bereich nicht bestimmbar, deshalb abbrechen.
                break;
            }

            new ContentRange(start.Start, end.End).LoadText(string.Empty);
            start = WordDocumentModel.Content.Find(PlatzhalterBereich).FirstOrDefault();
        }
    }

    /// <summary>
    /// Aktualisiert die Erstelldatum-Felder (z.B. «Erstellt:» in den Fusszeilen): Die
    /// Vorlagen enthalten CREATEDATE-Felder mit dem gecachten Datum der Vorlage; wie bei
    /// einem neu aus der Vorlage erzeugten Dokument soll der Generierungszeitpunkt
    /// erscheinen (das Zahlenformat kommt aus dem Format-Schalter des Felds).
    /// </summary>
    private void UpdateErstellungsdatum()
    {
        WordDocumentModel.DocumentProperties.BuiltIn[BuiltInDocumentProperty.DateContentCreated] =
            DateTime.Now.ToString("o");
        foreach (var feld in WordDocumentModel.GetChildElements(true, ElementType.Field)
                     .Cast<Field>().Where(f => f.FieldType == FieldType.CreateDate).ToList())
        {
            feld.Update();
        }
    }

    /// <summary>
    /// Sucht die Tabelle über den Titel im Alternativtext der Tabelleneigenschaften – Word bietet
    /// keine andere stabile Möglichkeit, eine Tabelle in der Vorlage zu benennen.
    /// </summary>
    private Table? FindeTabelle(string title)
    {
        return WordDocumentModel.GetChildElements(true, ElementType.Table)
            .Cast<Table>().FirstOrDefault(t => t.Metadata.Title == title);
    }

    /// <summary>Wie <see cref="FindeTabelle"/>, wirft aber, wenn die Vorlage die Tabelle nicht enthält.</summary>
    private Table FindeTabelleOderFehler(string title)
    {
        return FindeTabelle(title)
               ?? throw new InvalidOperationException($"{GeneralConstants.DocTemplateTable}?{title}");
    }
}
