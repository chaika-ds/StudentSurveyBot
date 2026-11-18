using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StudentSurveyBot.Bot.Handlers;
using StudentSurveyBot.Bot.Middleware;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace StudentSurveyBot.Bot;

public sealed class TelegramPollingService : IHostedService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ExceptionHandlingMiddleware _exceptionHandlingMiddleware;
    private readonly ILogger<TelegramPollingService> _logger;

    public TelegramPollingService(
        ITelegramBotClient botClient,
        ExceptionHandlingMiddleware exceptionHandlingMiddleware,
        ILogger<TelegramPollingService> logger)
    {
        _botClient = botClient;
        _exceptionHandlingMiddleware = exceptionHandlingMiddleware;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Telegram polling started.");

        _botClient.StartReceiving(
            updateHandler: (client, update, token) =>
                _exceptionHandlingMiddleware.InvokeAsync(client, update, token),
            errorHandler: (client, exception, source, token) =>
            {
                _logger.LogError(exception, "Telegram polling error.");
                return Task.CompletedTask;
            },
            receiverOptions: new ReceiverOptions
            {
                AllowedUpdates = [UpdateType.Message, UpdateType.CallbackQuery]
            },
            cancellationToken: cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Telegram polling stopped.");
        return Task.CompletedTask;
    }
}
