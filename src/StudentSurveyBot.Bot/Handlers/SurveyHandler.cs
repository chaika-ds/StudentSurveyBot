using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace StudentSurveyBot.Bot;

public sealed class SurveyHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly IUserStateStore _userStateStore;

    public SurveyHandler(ITelegramBotClient botClient, IUserStateStore userStateStore)
    {
        _botClient = botClient;
        _userStateStore = userStateStore;
    }

    public async Task HandleTextAsync(Message message, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var state = await _userStateStore.GetAsync(chatId, cancellationToken) ?? new SurveyState { ChatId = chatId };

        if (state.CurrentStep == SurveyStep.FormApplicant && string.IsNullOrWhiteSpace(state.FullName))
        {
            if (message.Text is null || !Regex.IsMatch(message.Text, "^[A-Za-zА-Яа-яЁё\\s-]{2,120}$"))
            {
                await _botClient.SendTextMessage(
                    chatId: chatId,
                    text: "ФИО должно содержать 2–120 символов, только буквы, пробелы и дефис.",
                    cancellationToken: cancellationToken);
                return;
            }

            state.FullName = message.Text.Trim();
            state.CurrentStep = SurveyStep.FormApplicant;
            await _userStateStore.SaveAsync(chatId, state, cancellationToken);

            await _botClient.SendTextMessage(
                chatId: chatId,
                text: "Направление: выбери вариант или напиши своё.",
                cancellationToken: cancellationToken);
            return;
        }

        if (state.CurrentStep == SurveyStep.TelegramNick)
        {
            if (!string.IsNullOrWhiteSpace(message.Text))
            {
                var nick = message.Text.Trim();
                if (!Regex.IsMatch(nick, "^@?[A-Za-z0-9_]{4,32}$"))
                {
                    await _botClient.SendTextMessage(
                        chatId: chatId,
                        text: "Ник должен быть вида @username или username, длиной 4–32 символа.",
                        cancellationToken: cancellationToken);
                    return;
                }

                state.ContactTelegramNick = nick;
            }

            state.CurrentStep = SurveyStep.Thanks;
            await _userStateStore.SaveAsync(chatId, state, cancellationToken);

            await _botClient.SendTextMessage(
                chatId: chatId,
                text: $"Спасибо за регистрацию, {state.FullName}!\n\nКраткая сводка: {state.FullName} / {state.Direction ?? "не указано"} / {state.Course ?? "не указан"}.",
                cancellationToken: cancellationToken);
            return;
        }

        await _botClient.SendTextMessage(
            chatId: chatId,
            text: "Сейчас обработка этого текста не предусмотрена. Перейдите к шагу анкеты.",
            cancellationToken: cancellationToken);
    }

    public async Task HandleCallbackAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        if (callbackQuery.Data is null)
        {
            return;
        }

        var chatId = callbackQuery.Message?.Chat.Id ?? callbackQuery.From.Id;
        var state = await _userStateStore.GetAsync(chatId, cancellationToken) ?? new SurveyState { ChatId = chatId };

        if (callbackQuery.Data.StartsWith("direction:", StringComparison.Ordinal))
        {
            state.Direction = callbackQuery.Data["direction:".Length..];
            await _userStateStore.SaveAsync(chatId, state, cancellationToken);
            await _botClient.AnswerCallbackQuery(callbackQuery.Id, "Направление сохранено.", cancellationToken: cancellationToken);
            return;
        }

        if (callbackQuery.Data.StartsWith("course:", StringComparison.Ordinal))
        {
            state.Course = callbackQuery.Data["course:".Length..];
            state.CurrentStep = SurveyStep.InterestSelection;
            await _userStateStore.SaveAsync(chatId, state, cancellationToken);
            await _botClient.AnswerCallbackQuery(callbackQuery.Id, "Курс сохранён.", cancellationToken: cancellationToken);
            return;
        }

        if (callbackQuery.Data.Equals("consent:yes", StringComparison.Ordinal))
        {
            state.ConsentGiven = true;
            state.ConsentVersion = "v1.0";
            state.ConsentTextHash = "sha256:placeholder";
            state.ConsentGivenAt = DateTimeOffset.UtcNow;
            state.CurrentStep = SurveyStep.TelegramNick;
            await _userStateStore.SaveAsync(chatId, state, cancellationToken);
            await _botClient.SendTextMessage(chatId, "Введите Telegram username или @username. Можно пропустить.", cancellationToken: cancellationToken);
            return;
        }

        if (callbackQuery.Data.Equals("consent:no", StringComparison.Ordinal))
        {
            state.ConsentGiven = false;
            state.CurrentStep = SurveyStep.None;
            await _userStateStore.SaveAsync(chatId, state, cancellationToken);
            await _botClient.SendTextMessage(chatId, "Согласие не получено. Диалог завершён.", cancellationToken: cancellationToken);
            return;
        }

        await _botClient.AnswerCallbackQuery(callbackQuery.Id, "Обработка callback не реализована.", cancellationToken: cancellationToken);
    }
}
