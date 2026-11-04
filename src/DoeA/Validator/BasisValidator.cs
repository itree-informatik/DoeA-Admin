// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Constants;
using DoeA.Data.Models.DTO;
using ItreeMud.Models;
using FluentValidation;

namespace DoeA.Validator;

/// <summary>Gemeinsame Regeln aller Entitäten: Id gesetzt und plausible Audit-Zeitstempel.</summary>
public class BasisValidator : AbstractValidator<Basis>
{
    /// <summary>Untergrenze für plausible Audit-Zeitstempel.</summary>
    public static readonly DateTime DateTooOld = new(1999, 12, 31);
    public BasisValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator));
        RuleFor(x => x.InsertedDate)
            .GreaterThan(DateTooOld.Ticks).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.DateConflictToOld), "InsertedDate"))
            .LessThan(_ => DateTime.Now.AddMinutes(1).Ticks).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.DateConflictToYoung), "InsertedDate"));
        RuleFor(x => x.InsertedUser)
            .NotEmpty().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator))
            .MaximumLength(255).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.MaxLengthValidator), 255));
        RuleFor(x => x.ChangedDate)
            .GreaterThan(DateTooOld.Ticks).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.DateConflictToOld), "ChangedDate"))
            .LessThan(_ => DateTime.Now.AddMinutes(1).Ticks).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.DateConflictToYoung), "ChangedDate"));
        RuleFor(x => x.ChangedUser)
            .NotEmpty().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator))
            .MaximumLength(255).WithMessage(_ => string.Format(TranslationHelper.Translate(GeneralConstants.MaxLengthValidator), 255));
    }
}
