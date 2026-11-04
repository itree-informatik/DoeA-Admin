# SPDX-License-Identifier: AGPL-3.0-or-later
# Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

## BUILD
FROM mcr.microsoft.com/dotnet/sdk:10.0-noble AS build
WORKDIR /src

ARG proj="./DoeA/DoeA.csproj"
ARG ASSEMBLY_VERSION
ARG ASSEMBLY_INFO_VERSION

# Nur Solution und Projektdateien kopieren, damit der Restore als eigener Layer gecacht wird
COPY DoeA.slnx ./
COPY DoeA/DoeA.csproj DoeA/
COPY DbUpdater/DbUpdater.csproj DbUpdater/

# Der NuGet-PAT wird via BuildKit-Secret eingespeist und liegt weder im Image-Layer
# noch im Build-Cache. Die temporäre NuGet.Config wird im selben RUN wieder gelöscht.
RUN --mount=type=secret,id=nuget_gh_pat \
    set -e; \
    GH_PAT="$(cat /run/secrets/nuget_gh_pat)"; \
    NUGET_DIR="$(mktemp -d)"; \
    dotnet new nugetconfig --output "$NUGET_DIR"; \
    NUGET_CFG="$NUGET_DIR/nuget.config"; \
    dotnet nuget add source "https://nuget.pkg.github.com/itree-informatik/index.json" \
      -n itree-nuget -u x-access-token -p "$GH_PAT" --store-password-in-clear-text \
      --configfile "$NUGET_CFG"; \
    # Ohne .razor-Dateien im Build-Kontext setzt das SDK RequiresAspNetWebAssets nicht und lässt beim Restore das Paket
    # Microsoft.AspNetCore.App.Internal.Assets weg; der spätere publish --no-restore liefert dann kein wwwroot/_framework (blazor.web.js → 404)
    dotnet restore "$proj" --configfile "$NUGET_CFG" -p:RequiresAspNetWebAssets=true; \
    rm -rf "$NUGET_DIR"

COPY . .

RUN dotnet publish $proj -c Release -o /app/publish --no-restore -p:Version=$ASSEMBLY_VERSION -p:InformationalVersion=$ASSEMBLY_INFO_VERSION

## RUN
FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble AS base
WORKDIR /app

# libgdiplus und Microsoft-Schriften für die PDF-Exporte aus den Word-Vorlagen (GemBox), wget für den HEALTHCHECK
RUN apt-get update && \
    apt-get -y install --no-install-recommends libgdiplus fontconfig wget && \
    echo "ttf-mscorefonts-installer msttcorefonts/accepted-mscorefonts-eula select true" | debconf-set-selections && \
    apt-get -y install --no-install-recommends ttf-mscorefonts-installer && \
    apt-get upgrade -y && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

ARG GEMBOX_LICENSEKEY

# Port, Sprache und Lizenzschlüssel (Build-Arg aus dem Workflow, gelesen über LicenseKeys:GemBox)
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV http_port=8080
ENV LANG=de_CH.UTF-8
ENV LANGUAGE=de_CH:de
ENV LC_ALL=de_CH.UTF-8
ENV LicenseKeys__GemBox=$GEMBOX_LICENSEKEY
# TLS endet am Proxy davor: ohne X-Forwarded-Proto baut der OIDC-Handler http-Redirect-URIs, die eIAM ablehnt
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true

# Zeitzone
ENV TZ=Europe/Zurich
ENV WEBSITE_TIME_ZONE=Europe/Zurich
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

EXPOSE 8080

# Liveness-Check für Docker und Compose (depends_on: service_healthy): nur «Prozess antwortet», keine Datenbankprüfung.
# Die Startphase deckt das Laden der Übersetzungen ab; wget-Fehlercodes werden auf 1 normiert (2 ist bei Docker reserviert)
HEALTHCHECK --interval=30s --timeout=5s --start-period=60s --retries=3 \
    CMD wget -q -T 3 -t 1 -O /dev/null http://localhost:8080/health || exit 1

# Die App läuft nicht als root
ARG UID=30120
ARG GID=30120
ARG APP_HOME_DIR=/home/doea

RUN groupadd -g ${GID} doea && \
    useradd -r -u ${UID} -g ${GID} -d ${APP_HOME_DIR} doea && \
    mkdir -p ${APP_HOME_DIR} && \
    chown -R ${UID}:${GID} ${APP_HOME_DIR}

# Publish-Output zuletzt kopieren, damit die System-Layer gecacht bleiben
COPY --from=build /app/publish .

USER doea

ENTRYPOINT [ "dotnet", "DoeA.dll"]
