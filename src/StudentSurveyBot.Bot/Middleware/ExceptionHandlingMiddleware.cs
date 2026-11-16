using Microsoft.Extensions.Logging;
using StudentSurveyBot.Bot.Extensions;
using StudentSurveyBot.Bot.Handlers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace StudentSurveyBot.Bot.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly UpdateHandler _next;
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        UpdateHandler next,
        ITelegramBotClient botClient,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _botClient = botClient;
        _logger = logger;
    }

    public async Task InvokeAsync(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken)
    {
        try
        {
            await _next.HandleAsync(botClient, update, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            var chatId = GetChatId(update);

            _logger.LogError(
                exception,
                "Ошибка при обработке Telegram update для чата {ChatId}.",
                chatId);

            if (chatId is null)
            {
                return;
            }

            try
            {
                await _botClient.SendMessageAsync(
                    chatId.Value,
                    "Сейчас возникли некоторые проблемы. Попробуйте ещё раз позже.",
                    CancellationToken.None);
            }
            catch (Exception notificationException)
            {
                _logger.LogError(
                    notificationException,
                    "Не удалось отправить сообщение об ошибке в чат {ChatId}.",
                    chatId.Value);
            }
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
