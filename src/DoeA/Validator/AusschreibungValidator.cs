// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Constants;
using DoeA.Data.Models.DTO;
using ItreeMud.Models;
using FluentValidation;

namespace DoeA.Validator;

/// <summary>Validierungsregeln für Ausschreibung.</summary>
public class AusschreibungValidator : AbstractValidator<Ausschreibung>
{
    public AusschreibungValidator(bool withoutBaseValidator = false)
    {
        if (!withoutBaseValidator)
        {
            Include(new BasisValidator());
        }

        // Solange kein Publikationsdatum eingetragen ist, sind nur Jahr, Projektart,
        // Verfahrensart und Bezeichnung obligatorisch; die übrigen Pflichtfelder greifen
        // erst ab gesetzter Publikation.
        RuleFor(x => x.PraId).NotEmpty().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator));
        RuleFor(x => x.VeaId).NotEmpty().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator));

        RuleFor(x => x.AmtId).NotEmpty().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator))
            .When(x => x.Publikation != null);
        RuleFor(x => x.RiaId).NotEmpty().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator))
            .When(x => x.Publikation != null);
        RuleFor(x => x.LeaId).NotEmpty().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator))
            .When(x => x.Publikation != null);
        RuleFor(x => x.SacDoeAId).NotEmpty().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator))
            .When(x => x.Publikation != null);
        RuleFor(x => x.Jahr).NotNull().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator));
        RuleFor(x => x.Jahr)
            .LessThanOrEqualTo(2999)
            .WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.LessOrEqualValidator), 2999))
            .GreaterThanOrEqualTo(2000)
            .WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.GreaterOrEqualValidator), 2000));

        // Ein erfasstes Abschlussjahr darf nicht vor dem Jahr der Ausschreibung liegen.
        RuleFor(x => x.AbschlussJahr)
            .GreaterThanOrEqualTo(x => x.Jahr)
            .WithMessage(x => string.Format(TranslationHelper.Translate(GeneralConstants.GreaterOrEqualValidator), x.Jahr))
            .When(x => x.AbschlussJahr != null && x.Jahr != null);

        RuleFor(x => x.Eingereicht).PflichtBeiPublikation(x => x.Publikation);
        RuleFor(x => x.Publikation).NichtVorZustellung(x => x.Eingereicht);
        RuleFor(x => x.EingereichtWiderruf).PflichtBeiPublikation(x => x.PublikationWiderruf);
        RuleFor(x => x.PublikationWiderruf).NichtVorZustellung(x => x.EingereichtWiderruf);

        RuleFor(x => x.Bezeichnung)
            .NotEmpty()
            .WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator));

        RuleFor(x => x.Bezeichnung)
            .MinimumLength(2)
            .WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.MinLengthValidator), 2))
            .MaximumLength(1000)
            .WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.MaxLengthValidator), 1000))
            .When(x => !string.IsNullOrWhiteSpace(x.Bezeichnung));

        RuleForEach(x => x.Berichtigungen).SetValidator(new BerichtigungValidator(true));
    }
}
