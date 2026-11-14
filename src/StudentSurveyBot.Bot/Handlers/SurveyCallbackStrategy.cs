using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace StudentSurveyBot.Bot.Handlers;

public sealed class SurveyCallbackStrategy : IUpdateStrategy
{
    public IReadOnlyCollection<string> Keys { get; } = [UpdateStrategyKeys.Callback];

    private readonly SurveyCallbackHandler _handler;

    public SurveyCallbackStrategy(SurveyCallbackHandler handler)
    {
        _handler = handler;
    }

    public bool CanHandle(Update update) =>
        update.Type == UpdateType.CallbackQuery &&
        update.CallbackQuery is not null;

    public Task HandleAsync(Update update, CancellationToken cancellationToken) =>
        _handler.HandleAsync(update.CallbackQuery!, cancellationToken);
}
