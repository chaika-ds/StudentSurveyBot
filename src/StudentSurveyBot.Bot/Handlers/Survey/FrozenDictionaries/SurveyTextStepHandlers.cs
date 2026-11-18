using System.Collections.Frozen;
using Telegram.Bot.Types;
using StudentSurveyBot.Bot.States;

namespace StudentSurveyBot.Bot.Handlers.Survey.FrozenDictionaries;

internal static class SurveyTextStepHandlers
{
    public static FrozenDictionary<
        SurveyStep,
        Func<SurveyTextHandler, Message, SurveyState, long, CancellationToken, Task<bool>>[]> Create()
    {
        return new Dictionary<
            SurveyStep,
            Func<SurveyTextHandler, Message, SurveyState, long, CancellationToken, Task<bool>>[]>
        {
            [SurveyStep.Wishes] =
            [
                static (handler, message, state, chatId, cancellationToken) =>
                    handler.HandleSpecialWishesAsync(message, state, chatId, cancellationToken)
            ],
            [SurveyStep.FormApplicant] =
            [
                static (handler, message, state, chatId, cancellationToken) =>
                    handler.HandleApplicantNameAsync(message, state, chatId, cancellationToken),
                static (handler, message, state, chatId, cancellationToken) =>
                    handler.HandleDirectionAsync(message, state, chatId, cancellationToken)
            ],
            [SurveyStep.TelegramNick] =
            [
                static (handler, message, state, chatId, cancellationToken) =>
                    handler.HandleTelegramNickAsync(message, state, chatId, cancellationToken)
            ]
        }.ToFrozenDictionary();
    }
}
