// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Constants;
using DoeA.Data.Models.DTO.Stammdaten;
using FluentValidation;

namespace DoeA.Validator.Stammdaten;

/// <summary>Validierungsregeln für Organisationszugehörigkeit.</summary>
public class OrganisationszugehoerigkeitValidator : AbstractValidator<Organisationszugehoerigkeit>
{
    public OrganisationszugehoerigkeitValidator(bool withoutBaseValidator = false)
    {

        if (!withoutBaseValidator)
        {
            Include(new BasisValidator());
        }
        RuleFor(o => o.Bezeichnung)
            .NotEmpty().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator))
            .MinimumLength(2).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.MinLengthValidator), 2))
            .MaximumLength(255).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.MaxLengthValidator), 255));
        RuleFor(o => o.Kurzbezeichnung)
            .NotEmpty().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator))
            .MinimumLength(1).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.MinLengthValidator), 1))
            .MaximumLength(255).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.MaxLengthValidator), 255));
    }
}