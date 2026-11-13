using StudentSurveyBot.Bot.Abstractions;
using StudentSurveyBot.Bot.States;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace StudentSurveyBot.Bot.Handlers;

public class SurveyCallbackHandler : SurveyHandlerBase
{
    public SurveyCallbackHandler(
        ITelegramBotClient botClient,
        IUserStateStore userStateStore)
        : base(botClient, userStateStore)
    {
    }

    public async Task HandleAsync(
        CallbackQuery callbackQuery,
        CancellationToken cancellationToken)
    {
        var data = callbackQuery.Data;

        if (data is null)
        {
            return;
        }

        var chatId = GetChatId(callbackQuery);
        var state = await GetStateAsync(chatId, cancellationToken);

        if (await HandleDirectionAsync(
                callbackQuery,
                data,
                state,
                chatId,
                cancellationToken))
        {
            return;
        }

        if (await HandleCourseAsync(
                callbackQuery,
                data,
                state,
                chatId,
                cancellationToken))
        {
            return;
        }

        if (await HandleConsentAsync(
                callbackQuery,
                data,
                state,
                chatId,
                cancellationToken))
        {
            return;
        }

        await BotClient.AnswerCallbackQuery(
            callbackQuery.Id,
            "Обработка callback не реализована.",
            cancellationToken: cancellationToken);
    }

    private async Task<bool> HandleDirectionAsync(
        CallbackQuery callbackQuery,
        string data,
        SurveyState state,
        long chatId,
        CancellationToken cancellationToken)
    {
        const string prefix = "direction:";

        if (!data.StartsWith(prefix, StringComparison.Ordinal))
        {
            return false;
        }

        var direction = StudyDirectionCatalog.Find(data[prefix.Length..]);

        if (direction is null)
        {
            await BotClient.AnswerCallbackQuery(
                callbackQuery.Id,
                "Вариант направления не найден.",
                cancellationToken: cancellationToken);

            return true;
        }

        state.Direction = direction.DisplayName;

        await SaveStateAsync(chatId, state, cancellationToken);

        await BotClient.AnswerCallbackQuery(
            callbackQuery.Id,
            "Направление сохранено.",
            cancellationToken: cancellationToken);

        await SendMessageAsync(
            chatId,
            $"Направление сохранено: {direction.DisplayName}.",
            cancellationToken);

        return true;
    }

    private async Task<bool> HandleCourseAsync(
        CallbackQuery callbackQuery,
        string data,
        SurveyState state,
        long chatId,
        CancellationToken cancellationToken)
    {
        const string prefix = "course:";

        if (!data.StartsWith(prefix, StringComparison.Ordinal))
        {
            return false;
        }

        state.Course = data[prefix.Length..];
        state.CurrentStep = SurveyStep.InterestSelection;

        await SaveStateAsync(chatId, state, cancellationToken);

        await BotClient.AnswerCallbackQuery(
            callbackQuery.Id,
            "Курс сохранён.",
            cancellationToken: cancellationToken);

        return true;
    }

    private async Task<bool> HandleConsentAsync(
        CallbackQuery callbackQuery,
        string data,
        SurveyState state,
        long chatId,
        CancellationToken cancellationToken)
    {
        if (data.Equals("consent:yes", StringComparison.Ordinal))
        {
            state.ConsentGiven = true;
            state.ConsentVersion = ConsentVersion;
            state.ConsentTextHash = ConsentTextHash;
            state.ConsentGivenAt = DateTimeOffset.UtcNow;
            state.CurrentStep = SurveyStep.TelegramNick;

            await SaveStateAsync(chatId, state, cancellationToken);

            await SendMessageAsync(
                chatId,
                "Введите Telegram username или @username. Можно пропустить.",
                cancellationToken);

            return true;
        }

        if (data.Equals("consent:no", StringComparison.Ordinal))
        {
            state.ConsentGiven = false;
            state.CurrentStep = SurveyStep.None;

            await SaveStateAsync(chatId, state, cancellationToken);

            await SendMessageAsync(
                chatId,
                "Согласие не получено. Диалог завершён.",
                cancellationToken);

            return true;
        }

        return false;
    }
}