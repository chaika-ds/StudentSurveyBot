using StudentSurveyBot.Bot.Abstractions;
using StudentSurveyBot.Bot.States;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace StudentSurveyBot.Bot.Handlers;

public class SurveyTextHandler : SurveyHandlerBase
{
    private static readonly Regex FullNameRegex = new(
        "^[A-Za-zА-Яа-яЁё\\s-]{2,120}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex TelegramNickRegex = new(
        "^@?[A-Za-z0-9_]{4,32}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public SurveyTextHandler(
        ITelegramBotClient botClient,
        IUserStateStore userStateStore)
        : base(botClient, userStateStore)
    {
    }

    public async Task HandleAsync(
        Message message,
        CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var state = await GetStateAsync(chatId, cancellationToken);

        if (await HandleDirectionAsync(message, state, chatId, cancellationToken))
        {
            return;
        }

        state.FullName = message.Text!.Trim();

        if (await HandleApplicantNameAsync(
                message,
                state,
                chatId,
                cancellationToken))
        {
            return;
        }

        if (await HandleTelegramNickAsync(
                message,
                state,
                chatId,
                cancellationToken))
        {
            return;
        }

        await SendMessageAsync(
            chatId,
            "Сейчас обработка этого текста не предусмотрена. Перейдите к шагу анкеты.",
            cancellationToken);
    }

    private async Task<bool> HandleApplicantNameAsync(
        Message message,
        SurveyState state,
        long chatId,
        CancellationToken cancellationToken)
    {
        if (state.CurrentStep != SurveyStep.FormApplicant ||
            string.IsNullOrWhiteSpace(state.FullName))
        {
            return false;
        }

        if (message.Text is null || !FullNameRegex.IsMatch(message.Text))
        {
            await SendMessageAsync(
                chatId,
                "ФИО должно содержать 2–120 символов, только буквы, пробелы и дефис.",
                cancellationToken);

            return true;
        }

        state.FullName = message.Text.Trim();

        await SaveStateAsync(chatId, state, cancellationToken);

        await SendMessageAsync(
            chatId,
            "Направление: выбери вариант или напиши своё.",
            cancellationToken);

        foreach (var group in StudyDirectionCatalog.Groups)
        {
            var keyboard = new InlineKeyboardMarkup(
                group.Directions.Select(direction =>
                    new[] { InlineKeyboardButton.WithCallbackData(
                        direction.DisplayName,
                        $"direction:{direction.Key}") }));

            await SendMessageAsync(
                chatId,
                group.Name,
                keyboard,
                cancellationToken);
        }

        return true;
    }

    private async Task<bool> HandleDirectionAsync(
        Message message,
        SurveyState state,
        long chatId,
        CancellationToken cancellationToken)
    {
        if (state.CurrentStep != SurveyStep.FormApplicant ||
            string.IsNullOrWhiteSpace(state.FullName) ||
            string.IsNullOrWhiteSpace(message.Text))
        {
            return false;
        }

        state.Direction = message.Text.Trim();
        await SaveStateAsync(chatId, state, cancellationToken);

        await SendMessageAsync(
            chatId,
            "Направление сохранено.",
            cancellationToken);

        return true;
    }

    private async Task<bool> HandleTelegramNickAsync(
        Message message,
        SurveyState state,
        long chatId,
        CancellationToken cancellationToken)
    {
        if (state.CurrentStep != SurveyStep.TelegramNick)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(message.Text))
        {
            var nick = message.Text.Trim();

            if (!TelegramNickRegex.IsMatch(nick))
            {
                await SendMessageAsync(
                    chatId,
                    "Ник должен быть вида @username или username, длиной 4–32 символа.",
                    cancellationToken);

                return true;
            }

            state.ContactTelegramNick = nick;
        }

        state.CurrentStep = SurveyStep.Thanks;

        await SaveStateAsync(chatId, state, cancellationToken);

        await SendMessageAsync(
            chatId,
            $"Спасибо за регистрацию, {state.FullName}!\n\n" +
            $"Краткая сводка: {state.FullName} / " +
            $"{state.Direction ?? "не указано"} / " +
            $"{state.Course ?? "не указан"}.",
            cancellationToken);

        return true;
    }
}