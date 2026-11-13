using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace StudentSurveyBot.Bot.Handlers;

public sealed class UpdateHandler
{
    private readonly StartCommandHandler _startCommandHandler;
    private readonly SurveyTextHandler _surveyTextHandler;
    private readonly SurveyCallbackHandler _surveyCallbackHandler;
    private readonly AdminHandler _adminHandler;
    private readonly MatchingHandler _matchingHandler;
    private readonly IChatRequestSynchronizer _requestSynchronizer;

    public UpdateHandler(
        StartCommandHandler startCommandHandler,
        SurveyTextHandler surveyTextHandler,
        SurveyCallbackHandler surveyCallbackHandler,
        AdminHandler adminHandler,
        MatchingHandler matchingHandler,
        IChatRequestSynchronizer requestSynchronizer)
    {
        _startCommandHandler = startCommandHandler;
        _surveyTextHandler = surveyTextHandler;
        _surveyCallbackHandler = surveyCallbackHandler;
        _adminHandler = adminHandler;
        _matchingHandler = matchingHandler;
        _requestSynchronizer = requestSynchronizer;
    }

    public async Task HandleAsync(
        ITelegramBotClient botClient,
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
        if (update.Type == UpdateType.Message &&
            update.Message is not null &&
            update.Message.Text is not null)
        {
            var message = update.Message;

            if (message.Text.StartsWith(
                    "/start",
                    StringComparison.Ordinal))
            {
                await _startCommandHandler.HandleAsync(
                    message,
                    cancellationToken);

                return;
            }

            if (message.Text.StartsWith(
                    "/export",
                    StringComparison.Ordinal) ||
                message.Text.StartsWith(
                    "/stats",
                    StringComparison.Ordinal))
            {
                await _adminHandler.HandleAsync(
                    message,
                    cancellationToken);

                return;
            }

            if (message.Text.StartsWith(
                    "/match",
                    StringComparison.Ordinal))
            {
                await _matchingHandler.HandleAsync(
                    message,
                    cancellationToken);

                return;
            }

            await _surveyTextHandler.HandleAsync(
                message,
                cancellationToken);

            return;
        }

        if (update.Type == UpdateType.CallbackQuery &&
            update.CallbackQuery is not null)
        {
            await _surveyCallbackHandler.HandleAsync(
                update.CallbackQuery,
                cancellationToken);
        }
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
