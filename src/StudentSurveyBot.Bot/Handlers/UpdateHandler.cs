using Microsoft.Extensions.Logging;
using System.Collections.Frozen;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace StudentSurveyBot.Bot.Handlers;

public sealed class UpdateHandler
{
    private readonly FrozenDictionary<string, IUpdateStrategy> _strategies;
    private readonly IChatRequestSynchronizer _requestSynchronizer;

    public UpdateHandler(
        IEnumerable<IUpdateStrategy> strategies,
        IChatRequestSynchronizer requestSynchronizer)
    {
        _strategies = strategies
            .SelectMany(strategy => strategy.Keys.Select(key => new { key, strategy }))
            .ToFrozenDictionary(item => item.key, item => item.strategy);
        _requestSynchronizer = requestSynchronizer;
    }

    public async Task HandleAsync(
        ITelegramBotClient _,
        Update update,
        CancellationToken cancellationToken)
    {
        var chatId = GetChatId(update);

        if (chatId is null)
        {
            return;
        }

        await _requestSynchronizer.ExecuteAsync(
            chatId.Value,
            token => DispatchAsync(update, token),
            cancellationToken);
    }

    private async Task DispatchAsync(
        Update update,
        CancellationToken cancellationToken)
    {
        if (_strategies.TryGetValue(GetStrategyKey(update), out var strategy) &&
            strategy.CanHandle(update))
        {
            await strategy.HandleAsync(update, cancellationToken);
        }
    }

    private static string GetStrategyKey(Update update)
    {
        if (update.Type == UpdateType.CallbackQuery)
        {
            return UpdateStrategyKeys.Callback;
        }

        if (update.Type != UpdateType.Message ||
            update.Message?.Text is not { } text)
        {
            return string.Empty;
        }

        if (text.StartsWith("/start", StringComparison.Ordinal))
        {
            return UpdateStrategyKeys.Start;
        }

        if (text.StartsWith("/export", StringComparison.Ordinal))
        {
            return UpdateStrategyKeys.Export;
        }

        if (text.StartsWith("/stats", StringComparison.Ordinal))
        {
            return UpdateStrategyKeys.Stats;
        }

        if (text.StartsWith("/match", StringComparison.Ordinal))
        {
            return UpdateStrategyKeys.Match;
        }

        return UpdateStrategyKeys.Message;
    }

    private static long? GetChatId(Update update)
    {
        if (update.Message is not null)
        {
            return update.Message.Chat.Id;
        }

        if (update.CallbackQuery is not null)
        {
            return update.CallbackQuery.Message?.Chat.Id
                   ?? update.CallbackQuery.From.Id;
        }

        return null;
    }
}
