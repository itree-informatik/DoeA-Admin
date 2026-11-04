// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using FluentValidation;
using static DoeA.Data.Helper.TranslationHelper;

namespace DoeA.Validator.Einstellungen;

/// <summary>Validierungsregeln für TranslationItem.</summary>
public class TranslationValidator : AbstractValidator<TranslationItem>
{
    public TranslationValidator()
    {
        RuleFor(x => x.Deutsch)
            .NotEmpty()
            .WithMessage(_ => TranslationHelper.Translate("VALIDATOR_NOTEMPTY"));
    }
}
