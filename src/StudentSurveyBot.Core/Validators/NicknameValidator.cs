using FluentValidation;

namespace StudentSurveyBot.Core.Validators;

public sealed class NicknameValidator : AbstractValidator<string?>
{
    public NicknameValidator()
    {
        RuleFor(x => x)
            .Matches("^@?[A-Za-z0-9_]{4,32}$")
            .When(x => !string.IsNullOrWhiteSpace(x))
            .WithMessage("Валидный формат Telegram username: @username или username.");
    }
}
