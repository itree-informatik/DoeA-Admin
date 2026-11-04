// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Constants;
using DoeA.Data.Models.DTO.Stammdaten;
using FluentValidation;

namespace DoeA.Validator.Stammdaten;

/// <summary>Validierungsregeln für CpvNummer.</summary>
public class CpvNummerValidator : AbstractValidator<CpvNummer>
{
    public CpvNummerValidator(bool withoutBaseValidator = false)
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
        RuleFor(b => b.Nummer)
            .NotEmpty().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator))
            .GreaterThanOrEqualTo(3000000).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.GreaterOrEqualValidator), 3000000))
            .LessThan(100000000).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.LessValidator), 100000000));
    }
}