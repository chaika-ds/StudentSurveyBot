using StudentSurveyBot.Bot.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace StudentSurveyBot.Bot.Handlers;

public sealed class AdminCommandStrategy : IUpdateStrategy
{
    public IReadOnlyCollection<string> Keys { get; } =
        [UpdateStrategyKeys.Export, UpdateStrategyKeys.Stats];

    private readonly AdminHandler _handler;

    public AdminCommandStrategy(AdminHandler handler)
    {
        _handler = handler;
    }

    public bool CanHandle(Update update)
    {
        if (!IsTextMessage(update))
        {
            return false;
        }

        var text = update.Message!.Text!;
        return text.StartsWith("/export", StringComparison.Ordinal) ||
               text.StartsWith("/stats", StringComparison.Ordinal);
    }

    public Task HandleAsync(Update update, CancellationToken cancellationToken) =>
        _handler.HandleAsync(update.Message!, cancellationToken);

    private static bool IsTextMessage(Update update) =>
        update.Type == UpdateType.Message &&
        update.Message is not null &&
        update.Message.Text is not null;
}
