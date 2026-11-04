// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Constants;
using DoeA.Data.Mapper.Einstellungen;
using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO.Einstellungen;
using DoeA.Interfaces;
using DoeA.Interfaces.Einstellungen;
using DoeA.Validator.Einstellungen;
using ItreeMud.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using ILogger = Serilog.ILogger;

namespace DoeA.Services.Einstellungen;

/// <summary>Liest und schreibt die Applikationskonfigurationen und füllt dabei die typisierten Wert-Properties.</summary>
public class KonfigurationService : BaseService<Konfiguration, TKonfiguration, KonfigurationValidator>, IKonfigurationService
{
    // Werte der Spalte KonTyp; sie steuern, welche Wert-Property gefüllt und welcher Editor angezeigt wird.
    private const string TypBool = "bool";
    private const string TypDate = "date";
    private const string TypDecimal = "decimal";
    private const string TypInt = "int";

    // Wahrheitswerte stehen in der DB als Text.
    private const string WertJa = "Ja";

    public KonfigurationService(IErrorHandlerService errorHandlerService, IDbContextFactory<DoeAContext> dbFactory, KonfigurationMapper mapper, ILogger logger, IAktuellerBenutzer aktuellerBenutzer) : base(errorHandlerService, dbFactory, mapper, logger, aktuellerBenutzer, new KonfigurationValidator(), "Kon")
    {
    }

    /// <summary>Aktiv ist auf der KonfigurationSeite nicht änderbar und wird beim Update nie zurückgeschrieben.</summary>
    protected override IEnumerable<string> GeschuetzteProperties => [nameof(TKonfiguration.KonAktiv)];

    /// <summary>Filtert die Konfigurationen über NaturalId, Bezeichnung und Beschreibung.</summary>
    public override async Task<ObservableCollection<Konfiguration>> Search(string searchValue, bool onlyActive)
    {
        var alle = await GetAll() ?? [];
        var treffer = alle
            .Where(item => (!onlyActive || item.Aktiv) &&
                           (string.IsNullOrWhiteSpace(searchValue) ||
                            item.NaturalId?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) == true ||
                            item.Bezeichnung.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                            item.Beschreibung.Contains(searchValue, StringComparison.OrdinalIgnoreCase)));
        return new ObservableCollection<Konfiguration>(treffer);
    }

    /// <summary>
    /// Liefert alle Konfigurationen inklusive der aus <c>Wert</c> abgeleiteten typisierten Werte. Die Signatur muss exakt der
    /// Basisklasse entsprechen: Bei abweichendem Rückgabetyp wäre nur <c>new</c> möglich, und Aufrufe der EditComponent über
    /// <c>IBaseService</c> landeten in der Basisklasse — die Wert*-Properties blieben dann leer.
    /// </summary>
    public override async Task<List<Konfiguration>?> GetAll()
    {
        var result = await base.GetAll();
        return result?.Select(MitTypisiertenWerten).ToList();
    }

    /// <summary>Liefert eine Kopie des Eintrags, bei der die zum Typ passende Wert-Property aus <c>Wert</c> befüllt ist.</summary>
    private Konfiguration MitTypisiertenWerten(Konfiguration item)
    {
        var result = item.Clone();

        if (result == null)
        {
            var message = $"Konfiguration '{item.NaturalId}' konnte nicht kopiert werden.";
            Logger.Error(GeneralConstants.MessageTemplate, nameof(KonfigurationService), nameof(MitTypisiertenWerten), message);
            throw new InvalidOperationException(message);
        }

        switch (item.Typ)
        {
            case TypBool:
                result.WertBool = result.Wert == WertJa;
                break;
            case TypDate:
                result.WertDate = result.Wert;
                break;
            case TypInt:
                if (!int.TryParse(result.Wert, out var wertInt))
                {
                    LogNichtLesbarerWert(result, TypInt);
                }
                result.WertInt = wertInt;
                break;
            case TypDecimal:
                if (!decimal.TryParse(result.Wert, out var wertDecimal))
                {
                    LogNichtLesbarerWert(result, TypDecimal);
                }
                result.WertDecimal = wertDecimal;
                break;
        }

        return result;
    }

    // Ein nicht lesbarer Wert bricht die Konfiguration nicht ab, der Eintrag fällt aber auf 0 zurück.
    private void LogNichtLesbarerWert(Konfiguration item, string typ) =>
        Logger.Warning(GeneralConstants.MessageTemplate, nameof(KonfigurationService), nameof(MitTypisiertenWerten),
            $"Wert '{item.Wert}' von '{item.NaturalId}' ist kein gültiger Wert vom Typ '{typ}' — es wird 0 verwendet.");
}
