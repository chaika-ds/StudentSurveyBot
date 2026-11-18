using StudentSurveyBot.Bot.States;
using StudentSurveyBot.Bot.Abstractions;
using Telegram.Bot.Types;

namespace StudentSurveyBot.Bot.Handlers.Survey;

public sealed class SurveyCallbackHandlerStrategy : ISurveyCallbackStrategy
{
    private readonly Func<
        CallbackQuery,
        string,
        SurveyState,
        long,
        CancellationToken,
        Task<bool>> _handler;

    public SurveyCallbackHandlerStrategy(
        Func<
            CallbackQuery,
            string,
            SurveyState,
            long,
            CancellationToken,
            Task<bool>> handler)
    {
        _handler = handler;
    }

    public Task<bool> HandleAsync(
        CallbackQuery callbackQuery,
        string data,
        SurveyState state,
        long chatId,
        CancellationToken cancellationToken) =>
        _handler(callbackQuery, data, state, chatId, cancellationToken);
}
