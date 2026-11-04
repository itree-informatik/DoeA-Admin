// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Constants;
using FluentValidation;

namespace DoeA.Validator;

/// <summary>Gemeinsame Regeln aller Datumspaare «an DöA zugestellt» (Eingereicht) und «Publikation».</summary>
public static class PublikationRegel
{
    private const string MeldungKey = "VALIDATOR_PUBLIKATION_VOR_ZUGESTELLT";

    /// <summary>Die Publikation darf nicht vor der Zustellung an den DöA liegen (gleicher Tag zulässig); geprüft nur, wenn beide Daten erfasst sind.</summary>
    public static IRuleBuilderOptions<T, DateOnly?> NichtVorZustellung<T>(this IRuleBuilder<T, DateOnly?> publikation, Func<T, DateOnly?> eingereicht,
        string meldungKey = MeldungKey) =>
        publikation
            .Must((model, datum) => datum == null || eingereicht(model) is not { } zugestellt || datum >= zugestellt)
            .WithMessage(_ => TranslationHelper.Translate(meldungKey));

    /// <summary>«Eingereicht» ist Pflicht, sobald die zugehörige Publikation erfasst ist (gilt auch für Altdaten).</summary>
    public static IRuleBuilderOptions<T, DateOnly?> PflichtBeiPublikation<T>(this IRuleBuilder<T, DateOnly?> eingereicht, Func<T, DateOnly?> publikation) =>
        eingereicht
            .Must((model, datum) => datum != null || publikation(model) == null)
            .WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator));
}
