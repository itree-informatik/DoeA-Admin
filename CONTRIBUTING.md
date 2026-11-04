# Mitwirken an DoeA

DoeA ist eine Fachanwendung des Bundesamts für Bauten und Logistik (BBL) und wird nach Art. 9 EMBAG als Open Source unter der [AGPL-3.0-or-later](LICENSE) veröffentlicht. Weiterentwicklung und Betrieb erfolgen im Auftrag des BBL durch die itree informatik GmbH. Fachliche Erweiterungen entscheidet das BBL; Fehlermeldungen, Sicherheitshinweise, Verbesserungsvorschläge und Korrekturen sind willkommen.

## Bevor du beginnst

**Bauen ist ohne Zugang zum privaten Feed nicht möglich.** DoeA verwendet das proprietäre Paket ItreeMud aus dem NuGet-Feed `itree-nuget` sowie einen Lizenzschlüssel für GemBox.Document (siehe [THIRD-PARTY-LICENSES.md](THIRD-PARTY-LICENSES.md)). Externe Beiträge deshalb bevorzugt als Issue mit konkretem Vorschlag oder als Pull Request einreichen, den das Projektteam baut und prüft.

## Sprache

Code, Kommentare, Commit-Messages und Dokumentation sind Deutsch in Schweizer Schreibweise (`ss` statt `ß`). Issues und Pull-Request-Beschreibungen dürfen auch auf Französisch, Italienisch oder Englisch verfasst sein.

## Issues

- Fehler: Schritte zum Nachstellen, erwartetes und tatsächliches Verhalten, Version (Fusszeile der Anwendung), Browser.
- Vorschläge: Nutzen für die Anwenderinnen und Anwender des BBL kurz begründen.
- **Sicherheitslücken nicht als öffentliches Issue melden**, sondern direkt an die technische Ansprechstelle: itree informatik GmbH, Bernstrasse 41, 3303 Jegenstorf, https://www.itree.ch. Fachliche Fragen zur Anwendung beantwortet das BBL (info@bbl.admin.ch), siehe README.

## Pull Requests

1. Branch von `main` erstellen, kleine und fokussierte Änderungen.
2. Konventionen einhalten: Stil des umgebenden Codes übernehmen, Formatierung gemäss `.editorconfig`.
3. Datenbankänderungen ausschliesslich als neues DbUp-Skript im aktuellen `Rel`-Ordner; bereits ausgelieferte Skripte nie ändern. Neue Übersetzungen ebenfalls per Skript.
4. `dotnet build src/DoeA.slnx` muss fehlerfrei sein; das Projektteam baut jeden Pull Request.
5. Beschreibung mit *Was* und *Warum*, Bezug auf das Issue.

## Lizenz der Beiträge

Mit einem Beitrag erklärst du dich einverstanden, dass er unter der AGPL-3.0-or-later veröffentlicht wird. Neue Quelldateien erhalten denselben SPDX-Kopf wie die bestehenden:

```csharp
// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL
```

## Verhaltenskodex

Für alle Beteiligten gilt der [Verhaltenskodex](CODE_OF_CONDUCT.md).
