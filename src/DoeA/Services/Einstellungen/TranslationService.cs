// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DB;
using DoeA.Interfaces.Einstellungen;
using Microsoft.EntityFrameworkCore;

namespace DoeA.Services.Einstellungen;

/// <summary>Liest und schreibt die Übersetzungen und hält den Cache im TranslationHelper aktuell.</summary>
public class TranslationService : ITranslationService
{
    private readonly IDbContextFactory<DoeAContext> _dbFactory;

    public TranslationService(IDbContextFactory<DoeAContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    /// <summary>Übersetzt einen Schlüssel aus dem beim Start geladenen Cache.</summary>
    public string Translate(string key) => TranslationHelper.Translate(key);

    /// <summary>Speichert die Übersetzungen eines Eintrags; liefert false, wenn zur Id kein Eintrag existiert.</summary>
    public async Task<bool> UpdateTranslation(Guid id, string deutsch, string? franzoesisch, string? italienisch)
    {
        await using var dbContext = await _dbFactory.CreateDbContextAsync();
        var entity = await dbContext.TTranslation.FindAsync(id);
        if (entity == null)
        {
            return false;
        }

        entity.TrlDeutsch = deutsch;
        entity.TrlFranzoesisch = franzoesisch;
        entity.TrlItalienisch = italienisch;
        await dbContext.SaveChangesAsync();

        // Der Cache wird nur beim Applikationsstart gefüllt — ohne dieses Nachziehen wäre die Änderung erst nach einem Neustart sichtbar.
        TranslationHelper.UpdateEntry(id, deutsch, franzoesisch, italienisch);
        return true;
    }
}
