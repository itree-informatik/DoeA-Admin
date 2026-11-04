// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Constants;
using DoeA.Data.Models.DTO.Exportgruppen;
using FluentValidation;

namespace DoeA.Validator;

/// <summary>Validierungsregeln für Exportgruppe.</summary>
public class ExportgruppeValidator : AbstractValidator<Exportgruppe>
{
    public ExportgruppeValidator(bool withoutBaseValidator = false)
    {
        if (!withoutBaseValidator)
        {
            Include(new BasisValidator());
        }

        RuleFor(a => a.Name)
            .NotEmpty().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator))
            .MinimumLength(2).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.MinLengthValidator), 2))
            .MaximumLength(1000).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.MaxLengthValidator), 1000));

        // von/bis sind einzeln nutzbar; nur wenn beide gesetzt sind, muss die Reihenfolge stimmen.
        RuleFor(a => a.PublikationBis)
            .GreaterThanOrEqualTo(a => a.PublikationVon)
            .When(a => a.PublikationVon != null && a.PublikationBis != null)
            .WithMessage(_ => TranslationHelper.Translate("AUSWERTUNG_DATUM_VON_BIS_FEHLER"));
    }
}