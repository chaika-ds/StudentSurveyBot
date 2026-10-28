using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace StudentSurveyBot.Bot;

public sealed class UpdateHandler
{
    private readonly StartCommandHandler _startCommandHandler;
    private readonly SurveyHandler _surveyHandler;
    private readonly AdminHandler _adminHandler;
    private readonly MatchingHandler _matchingHandler;
    private readonly ILogger<UpdateHandler> _logger;

    public UpdateHandler(
        StartCommandHandler startCommandHandler,
        SurveyHandler surveyHandler,
        AdminHandler adminHandler,
        MatchingHandler matchingHandler,
        ILogger<UpdateHandler> logger)
    {
        _startCommandHandler = startCommandHandler;
        _surveyHandler = surveyHandler;
        _adminHandler = adminHandler;
        _matchingHandler = matchingHandler;
        _logger = logger;
    }

    public Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message && update.Message is not null)
        {
            var message = update.Message;
            if (message.Text is not null)
            {
                if (message.Text.StartsWith("/start", StringComparison.Ordinal))
                {
                    return _startCommandHandler.HandleAsync(message, cancellationToken);
                }

                if (message.Text.StartsWith("/export", StringComparison.Ordinal) ||
                    message.Text.StartsWith("/stats", StringComparison.Ordinal))
                {
                    return _adminHandler.HandleAsync(message, cancellationToken);
                }

                if (message.Text.StartsWith("/match", StringComparison.Ordinal))
                {
                    return _matchingHandler.HandleAsync(message, cancellationToken);
                }

                return _surveyHandler.HandleTextAsync(message, cancellationToken);
            }
        }

        if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery is not null)
        {
            return _surveyHandler.HandleCallbackAsync(update.CallbackQuery, cancellationToken);
        }

        return Task.CompletedTask;
    }
}
