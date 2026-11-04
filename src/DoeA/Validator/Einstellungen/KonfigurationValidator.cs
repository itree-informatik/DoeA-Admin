// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Constants;
using DoeA.Data.Models.DTO.Einstellungen;
using FluentValidation;

namespace DoeA.Validator.Einstellungen;

/// <summary>Validierungsregeln für Konfiguration.</summary>
public class KonfigurationValidator : AbstractValidator<Konfiguration>
{
    public KonfigurationValidator(bool withoutBaseValidator = false)
    {
        if (!withoutBaseValidator)
        {
            Include(new BasisValidator());
        }
        RuleFor(a => a.Bezeichnung)
            .NotEmpty().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator))
            .MinimumLength(2).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.MinLengthValidator), 2))
            .MaximumLength(255).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.MaxLengthValidator), 255));
        RuleFor(a => a.Beschreibung)
            .NotEmpty().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator))
            .MinimumLength(2).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.MinLengthValidator), 2))
            .MaximumLength(1000).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.MaxLengthValidator), 1000));
        RuleFor(x => x.WertInt)
            .NotEmpty().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator))
            .When(x => x.WertIsInt);
        RuleFor(x => x.WertDate)
            .NotEmpty().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator))
            .Must(BeAValidDate).WithMessage(_ => TranslationHelper.Translate(GeneralConstants.DateNotValid))
            .When(x => x.WertIsDate);
        RuleFor(x => x.WertDecimal)
            .NotEmpty().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator))
            .When(x => x.WertIsDecimal);
    }

    private bool BeAValidDate(string? value)
    {
        if (value == null)
        {
            return false;
        }

        value = value.Trim();
        if (value.Length == 5)
        {
            return DateTime.TryParse(value, out _);
        }

        return false;
    }
}