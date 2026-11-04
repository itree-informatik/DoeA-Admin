// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Constants;
using DoeA.Data.Models.DTO;
using ItreeMud.Models;
using FluentValidation;

namespace DoeA.Validator;

/// <summary>Validierungsregeln für Offerte.</summary>
public class OfferteValidator : AbstractValidator<Offerte>
{
    /// <param name="withoutBaseValidator">Ohne die Regeln der Basisfelder (Client-Validierung).</param>
    /// <param name="publikationOfzIds">Offertezustände mit Publikation, siehe <see cref="OfferteOffertezustandValidator"/>.</param>
    public OfferteValidator(bool withoutBaseValidator = false, IReadOnlySet<Guid>? publikationOfzIds = null)
    {
        if (!withoutBaseValidator)
        {
            Include(new BasisValidator());
        }

        RuleFor(x => x.LosId).NotEmpty().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator));
        RuleFor(x => x.AnbId).NotEmpty().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator));
        RuleFor(x => x.ZuschlagswertChf).GreaterThanOrEqualTo(0).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.GreaterOrEqualValidator), 0));

        // Offertenzustände: Zustand und Datum sind obligatorisch (Inline-Bearbeitung analog Berichtigungen).
        RuleForEach(x => x.Offertenzustaende).SetValidator(new OfferteOffertezustandValidator(true, publikationOfzIds));
    }
}
