using Telegram.Bot.Types;
using StudentSurveyBot.Bot.States;

namespace StudentSurveyBot.Bot.Abstractions;

public interface ISurveyCallbackStrategy
{
    Task<bool> HandleAsync(
        CallbackQuery callbackQuery,
        string data,
        SurveyState state,
        long chatId,
        CancellationToken cancellationToken);
}
