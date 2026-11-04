// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Constants;
using DoeA.Data.Models.DTO.Stammdaten;
using FluentValidation;

namespace DoeA.Validator.Stammdaten;

/// <summary>Validierungsregeln für Verfahrensschritt.</summary>
public class VerfahrensschrittValidator : AbstractValidator<Verfahrensschritt>
{
    public VerfahrensschrittValidator(bool withoutBaseValidator = false)
    {

        if (!withoutBaseValidator)
        {
            Include(new BasisValidator());
        }
        RuleFor(v => v.Bezeichnung)
            .NotEmpty().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator))
            .MinimumLength(2).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.MinLengthValidator), 2))
            .MaximumLength(255).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.MaxLengthValidator), 255));
        // Kein NotEmpty: das würde bei int die gültige 0 verbieten; den Bereich decken die Range-Regeln ab.
        RuleFor(v => v.Sortierung)
            .GreaterThanOrEqualTo(0).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.GreaterOrEqualValidator), 0))
            .LessThanOrEqualTo(100).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.LessOrEqualValidator), 100));
    }
}