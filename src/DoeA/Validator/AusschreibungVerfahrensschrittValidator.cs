// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Constants;
using DoeA.Data.Models.DTO;
using ItreeMud.Models;
using FluentValidation;

namespace DoeA.Validator;

/// <summary>Validierungsregeln für AusschreibungVerfahrensschritt; mit der Terminplanung zusätzlich gegen doppelte Verfahrensschritte.</summary>
public class AusschreibungVerfahrensschrittValidator : AbstractValidator<AusschreibungVerfahrensschritt>
{
    public AusschreibungVerfahrensschrittValidator(bool withoutBaseValidator = false, IEnumerable<AusschreibungVerfahrensschritt>? terminplanung = null)
    {
        if (!withoutBaseValidator)
        {
            Include(new BasisValidator());
        }

        RuleFor(x => x.AusId).NotEmpty().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator));
        RuleFor(x => x.VesId).NotEmpty().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator));
        RuleFor(x => x.DatumVon).NotEmpty().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator));

        if (terminplanung != null)
        {
            // Beide Einträge eines Duplikats sind ungültig, damit der Hinweis auch am gerade umgestellten Eintrag erscheint.
            RuleFor(x => x.VesId)
                .Must((eintrag, vesId) => !terminplanung.Any(t => t.Id != eintrag.Id && t.VesId == vesId))
                .When(x => x.VesId != Guid.Empty)
                .WithMessage(_ => TranslationHelper.Translate("VALIDATOR_VERFAHRENSSCHRITT_BEREITS_VORHANDEN"));
        }
    }
}
