using StudentSurveyBot.Bot.States;
using Telegram.Bot.Types;

namespace StudentSurveyBot.Bot.Handlers;

public interface ISurveyCallbackStrategy
{
    Task<bool> HandleAsync(
        CallbackQuery callbackQuery,
        string data,
        SurveyState state,
        long chatId,
        CancellationToken cancellationToken);
}
