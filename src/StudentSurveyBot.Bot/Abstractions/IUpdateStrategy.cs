using Telegram.Bot.Types;

namespace StudentSurveyBot.Bot.Abstractions;

public interface IUpdateStrategy
{
    IReadOnlyCollection<string> Keys { get; }

    bool CanHandle(Update update);

    Task HandleAsync(Update update, CancellationToken cancellationToken);
}
