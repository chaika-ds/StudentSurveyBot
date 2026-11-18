using FluentValidation;
using System.Text.RegularExpressions;

namespace StudentSurveyBot.Core.Validators;

public sealed class NicknameValidator : AbstractValidator<string?>
{
    private static readonly Regex TelegramNicknameRegex = new(
        "^(?:@?[A-Za-z][A-Za-z0-9_]{3,31}|(?:https?://)?(?:www\\.)?(?:t\\.me|telegram\\.me)/[A-Za-z][A-Za-z0-9_]{3,31}/?)$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase,
        TimeSpan.FromMilliseconds(100));

    public NicknameValidator()
    {
        RuleFor(x => x)
            .Must(value => value is not null && TelegramNicknameRegex.IsMatch(value.Trim()))
            .When(x => !string.IsNullOrWhiteSpace(x))
            .WithMessage("Укажите Telegram username (@username) или ссылку вида https://t.me/username.");
    }
}
