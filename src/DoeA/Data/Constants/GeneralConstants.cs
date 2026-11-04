// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

namespace DoeA.Data.Constants;

/// <summary>Statuscodes der ServiceResults und Übersetzungsschlüssel der Fehler- und Validierungsmeldungen.</summary>
public class GeneralConstants
{
    /// <summary>Meldung für eine nicht abgefangene Exception.</summary>
    public const string UnhandledException = "ERROR_UNHANDLED_EXCEPTION";
    /// <summary>Meldung für einen bereits geloggten Fehler, dessen Details nicht in die Oberfläche gehören.</summary>
    public const string HandledException = "ERROR_HANDLED_EXCEPTION";
    /// <summary>Meldung, wenn der Datensatz zwischenzeitlich von jemand anderem geändert wurde.</summary>
    public const string ConcurrencyError = "ERROR_CONCURRENCY_EXCEPTION";
    /// <summary>Meldung für ein fehlendes Pflichtobjekt; der Parameter nach «?» benennt es.</summary>
    public const string NullReference = "ERROR_NULL_REFERENCE";
    /// <summary>Meldung für eine fehlgeschlagene FluentValidation; der Parameter nach «?» enthält die Fehlerliste.</summary>
    public const string ValidationException = "ERROR_VALIDATION";
    /// <summary>Meldung und Statuscode, wenn der Datensatz nicht (mehr) existiert.</summary>
    public const string NotFoundError = "ERROR_NOT_FOUND";
    /// <summary>Statuscode einer erfolgreichen Service-Operation.</summary>
    public const string Ok = "OK";
    /// <summary>Statuscode, wenn das Speichern keine Änderung gegenüber dem DB-Stand ergab.</summary>
    public const string NothingChanged = "NOTHING_CHANGED";
    /// <summary>Meldung, wenn dem Service kein Objekt übergeben wurde.</summary>
    public const string ItemWasNullError = "ERROR_ITEM_WAS_NULL";
    /// <summary>Meldung bei Verletzung eines Unique-Keys.</summary>
    public const string DuplicateKeyError = "ERROR_DUPLICATE_KEY";
    /// <summary>Meldung für eine fachlich nicht erlaubte Aktion (z.B. sich selbst deaktivieren).</summary>
    public const string ForbiddenError = "ERROR_FORBIDDEN";
    /// <summary>Word-Export: Zeilenzahl der Vorlagentabelle passt nicht zu den Daten.</summary>
    public const string DocTemplateTableRows = "ERROR_DOC_FILL_TABLE_ROWS";
    /// <summary>Word-Export: Spaltenzahl der Vorlagentabelle passt nicht zu den Daten.</summary>
    public const string DocTemplateTableColumns = "ERROR_DOC_FILL_TABLE_COLUMNS";
    /// <summary>Word-Export: Tabelle mit dem erwarteten Titel fehlt in der Vorlage.</summary>
    public const string DocTemplateTable = "ERROR_DOC_FILL_TABLE";

    /// <summary>Validierungsmeldung «grösser oder gleich».</summary>
    public const string GreaterOrEqualValidator = "VALIDATOR_GREATER_THAN_OR_EQUAL";
    /// <summary>Validierungsmeldung «grösser als».</summary>
    public const string GreaterValidator = "VALIDATOR_GREATER_THAN";
    /// <summary>Validierungsmeldung «kleiner oder gleich».</summary>
    public const string LessOrEqualValidator = "VALIDATOR_LESS_THAN_OR_EQUAL";
    /// <summary>Validierungsmeldung «kleiner als».</summary>
    public const string LessValidator = "VALIDATOR_LESS_THAN";
    /// <summary>Validierungsmeldung «Maximallänge überschritten».</summary>
    public const string MaxLengthValidator = "VALIDATOR_MAXLENGTH";
    /// <summary>Validierungsmeldung «Minimallänge unterschritten».</summary>
    public const string MinLengthValidator = "VALIDATOR_MINLENGTH";
    /// <summary>Validierungsmeldung «Pflichtfeld leer».</summary>
    public const string NotEmptyValidator = "VALIDATOR_NOTEMPTY";
    /// <summary>Validierungsmeldung «ungültiges Datum».</summary>
    public const string DateNotValid = "VALIDATOR_DATE_INVALID";
    /// <summary>Validierungsmeldung für Audit-Datumswerte vor dem zulässigen Minimum.</summary>
    public const string DateConflictToOld = "VALIDATOR_DATE_CONFLICT_TO_OLD";
    /// <summary>Validierungsmeldung für Audit-Datumswerte in der Zukunft.</summary>
    public const string DateConflictToYoung = "VALIDATOR_DATE_CONFLICT_TO_YOUNG";

    /// <summary>Serilog-Nachrichtenvorlage der Fehlerbehandlung: Ressource, Methode und Meldung.</summary>
    public const string MessageTemplate = "{Ressource}.{Method}: {Message}";

    /// <summary>NaturalId der Konfiguration «maximale Anzahl Arbeitstage von Eingereicht bis Publikation».</summary>
    public const string KonfigurationFristPublikation = "FristPublikation";
}
