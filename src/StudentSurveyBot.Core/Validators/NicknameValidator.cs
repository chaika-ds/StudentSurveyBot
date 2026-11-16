using FluentValidation;

namespace StudentSurveyBot.Core.Validators;

public sealed class NicknameValidator : AbstractValidator<string?>
{
    public NicknameValidator()
    {
        RuleFor(x => x)
            .Matches(@"^(?:@?[A-Za-z][A-Za-z0-9_]{3,31}|(?:https?://)?t\.me/[A-Za-z][A-Za-z0-9_]{3,31}/?)$")
            .When(x => !string.IsNullOrWhiteSpace(x))
            .WithMessage("Укажите Telegram username (@username) или ссылку вида https://t.me/username.");
    }
}
