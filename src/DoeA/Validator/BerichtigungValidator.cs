// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Models.DTO;
using ItreeMud.Models;
using FluentValidation;

namespace DoeA.Validator;

/// <summary>Validierungsregeln für Berichtigung.</summary>
public class BerichtigungValidator : AbstractValidator<Berichtigung>
{
    public BerichtigungValidator(bool withoutBaseValidator = false)
    {
        if (!withoutBaseValidator)
        {
            Include(new BasisValidator());
        }

        // Eine Berichtigung ohne jedes Datum darf nicht gespeichert werden:
        // mindestens eines der beiden Daten (eingereicht/publiziert) muss erfasst sein.
        RuleFor(x => x.Eingereicht)
            .Must((ber, _) => ber.Eingereicht != null || ber.Publiziert != null)
            .WithMessage(_ => TranslationHelper.Translate("VALIDATOR_BERICHTIGUNG_MINDESTENS_EIN_DATUM"));
        RuleFor(x => x.Eingereicht).PflichtBeiPublikation(x => x.Publiziert);
        RuleFor(x => x.Publiziert).NichtVorZustellung(x => x.Eingereicht);
    }
}
