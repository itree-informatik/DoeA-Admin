// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using DoeA.Data.Constants;
using DoeA.Data.Models.DTO;
using ItreeMud.Models;
using FluentValidation;

namespace DoeA.Validator;

/// <summary>Validierungsregeln für OfferteOffertezustand.</summary>
public class OfferteOffertezustandValidator : AbstractValidator<OfferteOffertezustand>
{
    /// <param name="withoutBaseValidator">Ohne die Regeln der Basisfelder (Client-Validierung).</param>
    /// <param name="publikationOfzIds">Offertezustände mit Publikation: Nur bei ihnen ist «Eingereicht» Pflicht.</param>
    public OfferteOffertezustandValidator(bool withoutBaseValidator = false, IReadOnlySet<Guid>? publikationOfzIds = null)
    {
        if (!withoutBaseValidator)
        {
            Include(new BasisValidator());
        }

        RuleFor(x => x.OfzId).NotEmpty().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator));
        RuleFor(x => x.Datum).NotNull().WithMessage(_ => TranslationHelper.Translate(GeneralConstants.NotEmptyValidator));
        // Bei Zuständen mit Publikation ist Datum das Publikationsdatum; ohne Publikation fehlt «Eingereicht», die Regel greift nicht.
        RuleFor(x => x.Eingereicht).PflichtBeiPublikation(ZustandDatum)
            .When(x => publikationOfzIds?.Contains(x.OfzId) == true);
        RuleFor(x => ZustandDatum(x))
            .NichtVorZustellung(x => x.Eingereicht)
            .OverridePropertyName(nameof(OfferteOffertezustand.Datum));
    }

    private static DateOnly? ZustandDatum(OfferteOffertezustand zustand) =>
        zustand.Datum.HasValue ? DateOnly.FromDateTime(zustand.Datum.Value) : null;
}
