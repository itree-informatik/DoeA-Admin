// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Constants;
using DoeA.Data.Models.DTO.Stammdaten;
using FluentValidation;

namespace DoeA.Validator.Stammdaten;

/// <summary>Validierungsregeln für BkpNummer.</summary>
public class BkpNummerValidator : AbstractValidator<BkpNummer>
{
    public BkpNummerValidator(bool withoutBaseValidator = false)
    {

        if (!withoutBaseValidator)
        {
            Include(new BasisValidator());
        }
        RuleFor(b => b.Bezeichnung)
            .NotEmpty().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator))
            .MinimumLength(2).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.MinLengthValidator), 2))
            .MaximumLength(255).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.MaxLengthValidator), 255));
        RuleFor(b => b.NummerStr)
            .NotEmpty().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator))
            .MinimumLength(2).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.MinLengthValidator), 2))
            .MaximumLength(255).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.MaxLengthValidator), 255));
        // Kein NotEmpty: das würde bei int die gültige BKP-Nummer 0 (Grundstück) verbieten.
        RuleFor(b => b.Nummer)
            .GreaterThanOrEqualTo(0).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.GreaterOrEqualValidator), 0))
            .LessThan(1000).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.LessValidator), 1000));
    }
}