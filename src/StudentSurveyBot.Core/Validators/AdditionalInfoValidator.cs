using FluentValidation;
using StudentSurveyBot.Core.Entities;

namespace StudentSurveyBot.Core.Validators;

public sealed class AdditionalInfoValidator : AbstractValidator<AdditionalInfo>
{
    public AdditionalInfoValidator()
    {
        RuleFor(x => x.SpecialWishes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.SpecialWishes))
            .WithMessage("Особые пожелания не должны превышать 500 символов.");
    }
}
