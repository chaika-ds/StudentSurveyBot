using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace StudentSurveyBot.Bot.Handlers;

public sealed class SurveyMessageStrategy : IUpdateStrategy
{
    public IReadOnlyCollection<string> Keys { get; } = [UpdateStrategyKeys.Message];

    private readonly SurveyTextHandler _handler;

    public SurveyMessageStrategy(SurveyTextHandler handler)
    {
        _handler = handler;
    }

    public bool CanHandle(Update update) =>
        update.Type == UpdateType.Message &&
        update.Message is not null &&
        update.Message.Text is not null;

    public Task HandleAsync(Update update, CancellationToken cancellationToken) =>
        _handler.HandleAsync(update.Message!, cancellationToken);
}
