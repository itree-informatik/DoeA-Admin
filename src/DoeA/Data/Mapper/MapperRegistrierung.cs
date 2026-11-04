// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

namespace DoeA.Data.Mapper;

/// <summary>Registriert alle Mapperly-Mapper; sie sind zustandslos und deshalb Singletons.</summary>
public static class MapperRegistrierung
{
    /// <summary>
    /// Registriert jede Klasse aus <c>Data/Mapper</c>, deren Name auf "Mapper" endet. Der Scan ersetzt
    /// eine Liste von Hand: Ein vergessener Eintrag fiele sonst erst beim Aufruf der betroffenen Seite auf,
    /// im Container-Betrieb ohne Startfehler. Nach dem Attribut <c>[Mapper]</c> lässt sich nicht suchen —
    /// Riok.Mapperly.Abstractions ist ein reines Build-Paket und liegt zur Laufzeit nicht vor.
    /// </summary>
    public static IServiceCollection RegistriereMapper(this IServiceCollection services)
    {
        var mapperNamespace = typeof(MapperRegistrierung).Namespace!;
        var mapperTypen = typeof(MapperRegistrierung).Assembly.GetTypes()
            .Where(typ => typ is { IsClass: true, IsAbstract: false, IsGenericTypeDefinition: false }
                          && typ.Name.EndsWith("Mapper", StringComparison.Ordinal)
                          && typ.Namespace?.StartsWith(mapperNamespace, StringComparison.Ordinal) == true);

        foreach (var typ in mapperTypen)
        {
            services.AddSingleton(typ);
        }

        return services;
    }
}
