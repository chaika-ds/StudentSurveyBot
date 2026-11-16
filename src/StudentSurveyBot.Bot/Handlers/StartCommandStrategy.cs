using StudentSurveyBot.Bot.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace StudentSurveyBot.Bot.Handlers;

public sealed class StartCommandStrategy : IUpdateStrategy
{
    public IReadOnlyCollection<string> Keys { get; } = [UpdateStrategyKeys.Start];

    private readonly StartCommandHandler _handler;

    public StartCommandStrategy(StartCommandHandler handler)
    {
        _handler = handler;
    }

    public bool CanHandle(Update update) =>
        IsTextMessage(update) &&
        update.Message!.Text!.StartsWith("/start", StringComparison.Ordinal);

    public Task HandleAsync(Update update, CancellationToken cancellationToken) =>
        _handler.HandleAsync(update.Message!, cancellationToken);

    private static bool IsTextMessage(Update update) =>
        update.Type == UpdateType.Message &&
        update.Message is not null &&
        update.Message.Text is not null;
}
