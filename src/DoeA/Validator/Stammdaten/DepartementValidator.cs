// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Constants;
using DoeA.Data.Models.DTO.Stammdaten;
using FluentValidation;

namespace DoeA.Validator.Stammdaten;

/// <summary>Validierungsregeln für Departement.</summary>
public class DepartementValidator : AbstractValidator<Departement>
{
    public DepartementValidator(bool withoutBaseValidator = false)
    {

        if (!withoutBaseValidator)
        {
            Include(new BasisValidator());
        }
        RuleFor(d => d.Bezeichnung)
            .NotEmpty().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator))
            .MinimumLength(2).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.MinLengthValidator), 2))
            .MaximumLength(255).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.MaxLengthValidator), 255));
        RuleFor(d => d.Kurzbezeichnung)
            .NotEmpty().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator))
            .MinimumLength(1).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.MinLengthValidator), 1))
            .MaximumLength(255).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.MaxLengthValidator), 255));
    }
}