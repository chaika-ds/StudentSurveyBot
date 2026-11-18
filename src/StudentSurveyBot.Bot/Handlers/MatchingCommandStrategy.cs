using StudentSurveyBot.Bot.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace StudentSurveyBot.Bot.Handlers;

public sealed class MatchingCommandStrategy : IUpdateStrategy
{
    public IReadOnlyCollection<string> Keys { get; } = [UpdateStrategyKeys.Match];

    private readonly MatchingHandler _handler;

    public MatchingCommandStrategy(MatchingHandler handler)
    {
        _handler = handler;
    }

    public bool CanHandle(Update update) =>
        update.Type == UpdateType.Message &&
        update.Message is not null &&
        update.Message.Text is not null &&
        update.Message.Text.StartsWith("/match", StringComparison.Ordinal);

    public Task HandleAsync(Update update, CancellationToken cancellationToken) =>
        _handler.HandleAsync(update.Message!, cancellationToken);
}
