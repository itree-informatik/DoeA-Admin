# SPDX-License-Identifier: AGPL-3.0-or-later
# Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

## Run
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS run

# ICU-Bibliotheken, damit .NET auf Alpine nicht im invarianten Globalisierungsmodus läuft
RUN apk --no-cache add icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Build-Kontext ist der Publish-Output (src/DbUpdater/pub), erzeugt im Workflow
COPY . ./app
WORKDIR /app/DbUpdater

# Exec-Form ohne Shell: Program.cs liest CONNECTIONSTRING selbst. Als Argument stünde das Passwort in der Prozessliste,
# die jeder Benutzer des Hosts lesen kann
CMD ["dotnet", "DbUpdater.dll"]