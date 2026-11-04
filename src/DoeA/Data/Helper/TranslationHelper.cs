// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.Runtime.CompilerServices;
using DoeA.Data.Models.DB;
using Microsoft.EntityFrameworkCore;

namespace DoeA.Data.Helper;

/// <summary>Statischer Übersetzungsspeicher: beim Start aus t_translation geladen, Sprache pro Circuit über dessen SynchronizationContext.</summary>
public static class TranslationHelper
{
    private static List<TranslationItem> _translationList = new();
    private static Dictionary<string, TranslationItem> _translations = new();

    // Pro Blazor-Server-Circuit existiert ein eigener SynchronizationContext, der über die gesamte
    // Circuit-Lebensdauer identisch bleibt. ConditionalWeakTable verhindert Speicherlecks: wird der Circuit
    // beendet und der SynchronizationContext vom GC eingesammelt, verschwindet der Eintrag von selbst.
    private static readonly ConditionalWeakTable<SynchronizationContext, LanguageHolder> _languageMap = new();

    /// <summary>Sprache des aktuellen Circuits; ohne SynchronizationContext (z.B. in Minimal-APIs) Deutsch.</summary>
    public static string CurrentLanguage
    {
        get
        {
            var ctx = SynchronizationContext.Current;
            if (ctx != null && _languageMap.TryGetValue(ctx, out var holder))
                return holder.Language;
            return "de";
        }
        set
        {
            var ctx = SynchronizationContext.Current;
            if (ctx != null)
            {
                var holder = _languageMap.GetOrCreateValue(ctx);
                holder.Language = value;
            }
        }
    }

    /// <summary>Alle Übersetzungen als Liste für die TranslationSeite.</summary>
    public static IReadOnlyList<TranslationItem> Translations => _translationList;

    /// <summary>Lädt alle Übersetzungen aus der DB; läuft nur beim Start, neue Schlüssel erscheinen erst nach einem Neustart.</summary>
    public static async Task LoadTranslations(IDbContextFactory<DoeAContext> dbFactory)
    {
        await using var dbContext = await dbFactory.CreateDbContextAsync();

        var list = await dbContext.TTranslation
            .Include(t => t.TrlRel)
            .OrderBy(t => t.TrlKey)
            .Select(t => new TranslationItem
            {
                Id = t.TrlId,
                Key = t.TrlKey,
                Deutsch = t.TrlDeutsch,
                Franzoesisch = t.TrlFranzoesisch,
                Italienisch = t.TrlItalienisch,
                RelVersion = t.TrlRel.RelVersion
            })
            .ToListAsync();

        _translationList = list;
        _translations = list.ToDictionary(t => t.Key);
    }

    /// <summary>Übersetzt den Schlüssel in die aktuelle Sprache; ohne Übersetzung Deutsch, ohne Eintrag der Schlüssel selbst.</summary>
    public static string Translate(string key)
    {
        if (!_translations.TryGetValue(key, out var entry))
        {
            return key;
        }

        var value = CurrentLanguage switch
        {
            "fr" => entry.Franzoesisch,
            "it" => entry.Italienisch,
            _ => entry.Deutsch
        };

        if (string.IsNullOrEmpty(value))
        {
            value = entry.Deutsch;
        }

        return string.IsNullOrEmpty(value) ? key : value;
    }

    /// <summary>Aktualisiert einen Eintrag im Speicher nach dem Speichern in der DB.</summary>
    public static void UpdateEntry(Guid id, string deutsch, string? franzoesisch, string? italienisch)
    {
        var item = _translationList.FirstOrDefault(t => t.Id == id);
        if (item == null) return;

        item.Deutsch = deutsch;
        item.Franzoesisch = franzoesisch;
        item.Italienisch = italienisch;
    }

    /// <summary>Übersetzungseintrag im Speicher; die TranslationSeite bearbeitet ihn und führt ihn über UpdateEntry nach.</summary>
    public class TranslationItem
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Deutsch { get; set; } = string.Empty;
        public string? Franzoesisch { get; set; }
        public string? Italienisch { get; set; }
        public string RelVersion { get; set; } = string.Empty;
    }

    private class LanguageHolder
    {
        public string Language { get; set; } = "de";
    }
}
