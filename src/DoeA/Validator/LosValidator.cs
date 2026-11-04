// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Constants;
using DoeA.Data.Models.DTO;
using ItreeMud.Models;
using FluentValidation;

namespace DoeA.Validator;

/// <summary>Validierungsregeln für Los.</summary>
public class LosValidator : AbstractValidator<Los>
{
    public LosValidator(bool withoutBaseValidator = false)
    {
        if (!withoutBaseValidator)
        {
            Include(new BasisValidator());
        }

        RuleFor(x => x.AusId).NotEmpty().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator));
        RuleFor(x => x.Bezeichnung).NotEmpty().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator));
        // Das Abbruchdatum ist das Datum der Publikation des Abbruchs.
        RuleFor(x => x.Eingereicht).PflichtBeiPublikation(x => x.Abbruchdatum);
        RuleFor(x => x.Abbruchdatum).NichtVorZustellung(x => x.Eingereicht, "VALIDATOR_ABBRUCH_VOR_ZUGESTELLT");
        // Fachliche Vorgabe: das Abbruchdatum darf auch in der Zukunft liegen. Es bleibt nur eine
        // Plausibilitätsgrenze gegen Tippfehler (z.B. Jahr 0216 statt 2016), analog BasisValidator.DateTooOld.
        When(x => x.Abbruchdatum.HasValue, () => RuleFor(x => x.Abbruchdatum)
            .Must(d => d!.Value > DateOnly.FromDateTime(BasisValidator.DateTooOld))
            .WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.GreaterValidator), BasisValidator.DateTooOld.ToString("dd.MM.yyyy"))));
    }
}
