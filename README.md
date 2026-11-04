# DoeA – Dienst öffentliche Ausschreibungen

> **English:** DoeA is the web application of the Swiss Federal Office for Buildings and Logistics (FOBL/BBL) for managing its publications on SIMAP, the Swiss public procurement platform: tenders, lots and bids, awards and revocations, scheduling and reporting. It is a Blazor Server application on .NET 10 with PostgreSQL, authenticates via the federal eIAM (OpenID Connect) and is published as open source under the AGPL-3.0-or-later in accordance with Art. 9 of the Swiss Federal Act on the Use of Electronic Means (EMBAG). **Note:** building the application requires the proprietary NuGet package ItreeMud from a private feed as well as a licence key for GemBox.Document, see [THIRD-PARTY-LICENSES.md](THIRD-PARTY-LICENSES.md). Documentation, code and comments are in German. Contacts: FOBL, Federal Procurement Competence Centre (KBB), info@bbl.admin.ch, for subject matter and operations; itree informatik GmbH (https://www.itree.ch) for security reports, bugs and questions about the code.
>
> **Français :** DoeA est l'application web de l'Office fédéral des constructions et de la logistique (OFCL) pour la gestion de ses publications sur SIMAP, la plateforme suisse des marchés publics : appels d'offres, lots et offres, adjudications et révocations, planification et évaluations. Il s'agit d'une application Blazor Server sur .NET 10 avec PostgreSQL ; l'authentification passe par l'eIAM de la Confédération (OpenID Connect). Le logiciel est publié en open source sous licence AGPL-3.0-or-later conformément à l'art. 9 LMETA. **Remarque :** la compilation nécessite le paquet NuGet propriétaire ItreeMud (dépôt privé) ainsi qu'une clé de licence pour GemBox.Document, voir [THIRD-PARTY-LICENSES.md](THIRD-PARTY-LICENSES.md). La documentation, le code et les commentaires sont en allemand. Contacts : OFCL, Centre de compétence des marchés publics de la Confédération (CCMP), info@bbl.admin.ch, pour les questions métier et l'exploitation ; itree informatik GmbH (https://www.itree.ch) pour les failles de sécurité, les bogues et les questions sur le code.

## Sinn und Zweck

DoeA ist eine webbasierte Applikation für das Bundesamt für Bauten und Logistik (BBL) zur Administration der Publikationen auf SIMAP (Informationssystem über das öffentliche Beschaffungswesen in der Schweiz). Die Applikation deckt den gesamten Lebenszyklus einer Ausschreibung ab:

- **Stammdaten** – Verwaltung von Ämtern, Departementen, Kantonen, Anbietern, Sachbearbeitenden, CPV- und BKP-Nummern
- **Ausschreibungen** – Erfassung und Verwaltung von Ausschreibungen, Losen und Offerten
- **Zuschläge** – Verwaltung von Zuschlägen und Widerrufen
- **Auswertungen** – Berichte zu Abbrüchen und Widerrufen, Zuschlägen, Zielerreichung und Beschaffungscontrolling (PDF- und Excel-Export)
- **Terminplanung** – Verwaltung von Verfahrensschritten und Fristen
- **Exportgruppen** – gespeicherte Filterkombinationen für den Excel-Export
- **Einstellungen** – Konfiguration, Übersetzungen, Log-Einsicht und Benutzerverwaltung (Freischaltung der eIAM-Benutzer)

Die Software unterliegt dem EMBAG (Bundesgesetz über den Einsatz elektronischer Mittel zur Erfüllung von Behördenaufgaben) und wird unter der AGPL-3.0-or-later veröffentlicht, siehe [Lizenz](#lizenz).

## Voraussetzungen

- .NET 10 SDK
- PostgreSQL-Instanz
- Zugang zum privaten NuGet-Feed `itree-nuget` für das Paket **ItreeMud** (GitHub Packages, Personal Access Token erforderlich)
- Lizenzschlüssel für **GemBox.Document**; ohne Schlüssel laufen die PDF-Exporte im Free-Modus mit abgeschnittenen Dokumenten
- Docker für den Container-Build und für Keycloak als lokalen eIAM-Ersatz

**Hinweis für Aussenstehende:** ItreeMud ist proprietär und nicht Teil dieser Veröffentlichung. Ohne Zugang zum Feed lässt sich DoeA nicht bauen. Details zu allen Drittkomponenten stehen in [THIRD-PARTY-LICENSES.md](THIRD-PARTY-LICENSES.md).

## Lokal entwickeln

NuGet-Feed einrichten:

```powershell
dotnet nuget add source https://nuget.pkg.github.com/itree-informatik/index.json --name itree-nuget --username GITHUB_USERNAME --password GITHUB_PAT
```

Konfiguration über User Secrets. Ohne `ConnectionStrings:APP` und die drei `OpenIdConnect`-Werte bricht die Applikation beim Start ab:

```powershell
dotnet user-secrets set "ConnectionStrings:APP" "Host=localhost;Port=5432;Database=doea;Username=postgres;Password=..." --project src/DoeA
dotnet user-secrets set "LicenseKeys:GemBox" "..." --project src/DoeA
dotnet user-secrets set "OpenIdConnect:Authority" "http://localhost:8081/realms/doea" --project src/DoeA
dotnet user-secrets set "OpenIdConnect:ClientId" "doea" --project src/DoeA
dotnet user-secrets set "OpenIdConnect:ClientSecret" "doea-lokal-secret" --project src/DoeA
```

Datenbank aufsetzen und Applikation starten:

```powershell
# Schema, Migrationen und Übersetzungen einspielen (Working Directory muss src/DbUpdater sein,
# da der Pfad zu den MudBlazor-Übersetzungen relativ aufgelöst wird)
cd src/DbUpdater; dotnet run -- "connectionstring=Host=localhost;Port=5432;Database=doea;Username=postgres;Password=..."; cd ../..

# Keycloak als eIAM-Ersatz starten (Admin-Konsole http://localhost:8081, admin/admin)
docker compose -f src/keycloak/docker-compose.yml up -d

# Applikation starten, zwingend über https (siehe Authentifizierung)
dotnet run --project src/DoeA --launch-profile https
```

Die Applikation läuft danach unter `https://localhost:7000`. Testbenutzer im Keycloak-Realm: `hans.muster`, `marie.exemple`, `luca.esempio` (Passwort `doea`). Nach dem ersten Login muss der Benutzer in `dbo.t_user` freigeschaltet werden, siehe [Ablauf und Freischaltung](#ablauf-und-freischaltung).

Die Installation im Produktivbetrieb beschreibt [INSTALL.md](INSTALL.md). Für die einmalige Datenübernahme aus dem Altsystem siehe [src/DataMigration/README.md](src/DataMigration/README.md).

## Architektur

### Solution-Struktur

```
src/
├── DoeA/                Blazor-Server-Applikation (Host + UI + Services + EF Core)
├── DbUpdater/           Datenbank-Migrationen (DbUp) + Sync der MudBlazor-Übersetzungen
│   └── Dockerfile       Init-Container-Build (Alpine, führt Migrationen aus)
├── DataMigration/       Einmalige Datenübernahme SQL Server (Altsystem) → PostgreSQL
├── keycloak/            Lokaler OIDC-Provider (Docker Compose + Realm) als eIAM-Ersatz für die Entwicklung
├── nginx/               Fehlerseite und nginx-Snippet für den Reverse Proxy vor der Applikation
├── Dockerfile           Multi-Stage-Docker-Build der Hauptapplikation
└── DoeA.slnx            Solution-Datei
```

### Vertical Slice Pattern

Jedes Feature folgt demselben Aufbau:

```
DB-Entity (Data/Models/DB/T*.cs)
  → DTO (Data/Models/DTO/*.cs)
    → Mapperly-Mapper (Data/Mapper/)
      → Service-Interface (Interfaces/)
        → Service (Services/)
          → Razor-Komponente (Components/Pages/)
```

### Basisklassen

| Klasse | Zweck |
|--------|-------|
| `Basis` | Abstrakte DTO-Basisklasse mit Audit-Feldern (Id, NaturalId, Aktiv, InsertedDate/User, ChangedDate/User) |
| `BaseService<TModel, TEntity, TValidator>` | Generischer CRUD-Service mit Aktiv-Filter, Sortierung, Suche und Concurrency-Prüfung |
| `BaseComponent` | Basis der Razor-Komponenten mit Clipboard-, Download- und Dropdown-Helfern |

### Namenskonvention der DB-Entities

Alle EF-Core-Entities tragen das Präfix `T` und verwenden ein 3-Buchstaben-Spaltenpräfix. Beispiel: `TAmt` hat die Spalten `AmtId`, `AmtBezeichnung`, `AmtAktiv`. Der `BaseService` löst diese Präfixe per Reflection auf. In PostgreSQL sind Tabellen- und Spaltennamen durchgehend lowercase ohne Anführungszeichen (`T_Amt` → `t_amt`, `AmtBezeichnung` → `amtbezeichnung`).

### Übersetzungen

Datenbankbasierte Mehrsprachigkeit in Deutsch, Französisch und Italienisch (Tabelle `t_translation`), beim Start via `TranslationHelper` geladen. Schlüssel verwenden SCREAMING_SNAKE_CASE (z.B. `ERROR_CONCURRENCY`, `VALIDATOR_REQUIRED`). Fehlt eine Übersetzung, fällt die Anzeige auf Deutsch zurück. MudBlazor-Komponententexte übersetzt der `ItreeMudLocalizer` aus Schlüsseln mit dem Präfix `MUD_`; der DbUpdater gleicht sie mit den `.resx`-Dateien ab.

## Datenbank

Die Applikation verwendet eine **PostgreSQL**-Datenbank mit dem Schema `dbo`. Der DbUpdater führt SQL-Migrationen via DbUp aus; die Skripte liegen unter `src/DbUpdater/Files/Skripts/` (organisiert in Unterverzeichnissen wie `Rel0000`). Ausgeführte Skripte werden in der Tabelle `schemaversions` protokolliert und nie erneut ausgeführt. Zusätzlich synchronisiert der DbUpdater die MudBlazor-Übersetzungen aus `.resx`-Dateien in die Tabelle `t_translation`.

Der DbUpdater wird als eigener Init-Container (`doea-init`, Alpine-basiert) gebaut und vor dem Start der Hauptapplikation einmalig ausgeführt. Der Connection-String kommt aus der Umgebungsvariable `CONNECTIONSTRING`.

Datumswerte werden als `bigint` (.NET `DateTime.Ticks`) gespeichert; einzige Ausnahme ist die von Serilog geschriebene Spalte `timestamp` in `t_log`.

## Container

Dieser Abschnitt beschreibt Aufbau und Build der Images. Die vollständige Installation — Datenbank, Init-Container, Umgebungsvariablen, Reverse Proxy, Freischaltung des ersten Benutzers — steht in [INSTALL.md](INSTALL.md).

### Build

Der NuGet-PAT wird als BuildKit-Secret `nuget_gh_pat` eingespeist, nicht als Build-Arg, und landet dadurch weder in einem Image-Layer noch im Build-Cache:

```bash
cd src
export GH_PAT=<pat>
docker build -t doea:latest \
    --secret id=nuget_gh_pat,env=GH_PAT \
    --build-arg ASSEMBLY_VERSION=1.0.0 \
    --build-arg ASSEMBLY_INFO_VERSION=1.0.0 \
    -f Dockerfile .
```

### Multi-Stage-Build

| Stage | Base Image | Zweck |
|-------|------------|-------|
| `build` | `mcr.microsoft.com/dotnet/sdk:10.0-noble` | Restore, Build und Publish |
| `base` | `mcr.microsoft.com/dotnet/aspnet:10.0-noble` | Runtime (Ubuntu 24.04 Noble) |

### Build-Argumente

| Argument | Zweck |
|----------|-------|
| `ASSEMBLY_VERSION` | Versionsnummer der Assembly |
| `ASSEMBLY_INFO_VERSION` | Informationsversion (z.B. mit Branch-Suffix) |
| `GEMBOX_LICENSEKEY` | GemBox-Lizenzschlüssel, im Image als `LicenseKeys__GemBox` |

### Container starten

```bash
docker run -d --name doea -p 8080:8080 \
    -e ConnectionStrings__APP="Host=...;Port=5432;Database=...;Username=...;Password=...;" \
    -e OpenIdConnect__Authority="https://..." \
    -e OpenIdConnect__ClientId="..." \
    -e OpenIdConnect__ClientSecret="..." \
    doea:latest
```

Der Connection-String mit dem Schlüssel `APP` und die drei `OpenIdConnect`-Werte sind zwingend, ohne sie startet die Applikation nicht. Produktiv steht ein TLS-terminierender Proxy vor dem Container, siehe [Authentifizierung](#authentifizierung-eiam).

### Runtime-Konfiguration

| Einstellung | Wert |
|-------------|------|
| Port | 8080 |
| Locale | de_CH.UTF-8 |
| Zeitzone | Europe/Zurich |
| Environment | Production |
| Forwarded Headers | `ASPNETCORE_FORWARDEDHEADERS_ENABLED=true`, weil TLS am Proxy endet; ohne `X-Forwarded-Proto` würde der OIDC-Handler `http`-Redirect-URIs bilden |
| Health Check | `HEALTHCHECK` im Image gegen `GET /health` (Liveness, anonym, ohne Datenbankprüfung), siehe [INSTALL.md](INSTALL.md#health-check) |

### Zusätzliche Pakete im Runtime-Image

- **libgdiplus** – GDI+-Kompatibilität für die Dokumentgenerierung
- **fontconfig + ttf-mscorefonts-installer** – Microsoft Core Fonts für die korrekte Darstellung der PDF-Exporte
- **wget** – Probe für den `HEALTHCHECK` des Images (`GET /health`)

### Sicherheitsmassnahmen

- **Rootless** – der Container läuft unter dem dedizierten Benutzer `doea` (UID 30120, GID 30120)
- **Aktualisierte Pakete** – `apt-get upgrade` beim Image-Build, Paketlisten werden entfernt
- **Keine Credentials im Image** – die temporäre NuGet-Konfiguration mit dem PAT wird im selben Build-Schritt wieder gelöscht, Lizenzschlüssel kommen nur als Build-Args ins Runtime-Image

### Fehlerseite des Reverse Proxy

Antwortet der Container nicht (Neustart, Wartungsfenster, Störung), zeigt der Browser die Standardmeldung des Proxy. `src/nginx/` enthält dafür eine Fehlerseite im Erscheinungsbild der Applikation:

| Datei | Zweck |
|-------|-------|
| `fehlerseite.html` | Die Seite selbst: dreisprachig (de/fr/it), ohne Stylesheet, Bild oder Skript aus `wwwroot` – diese wären im Fehlerfall ebenfalls nicht erreichbar, deshalb steckt das Logo als Data-URI im Markup |
| `fehlerseite.conf` | nginx-Snippet mit `error_page` und dem passenden `location`-Block |

Einrichtung: `fehlerseite.html` in das `root`-Verzeichnis des Proxy kopieren (im nginx-Containerimage `/usr/share/nginx/html`) und `fehlerseite.conf` im `server`-Block einbinden, der auf DoeA weiterleitet.

Die Seite deckt bewusst nur den Fall ab, dass die Applikation gar nicht antwortet (502/503/504). Fehler der laufenden Applikation bleiben bei ihr: den Anmelde-Redirect zum eIAM und die Seite `/Error` darf der Proxy nicht ersetzen, deshalb kein `proxy_intercept_errors`. Bricht die Verbindung während einer offenen Sitzung ab, zeigt Blazor zuerst seinen eigenen Wiederverbindungsdialog – die Fehlerseite erscheint erst beim nächsten Seitenaufruf.

## Authentifizierung (eIAM)

DoeA authentisiert über das eIAM des Bundes (OpenID Connect, Authorization Code Flow mit PKCE und Client Secret, eIAM-Muster «Klassisch»: das ID-Token baut die Cookie-Session, danach zählt nur noch das Cookie). eIAM authentisiert alle Bundesbenutzer; **autorisiert** ist nur, wer in `dbo.t_user` aktiv ist.

### Konfiguration

| Schlüssel | Umgebungsvariable im Container | Zweck |
|-----------|-------------------------------|-------|
| `OpenIdConnect:Authority` | `OpenIdConnect__Authority` | Issuer-URL des eIAM (Discovery über `/.well-known/openid-configuration`) |
| `OpenIdConnect:ClientId` | `OpenIdConnect__ClientId` | Client-ID aus dem eIAM-Dossier |
| `OpenIdConnect:ClientSecret` | `OpenIdConnect__ClientSecret` | Client Secret, nur als App-Setting der Web App bzw. User Secret, nie im Image oder Repo |

Fehlt einer der Werte, startet die Applikation nicht; ausserhalb von Development muss die Authority `https` sein. Ein Timeout-Schlüssel existiert bewusst nicht, siehe [Session](#session).

### Bei eIAM zu registrieren

| Angabe | Wert |
|--------|------|
| Redirect-URI | `https://<host>/signin-oidc` |
| Post-Logout-Redirect-URI | `https://<host>/signout-callback-oidc` |
| Logoutseite (falls das Dossier eine eigene verlangt) | `https://<host>/anmeldung`; die Seite bestätigt die Abmeldung und bietet «Erneut anmelden», wie von eIAM gefordert |
| Front-Channel-Logout-URI (LogoutURI im Dossier, optional) | `https://<host>/signout-oidc`; kann weggelassen werden, weil das Cookie `DoeA.Auth` mit `SameSite=Lax` im Logout-iframe des eIAM weder mitgeschickt noch gelöscht wird |
| Flow | Authorization Code + PKCE, Client Secret, `response_mode=form_post` |
| Scopes | `openid profile email` |
| Claims | `sub`, `given_name`, `family_name`, `email`, `language` |

Für die Entwicklung gelten dieselben Pfade unter `https://localhost:7000`.

### Ablauf und Freischaltung

1. Unangemeldete Aufrufe werden direkt zu eIAM geleitet, eine eigene Login-Seite gibt es nicht. Die vom eIAM reservierten Pfade und Parameter (`/login`, `/logout`, `/auth`, `?login`, `?logout`) werden nicht verwendet; DoeA nutzt `/anmeldung` (Statusseite) und `/abmeldung`.
2. Nach dem Login sucht DoeA den Benutzer über den Claim `sub` in `t_user` (`usrextid`). Unbekannte Benutzer werden **inaktiv** angelegt (Vorname, Nachname, E-Mail und Sprache aus den Claims) und sehen auf `/anmeldung` den Hinweis «noch nicht freigeschaltet»; im Log erscheint eine Warnung.
3. Die Freischaltung erfolgt unter **Einstellungen → Benutzer** (Filter Aktive / Inaktive / Neue, Textsuche). Der eigene Benutzer kann nicht deaktiviert werden. Ein freigeschalteter Benutzer kommt über «Erneut prüfen» ohne neues Login hinein; ein deaktivierter fliegt beim nächsten Request, im offenen Browserfenster spätestens nach 5 Minuten.
4. Bei jedem Login werden Vorname, Nachname und E-Mail aus den Claims übernommen; `usrlastlogin` wird nur bei erfolgreicher Autorisierung gesetzt. Die UI-Sprache (`usrsprache`) kommt beim Anlegen aus dem Claim `language` und danach ausschliesslich aus der Sprachwahl in der Kopfzeile.

**Erster Administrator:** Nach dessen erstem Login einmalig per SQL freischalten:

```sql
update dbo.t_user set usraktiv = true where usrextid = '<sub aus eIAM>';
```

### Session

| Parameter | Wert | Quelle |
|-----------|------|--------|
| Inaktivität (Sliding-Ablauf des Cookies) | 120 min | eIAM-Standard, Inactivity Interval 7'200 s |
| Maximale Dauer ab Login | 12 h | eIAM-Standard, Session-Lifetime 43'200 s |
| Warnung vor Ablauf | 10 min | DoeA-Wert |
| Heartbeat | 5 min | DoeA-Wert |
| Aktivitätsprüfung | 30 s | DoeA-Wert |

Alle Werte sind Konstanten in `AuthKonstanten`. Nach Ablauf führt der nächste HTTP-Request zurück zu eIAM; lebt die SSO-Session dort noch, geschieht das ohne Benutzerinteraktion. Abmelden über die Kopfzeile beendet Cookie und eIAM-Session (RP-initiated Logout über den `end_session_endpoint`) und führt auf `/anmeldung` («Sie wurden abgemeldet», «Erneut anmelden»).

Der Logout ist nur gegen Keycloak verifiziert, eine eIAM-Teststufe steht nicht zur Verfügung. Er ist deshalb so gebaut, dass er ohne Codeänderung in jeder dokumentierten eIAM-Variante auf `/anmeldung` endet: Liefert die Discovery keinen `end_session_endpoint`, wird nur das Cookie abgemeldet (Warnung im Log) und direkt weitergeleitet. Kehrt eIAM nach dem Logout auf `/signout-callback-oidc` zurück, leitet der OIDC-Handler mit oder ohne `state` auf `/anmeldung` weiter. Leitet eIAM stattdessen auf eine im Dossier registrierte Logoutseite, ist das `/anmeldung` selbst; das Cookie ist zu diesem Zeitpunkt bereits gelöscht. Der Abmeldegrund hängt von keiner dieser Varianten ab, er liegt im `sessionStorage` des Browsers.

Das Cookie wird nur bei HTTP-Requests geprüft und verlängert, Blazor Server arbeitet nach dem Seitenaufbau aber über SignalR. Die Inaktivitätsregel setzt deshalb der Browser durch (`wwwroot/js/sitzung.js`): Aktivität ist jeder Klick, jede Taste und jede Eingabe (kein Scrollen, keine Mausbewegung); der Zeitpunkt der letzten Aktivität liegt im `localStorage` und gilt für alle Tabs desselben Browsers. Solange ein Tab offen ist, hält ein Heartbeat alle 5 Minuten das Cookie am Leben; nach dem Schliessen des Browsers stirbt es spätestens 120 Minuten später. Nach 110 Minuten ohne Aktivität erscheint ein Dialog mit Countdown («Sitzung verlängern»), nach 120 Minuten meldet der Browser über `/abmeldung` ab (RP-initiated Logout) und `/anmeldung` nennt den Grund; den merkt sich der Browser im `sessionStorage` des Tabs, weil eIAM beim Logout keine Parameter zurück zur Applikation trägt. Nach einem Standby führt das Aufwachen sofort zur Abmeldung, wenn die Inaktivität überschritten ist. Ein manueller Logout in einem Tab erreicht die anderen Tabs beim nächsten Heartbeat. Die 12-Stunden-Grenze wird nicht angekündigt; «Sitzung verlängern» verlängert sie nicht.

Die Data-Protection-Schlüssel zur Verschlüsselung des Auth-Cookies liegen in `dbo.t_dataprotectionkey`, damit Cookies Container-Neustarts und mehrere Instanzen überleben; sie sind dort nicht zusätzlich verschlüsselt.

**Abstimmung mit dem eIAM-Betrieb:** Die eIAM-Anforderungen «applikatorische Timeouts höher als am PEP», reservierter Pfad `/favicon.ico`, kein `<base href>` und relative URLs betreffen die Integration hinter dem eIAM-Web-PEP. DoeA ist direkt per OIDC angebunden und braucht als Blazor-App `<base href="/">`.

### Lokal ohne eIAM (Keycloak)

`src/keycloak/docker-compose.yml` startet ein Keycloak mit dem Realm `doea`: Client `doea` (Secret `doea-lokal-secret`), Testbenutzer `hans.muster`, `marie.exemple`, `luca.esempio` (Passwort `doea`) mit dem Attribut `language` als Claim. Die App muss über `https://localhost:7000` laufen: Die OIDC-Antwort kommt als Cross-Site-POST (`form_post`), dessen Correlation-Cookies `Secure` sind.

Das Fehlerbild «Correlation failed» bedeutet praktisch immer: App über `http` statt `https`, oder im Container fehlende Forwarded Headers.

## Continuous Integration

Der Build läuft als GitHub-Actions-Workflow:

| Job | Trigger | Inhalt |
|-----|---------|--------|
| `versioning` + `build` | Push auf `develop`/`main`, alle Pull Requests, manuell | .NET Restore und Build; es gibt kein Testprojekt und keinen Test-Step |
| `publish` | Push auf `main`; Push auf `develop` nur, wenn die Repo-Variable `DEVELOP_PUBLISH` auf `true` steht | Docker-Images bauen und in die Container Registry pushen (`main` → `doea`/`doea-init`, `develop` → `doeadev`/`doeadev-init`); nur auf `main` zusätzlich SBOMs generieren (CycloneDX + syft), nach Dependency-Track hochladen und Git-Tag `release-<version>` setzen |

Pro Release werden zwei SBOMs generiert und in separate Dependency-Track-Projekte hochgeladen: eines für die Hauptapplikation (DoeA-Solution + Hauptcontainer), eines für den Init-Container (DbUpdater-Projekt + Init-Container).

## Drittkomponenten

Alle verwendeten Pakete, Frontend-Ressourcen und Laufzeitkomponenten mit ihren Lizenzen sind in [THIRD-PARTY-LICENSES.md](THIRD-PARTY-LICENSES.md) aufgeführt. Die massgeblichen Versionen stehen in den `csproj`-Dateien.

## Mitwirken und Kontakt

Hinweise für Fehlermeldungen, Vorschläge und Pull Requests stehen in [CONTRIBUTING.md](CONTRIBUTING.md); für alle Beteiligten gilt der [Verhaltenskodex](CODE_OF_CONDUCT.md). Änderungen werden im [CHANGELOG.md](CHANGELOG.md) festgehalten.

**Fachliche Anlaufstelle und Betrieb der Software:**

Bundesamt für Bauten und Logistik BBL
Kompetenzzentrum Beschaffungswesen Bund KBB
Fellerstrasse 21, 3003 Bern
info@bbl.admin.ch
https://www.bbl.admin.ch/de/kompetenzzentrum-beschaffungswesen-bund-kbb

**Technische Ansprechstelle (Entwicklung und Wartung) für Sicherheitslücken, Fehler und Fragen zum Code:**

itree informatik GmbH
Bernstrasse 41, 3303 Jegenstorf
https://www.itree.ch

## Lizenz

Dieses Projekt steht unter der [AGPL-3.0-or-later](LICENSE) und wird nach Art. 9 EMBAG veröffentlicht. Urheberin ist die Schweizerische Eidgenossenschaft, vertreten durch das Bundesamt für Bauten und Logistik (BBL). Entwickelt wurde die Software im Auftrag des BBL von der itree informatik GmbH.

Das verwendete NuGet-Paket **ItreeMud** ist proprietär und nicht Bestandteil dieser Veröffentlichung. Es verbleibt im alleinigen Urheberrecht der itree informatik GmbH.
