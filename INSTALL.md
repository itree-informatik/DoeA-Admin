# Installation und Betrieb

Diese Anleitung beschreibt die Installation von DoeA im **Produktivbetrieb als Container**. Für die lokale Entwicklungsumgebung siehe [README, Abschnitt «Lokal entwickeln»](README.md#lokal-entwickeln); für die einmalige Datenübernahme aus dem Altsystem siehe [src/DataMigration/README.md](src/DataMigration/README.md).

## Zielbild

DoeA besteht aus zwei Images: der Applikation (`doea`) und einem Init-Container (`doea-init`), der die Datenbank auf den passenden Stand bringt und sich danach beendet. TLS endet auf einem vorgelagerten Reverse Proxy, die Benutzeranmeldung läuft über das eIAM des Bundes.

```
                                  ┌──────────────┐
Browser ──https──► Reverse Proxy ─┤ doea :8080   ├──► PostgreSQL
                   (TLS, WSS)     └──────┬───────┘         ▲
                                         │ OIDC            │
                                         ▼                 │
                                       eIAM       doea-init (einmalig
                                                  vor jedem Start)
```

Die Applikation hält keinen Zustand auf der Festplatte: Sitzungsschlüssel liegen in der Datenbank, erzeugte Exportdateien nur im Arbeitsspeicher. Ein Volume ist deshalb nicht nötig.

## 1. Voraussetzungen

| Komponente | Anforderung | Bemerkung |
|------------|-------------|-----------|
| Container-Laufzeit | beliebig (Docker, Kubernetes, App Service für Container) | Der Init-Container muss vor der Applikation laufen können |
| PostgreSQL | Version 13 oder neuer, Encoding UTF8 | `gen_random_uuid()` wird ohne Extension vorausgesetzt (seit PostgreSQL 13 im Kern) |
| Reverse Proxy | TLS-Terminierung, WebSocket-Weiterleitung, Forwarded Headers | Blazor Server hält eine dauerhafte SignalR-Verbindung |
| eIAM | registriertes Dossier (OIDC-Client), Netzwerkfreigabe vom Container zur Authority | siehe [Schritt 5](#5-eiam-dossier-registrieren) |
| Container Registry | Lesezugriff auf die beiden Images `doea` und `doea-init` | dorthin publiziert die CI |

Die Images werden fertig ausgeliefert und sind ohne weitere Zutaten lauffähig: Das proprietäre Paket **ItreeMud** und der Lizenzschlüssel für **GemBox.Document** sind bereits enthalten. Für den Betrieb braucht es deshalb weder Zugang zum privaten NuGet-Feed noch eine eigene Lizenz.

## 2. Datenbank vorbereiten

Anzulegen sind nur Datenbank und Benutzer — Schema, Tabellen, Funktionen und Startdaten bringt der Init-Container mit.

```sql
create database doea encoding 'UTF8';
create role doea_app login password '<passwort>';
alter database doea owner to doea_app;
```

Der Benutzer braucht auf der Datenbank das Recht, das Schema `dbo` anzulegen (`create schema if not exists dbo` ist der erste Befehl des Init-Containers) sowie anschliessend Lese- und Schreibrechte auf allen Objekten darin. Am einfachsten ist ein einziger Benutzer, dem die Datenbank gehört.

**Getrennte Benutzer für Migration und Applikation** sind möglich, aber Handarbeit: Die Migrationsskripte vergeben selbst keine Rechte. In dem Fall dem Applikationsbenutzer nach dem ersten Lauf des Init-Containers die Rechte auf `dbo` erteilen und über `alter default privileges` dafür sorgen, dass auch später angelegte Tabellen erfasst sind.

**Verbindungen:** Die Applikation hält ihre Verbindungen in einem Pool, den sie mit dem Logging teilt — höchstens 100 gleichzeitig pro Instanz (Standardwert des Treibers Npgsql). Eine Verbindung ist nur während einer Abfrage belegt, nicht für die Dauer einer Benutzersitzung; ungenutzte schliesst der Pool nach rund fünf Minuten. Ist er ausgeschöpft, wartet eine Anfrage bis zu 15 Sekunden auf eine freie Verbindung und bricht danach mit einem Fehler ab. `max_connections` deshalb auf mindestens 100 pro Applikationsinstanz plus Reserve für weitere Zugriffe auslegen; der Init-Container braucht nur während seines Laufs eine Verbindung. Eine tiefere Obergrenze lässt sich ohne Anpassung der Software im Connection-String setzen, z.B. `…;Maximum Pool Size=20`.

## 3. Images beziehen

Beide Images liegen in der Container Registry, jeweils unter der Release-Version und unter `latest`:

```bash
docker pull <registry>/doea:1.0.0
docker pull <registry>/doea-init:1.0.0
```

**Applikation und Init-Container müssen dieselbe Version tragen** — der Init-Container bringt die Datenbankskripte mit, die zur jeweiligen Applikationsversion gehören. Die Version erscheint in der Fusszeile der Anwendung und dient bei Supportfällen als Referenz.

## 4. Schema und Übersetzungen einspielen

Der Init-Container erhält die Verbindung über die Umgebungsvariable `CONNECTIONSTRING` und beendet sich nach getaner Arbeit:

```bash
docker run --rm \
    -e CONNECTIONSTRING="Host=<db>;Port=5432;Database=doea;Username=doea_app;Password=<passwort>" \
    <registry>/doea-init:1.0.0
```

Was dabei passiert:

1. Verbindungstest; schlägt er fehl, endet der Lauf mit Exit-Code `-1` und der Meldung `Connection to database failed`.
2. Schema `dbo` und die Journaltabelle `dbo.schemaversions` anlegen, falls sie fehlen.
3. Alle Skripte unter `Files/Skripts/RelNNNN/` in Ordner- und Dateireihenfolge ausführen. Bereits protokollierte Skripte werden übersprungen (`Already executed`), jedes Skript läuft in einer eigenen Transaktion zusammen mit seinem Journaleintrag.
4. Die MudBlazor-Übersetzungen (`MUD_`-Schlüssel) mit den `.resx`-Dateien abgleichen.

Der Lauf ist wiederholbar; ein Fehler bricht ihn mit Exit-Code `-1` ab. **Die Applikation erst starten, wenn der Init-Container erfolgreich durchgelaufen ist** — sie erwartet das Schema und lädt die Übersetzungen einmalig beim Start.

## 5. eIAM-Dossier registrieren

| Angabe | Wert |
|--------|------|
| Redirect-URI | `https://<host>/signin-oidc` |
| Post-Logout-Redirect-URI | `https://<host>/signout-callback-oidc` |
| Logoutseite (falls das Dossier eine eigene verlangt) | `https://<host>/anmeldung` |
| Front-Channel-Logout-URI (optional) | `https://<host>/signout-oidc` |
| Flow | Authorization Code + PKCE, Client Secret, `response_mode=form_post` |
| Scopes | `openid profile email` |
| Claims | `sub`, `given_name`, `family_name`, `email`, `language` |

Hintergrund, Sitzungsverhalten und die Abgrenzung zum eIAM-Web-PEP stehen im [README, Abschnitt Authentifizierung](README.md#authentifizierung-eiam). Die vom eIAM reservierten Pfade und Parameter (`/login`, `/logout`, `/auth`, `?login`, `?logout`) verwendet DoeA bewusst nicht.

## 6. Applikationscontainer starten

### Umgebungsvariablen

| Variable | Pflicht | Zweck |
|----------|---------|-------|
| `ConnectionStrings__APP` | ja | PostgreSQL-Verbindung; fehlt sie, bricht der Start mit `ConnectionStrings:APP ist nicht konfiguriert` ab |
| `OpenIdConnect__Authority` | ja | Issuer-URL des eIAM; ausserhalb von Development zwingend `https` |
| `OpenIdConnect__ClientId` | ja | Client-ID aus dem eIAM-Dossier |
| `OpenIdConnect__ClientSecret` | ja | Client Secret; gehört in die Secret-Verwaltung der Plattform, nie ins Image oder Repository |
| `Serilog__MinimumLevel` | nein | Standard `Warning`; für die Fehlersuche vorübergehend `Information` oder `Debug` |

Alles Übrige bringt das Image mit und ist nur in Ausnahmefällen zu überschreiben: `ASPNETCORE_URLS=http://+:8080`, `ASPNETCORE_ENVIRONMENT=Production`, `ASPNETCORE_FORWARDEDHEADERS_ENABLED=true`, `TZ=Europe/Zurich`, `LANG=de_CH.UTF-8` sowie der GemBox-Lizenzschlüssel.

### Start

```bash
docker run -d --name doea -p 8080:8080 \
    -e ConnectionStrings__APP="Host=<db>;Port=5432;Database=doea;Username=doea_app;Password=<passwort>" \
    -e OpenIdConnect__Authority="https://<eiam-authority>" \
    -e OpenIdConnect__ClientId="<client-id>" \
    -e OpenIdConnect__ClientSecret="<client-secret>" \
    <registry>/doea:1.0.0
```

Der Container lauscht auf Port 8080 (HTTP) und läuft unter dem Benutzer `doea` (UID 30120), nicht als root.

### Health Check

`GET /health` ist ohne Anmeldung erreichbar und antwortet mit `200` und dem Text `Healthy`, solange der Prozess Anfragen beantwortet (Liveness). Die Datenbank wird bewusst nicht geprüft: Ohne sie startet die Applikation gar nicht, und ein späterer Unterbruch soll keinen Neustart auslösen, der alle laufenden Blazor-Sitzungen beendet.

Das Image bringt einen `HEALTHCHECK` mit (`wget` gegen `http://localhost:8080/health`, alle 30 Sekunden, 60 Sekunden Startphase, nach drei Fehlversuchen `unhealthy`). Docker Compose startet damit den Reverse Proxy erst, wenn die Applikation antwortet:

```yaml
services:
  doea:
    image: <registry>/doea:1.0.0
    # Umgebungsvariablen wie oben
  proxy:
    image: nginx
    depends_on:
      doea:
        condition: service_healthy
```

Der Endpunkt gibt nichts weiter preis als den Text `Healthy` und darf deshalb auch durch den Proxy weitergeleitet werden — etwa damit dieser die Applikation selbst prüft, bevor er Anfragen an sie weitergibt.

## 7. Reverse Proxy

Anforderungen an den Proxy:

- **TLS terminieren** und ausschliesslich `https` nach aussen anbieten. Die Applikation setzt HSTS und leitet `http` auf `https` um.
- **`X-Forwarded-Proto` und `X-Forwarded-For` setzen.** Ohne `X-Forwarded-Proto: https` bildet der OIDC-Handler `http`-Redirect-URIs, die das eIAM ablehnt, und die HTTPS-Umleitung der Applikation läuft in eine Schleife.
- **Vom Client mitgeschickte `X-Forwarded-*`-Header verwerfen**, damit sie nicht gefälscht werden können.
- **WebSocket-Upgrade weiterleiten.** Blazor Server hält die Verbindung über `/_blazor`; ohne Upgrade fällt die Anwendung auf Long Polling zurück oder bricht ab.
- **Lesetimeout grosszügig** wählen (mindestens 100 Sekunden), sonst trennt der Proxy die stehende Verbindung.
- **Puffer für Antwort-Header nicht unter 32k.** Die Antwort des Login-Callbacks `/signin-oidc` setzt mehrere Cookies; mit den nginx-Standardwerten (`proxy_buffer_size` 4k) verwirft der Proxy sie mit «upstream sent too big header», und der Login endet mit 502.

Beispiel für nginx (Ausschnitt, an die Umgebung anzupassen):

```nginx
location / {
    proxy_pass         http://doea:8080;
    proxy_http_version 1.1;
    proxy_set_header   Upgrade $http_upgrade;
    proxy_set_header   Connection "upgrade";
    proxy_set_header   Host $host;
    proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
    proxy_set_header   X-Forwarded-Proto $scheme;
    proxy_read_timeout 100s;
    # Antwort-Header des Login-Callbacks (mehrere Set-Cookie-Zeilen) passen nicht in den Standardpuffer von 4k
    proxy_buffer_size       32k;
    proxy_buffers           8 32k;
    proxy_busy_buffers_size 64k;
}
```

## 8. Ersten Administrator freischalten

Das eIAM authentisiert alle Bundesbenutzer; **autorisiert** ist nur, wer in `dbo.t_user` aktiv ist. Unbekannte Benutzer werden beim ersten Login inaktiv angelegt und sehen den Hinweis, dass sie noch nicht freigeschaltet sind.

1. Die künftige administrierende Person meldet sich einmal an.
2. Freischalten:

   ```sql
   update dbo.t_user set usraktiv = true where usrextid = '<sub aus eIAM>';
   ```

3. Sie kommt über «Erneut prüfen» ohne neues Login hinein und schaltet alle weiteren Benutzer unter **Einstellungen → Benutzer** frei. Ein einmaliger SQL-Eingriff genügt also pro Installation.

## 9. Installation prüfen

| Prüfung | Erwartung |
|---------|-----------|
| `docker inspect --format '{{.State.Health.Status}}' doea` | `healthy` — der Health Check im Image erreicht die Applikation |
| `curl -s -o /dev/null -w "%{http_code}" https://<host>/health` | `200` — sofern der Proxy den Pfad weiterleitet; die Applikation antwortet ohne Anmeldung und leitet nicht zum eIAM um |
| Aufruf von `https://<host>/` ohne Anmeldung | Weiterleitung zum eIAM, danach zurück auf die Startseite |
| Fusszeile | zeigt die erwartete Versionsnummer |
| Einstellungen → Log | enthält Einträge, das Logging in `t_log` funktioniert |
| Sprachwahl in der Kopfzeile | Oberfläche wechselt auf Französisch bzw. Italienisch |
| Eine Auswertung als PDF exportieren | vollständiges Dokument; abgeschnittene Seiten weisen auf ein fehlerhaft gebautes Image hin (siehe [Fehlerbehebung](#14-fehlerbehebung)) |

## 10. Betriebsparameter

Fachliche Parameter liegen in `dbo.t_konfiguration` und werden in der Anwendung unter **Einstellungen → Konfiguration** gepflegt, nicht über Umgebungsvariablen:

| NaturalId | Vorbelegung | Bedeutung |
|-----------|-------------|-----------|
| `AnzahlTageLog` | 30 | Aufbewahrungsdauer der Logeinträge; bei jedem Start werden ältere Einträge gelöscht |
| `FristPublikation` | 3 | Tolerierte Tage zwischen Auftragseingang und Publikation für die Zielerreichung |
| `MinimaleZielerreichung` | 95 | Minimaler Prozentsatz fristgerechter Publikationen |

## 11. Aktualisierung

1. Neue Images beziehen (`doea` und `doea-init` derselben Version).
2. Init-Container ausführen (siehe [Schritt 4](#4-schema-und-übersetzungen-einspielen)). Bereits angewendete Skripte überspringt er.
3. Applikationscontainer auf das neue Image umstellen und neu starten.

Der Neustart ist auch dann nötig, wenn nur Übersetzungen dazugekommen sind: Sie werden einmalig beim Start geladen. Laufende Sitzungen verlieren ihre Blazor-Verbindung und bauen sie neu auf; das Anmelde-Cookie bleibt gültig, weil die Schlüssel in der Datenbank liegen.

Für einen **Rollback** genügt es nicht, das alte Image zu starten: DbUp kennt keine Rücknahmeskripte. Vor einem Update mit Schemaänderungen deshalb eine Sicherung anlegen.

## 12. Mehrere Instanzen

- **Sitzungsaffinität ist zwingend** («Sticky Sessions»). Blazor Server hält den Zustand einer Sitzung im Arbeitsspeicher der Instanz, und erzeugte Exportdateien warten dort bis zu 10 Minuten auf ihre Abholung.
- Die Data-Protection-Schlüssel liegen in `dbo.t_dataprotectionkey`, Cookies gelten deshalb instanzübergreifend und überleben Neustarts.
- Der Init-Container darf nur einmal pro Aktualisierung laufen, nicht je Instanz parallel.

## 13. Datensicherung

Zu sichern ist ausschliesslich die PostgreSQL-Datenbank (z.B. `pg_dump`); die Container selbst sind zustandslos und aus den Images reproduzierbar.

Die Sicherung enthält mit `dbo.t_dataprotectionkey` die Schlüssel, mit denen die Anmelde-Cookies verschlüsselt sind — sie liegen dort unverschlüsselt und sind entsprechend zu schützen. Sie enthält ausserdem die Logtabelle `t_log`.

## 14. Fehlerbehebung

| Symptom | Ursache | Abhilfe |
|---------|---------|---------|
| Start bricht ab mit `ConnectionStrings:APP ist nicht konfiguriert` | Verbindung fehlt oder ist falsch benannt | Variable `ConnectionStrings__APP` (zwei Unterstriche) setzen |
| Start bricht ab mit `OpenIdConnect:Authority muss ausserhalb von Development https verwenden` | Authority als `http` konfiguriert | Authority auf `https` umstellen |
| «Correlation failed» nach der Anmeldung | Die Applikation sieht den Request als `http` | `X-Forwarded-Proto` am Proxy setzen, `ASPNETCORE_FORWARDEDHEADERS_ENABLED=true` prüfen |
| Endlose Weiterleitungen zwischen Applikation und eIAM | dasselbe Ursachenbild wie oben | wie oben |
| 502 unmittelbar nach der Anmeldung, im Proxy-Log «upstream sent too big header»; nach erneutem Absenden zusätzlich «Code not valid» im Applikationslog | Antwort-Header des Login-Callbacks überschreiten den Header-Puffer des Proxys | `proxy_buffer_size`/`proxy_buffers` erhöhen, siehe [Reverse Proxy](#7-reverse-proxy) |
| Anmeldung endet auf `/anmeldung` mit «noch nicht freigeschaltet» | Benutzer ist in `t_user` inaktiv | unter Einstellungen → Benutzer freischalten, beim allerersten Benutzer per SQL |
| Oberfläche verliert nach kurzer Zeit die Verbindung | Proxy leitet kein WebSocket-Upgrade weiter oder trennt zu früh | Upgrade-Header und `proxy_read_timeout` prüfen |
| PDF-Exporte abgeschnitten | Im Image fehlt der GemBox-Lizenzschlüssel, die Dokumentgenerierung läuft im Free-Modus (Warnung im Log beim Start) | Das Image ist fehlerhaft gebaut; bei der technischen Ansprechstelle melden |
| Neue Übersetzungen erscheinen nicht | Übersetzungen werden nur beim Start geladen | Applikationscontainer neu starten |
| Container wird `unhealthy`, der Proxy startet wegen `depends_on` nicht | Die Applikation antwortet nicht auf `/health`: Start fehlgeschlagen (siehe oben) oder der Prozess hängt | `docker logs doea` prüfen; der Health Check meldet nur, ob der Prozess antwortet, die Ursache steht im Log |

Weitere Fehlerbilder, Kontaktstellen und der Umgang mit Sicherheitshinweisen stehen in [README](README.md) und [CONTRIBUTING.md](CONTRIBUTING.md).
