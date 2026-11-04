// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.Reflection;

namespace DoeA.Data.Helper;

/// <summary>InformationalVersion der Assembly für die Fusszeile der Layouts.</summary>
public static class AppVersion
{
    /// <summary>InformationalVersion der DoeA-Assembly (aus der CI-Versionierung), sonst "0.0.0".</summary>
    public static readonly string Aktuell = Assembly.GetExecutingAssembly()
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "0.0.0";
}
