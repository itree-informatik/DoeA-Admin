# Drittkomponenten und Lizenzen

DoeA selbst steht unter der [AGPL-3.0-or-later](LICENSE). Diese Datei listet die Drittkomponenten, die DoeA verwendet oder für Build und Betrieb voraussetzt. Massgebliche Versionen stehen in den `csproj`-Dateien; pro Release erzeugt die CI zusätzlich vollständige SBOMs (CycloneDX) inklusive aller transitiven Abhängigkeiten.

## Nicht frei verfügbare Komponenten

Ohne diese beiden Komponenten lässt sich DoeA nicht bauen bzw. nur eingeschränkt betreiben. Sie sind **nicht Bestandteil dieser Veröffentlichung**.

| Komponente | Anbieter | Lizenz | Verwendung in DoeA |
|---|---|---|---|
| ItreeMud | itree informatik GmbH | Proprietär; Bezug nur über den privaten NuGet-Feed `itree-nuget` | Gemeinsame MudBlazor-Komponenten (Buttons, Fenster und Dialoge, Formulare, Seitentitel, Sprachwahl, Theme) |
| GemBox.Document | GemBox Ltd. | Proprietär (EULA); Lizenzschlüssel `LicenseKeys:GemBox`. Ohne Schlüssel läuft die Bibliothek im Free-Modus mit abgeschnittenen Dokumenten | PDF-Erzeugung aus den `.dotx`-Vorlagen in `src/DoeA/Templates/` |

## NuGet-Pakete

### DoeA (Hauptapplikation)

| Paket | Lizenz | Projekt |
|---|---|---|
| FluentValidation | Apache-2.0 | https://fluentvalidation.net/ |
| Microsoft.AspNetCore.Authentication.OpenIdConnect | MIT | https://github.com/dotnet/aspnetcore |
| Microsoft.AspNetCore.DataProtection.EntityFrameworkCore | MIT | https://github.com/dotnet/aspnetcore |
| Microsoft.EntityFrameworkCore | MIT | https://github.com/dotnet/efcore |
| Microsoft.EntityFrameworkCore.Tools (nur Entwicklung) | MIT | https://github.com/dotnet/efcore |
| Npgsql.EntityFrameworkCore.PostgreSQL | PostgreSQL License | https://github.com/npgsql/efcore.pg |
| Npgsql (transitiv) | PostgreSQL License | https://github.com/npgsql/npgsql |
| Riok.Mapperly (Source Generator, nur zur Build-Zeit, nicht im Image) | Apache-2.0 | https://mapperly.riok.app/ |
| Serilog | Apache-2.0 | https://serilog.net/ |
| Serilog.AspNetCore | Apache-2.0 | https://github.com/serilog/serilog-aspnetcore |
| Serilog.Sinks.Console | Apache-2.0 | https://github.com/serilog/serilog-sinks-console |
| Serilog.Sinks.PostgreSQL | Apache-2.0 | https://github.com/b00ted/serilog-sinks-postgresql |
| ClosedXML | MIT | https://github.com/ClosedXML/ClosedXML |
| MudBlazor (transitiv über ItreeMud) | MIT | https://mudblazor.com/ |

### DbUpdater (Init-Container)

| Paket | Lizenz | Projekt |
|---|---|---|
| dbup-postgresql, dbup-core | MIT | https://dbup.github.io/ |
| Microsoft.EntityFrameworkCore | MIT | https://github.com/dotnet/efcore |
| Microsoft.Extensions.Configuration.EnvironmentVariables, .UserSecrets | MIT | https://github.com/dotnet/runtime |
| Npgsql.EntityFrameworkCore.PostgreSQL | PostgreSQL License | https://github.com/npgsql/efcore.pg |

### DataMigration (einmalige Datenübernahme)

| Paket | Lizenz | Projekt |
|---|---|---|
| Microsoft.Data.SqlClient | MIT | https://github.com/dotnet/SqlClient |
| Microsoft.Extensions.Configuration.CommandLine, .EnvironmentVariables, .Json, .UserSecrets | MIT | https://github.com/dotnet/runtime |
| Npgsql | PostgreSQL License | https://github.com/npgsql/npgsql |

## Frontend-Ressourcen (`src/DoeA/wwwroot`)

| Ressource | Lizenz | Herkunft |
|---|---|---|
| Bootstrap 5.1.0 (`bootstrap/`) | MIT | https://getbootstrap.com/ |
| Bootstrap Icons 1.10.5 (`lib/bootstrap-icons/`, Lizenztext dort) | MIT | https://icons.getbootstrap.com/ |
| Noto Sans, Web-Schrift des CD Bund, Variable Font 300 bis 700, Subsets latin und latin-ext (`lib/noto-sans/`, Lizenztext `OFL.txt`) | SIL Open Font License 1.1 | https://fonts.google.com/noto/specimen/Noto+Sans |
| MudBlazor CSS/JS (`_content/MudBlazor`) | MIT | https://mudblazor.com/ |
| Jodit 4.19.0 (`_content/ItreeMud/lib/jodit`, mit ItreeMud ausgeliefert, Lizenztext dort) | MIT | https://xdsoft.net/jodit/ |

Die MudBlazor-Übersetzungen unter `src/DbUpdater/Files/MudBlazor/*.resx` verwenden die Ressourcenschlüssel von MudBlazor (MIT); die deutschen, französischen und italienischen Texte wurden für DoeA erstellt und stehen unter der Lizenz des Projekts.

## Laufzeit- und Entwicklungsumgebung

| Komponente | Lizenz | Bemerkung |
|---|---|---|
| .NET 10 SDK und Runtime, Container-Basisimages `mcr.microsoft.com/dotnet` | MIT | Ubuntu 24.04 (Hauptcontainer), Alpine (Init-Container) |
| libgdiplus, fontconfig | MIT | Im Runtime-Image installiert |
| ttf-mscorefonts-installer | Microsoft-EULA für die Core Fonts | Wird beim Image-Build von Microsoft bezogen, nicht im Repo enthalten |
| PostgreSQL | PostgreSQL License | Datenbank |
| Keycloak (nur lokale Entwicklung) | Apache-2.0 | eIAM-Ersatz, `src/keycloak/` |
| GitHub Actions (`actions/*`, `azure/*`, `docker/*`), CycloneDX, syft | MIT / Apache-2.0 | CI/CD, SBOM-Erzeugung |
