using System.Collections.Frozen;
using StudentSurveyBot.Bot.Abstractions;
using StudentSurveyBot.Bot.Handlers.Survey.FrozenDictionaries;
using StudentSurveyBot.Bot.States;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace StudentSurveyBot.Bot.Handlers.Survey;

public class SurveyCallbackHandler : SurveyHandlerBase
{
    private readonly FrozenDictionary<SurveyCallbackKey, ISurveyCallbackStrategy> _strategies;

    public SurveyCallbackHandler(
        ITelegramBotClient botClient,
        IUserStateStore userStateStore,
        IServiceScopeFactory scopeFactory)
        : base(botClient, userStateStore, scopeFactory)
    {
        _strategies = SurveyCallbackStrategies.Create(this);
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

        var key = GetStrategyKey(data);

        if (_strategies.TryGetValue(key, out var strategy) &&
            await strategy.HandleAsync(
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

    private static SurveyCallbackKey GetStrategyKey(string data)
    {
        var separatorIndex = data.IndexOf(':');

        if (separatorIndex <= 0 ||
            !SurveyCallbackKeys.Values.TryGetValue(data[..separatorIndex], out var key))
        {
            return SurveyCallbackKey.Unknown;
        }

        return key;
    }

    internal async Task<bool> HandleDirectionAsync(
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
        state.CurrentStep = SurveyStep.InterestSelection;

        await SaveStateAsync(chatId, state, cancellationToken);

        await BotClient.AnswerCallbackQuery(
            callbackQuery.Id,
            "Направление сохранено.",
            cancellationToken: cancellationToken);

        await SendMessageAsync(
            chatId,
            $"Направление сохранено: {direction.DisplayName}.",
            cancellationToken);

        await SendMessageAsync(
            chatId,
            "Заполните форму по интересам.\n\nНаправления интересов (можно выбрать несколько):",
            BuildInterestKeyboard(state.SelectedInterestIds),
            cancellationToken);

        return true;
    }

    internal async Task<bool> HandleInterestSelectionAsync(
        CallbackQuery callbackQuery,
        string data,
        SurveyState state,
        long chatId,
        CancellationToken cancellationToken)
    {
        const string prefix = "interest:";

        if (!data.StartsWith(prefix, StringComparison.Ordinal))
        {
            return false;
        }

        var value = data[prefix.Length..];

        if (value.Equals("done", StringComparison.Ordinal))
        {
            await SaveStateAsync(chatId, state, cancellationToken);

            await BotClient.AnswerCallbackQuery(
                callbackQuery.Id,
                "Интересы сохранены.",
                cancellationToken: cancellationToken);

            await SendMessageAsync(
                chatId,
                "Желаемый вайб:",
                new InlineKeyboardMarkup(
                [
                    [InlineKeyboardButton.WithCallbackData(SurveyAnswerCatalog.DesiredVibes["meet-people"], "interest-vibe:meet-people")],
                    [InlineKeyboardButton.WithCallbackData(SurveyAnswerCatalog.DesiredVibes["impressions"], "interest-vibe:impressions")],
                    [InlineKeyboardButton.WithCallbackData(SurveyAnswerCatalog.DesiredVibes["discuss"], "interest-vibe:discuss")],
                    [InlineKeyboardButton.WithCallbackData(SurveyAnswerCatalog.DesiredVibes["just-go-out"], "interest-vibe:just-go-out")]
                ]),
                cancellationToken);

            return true;
        }

        if (!int.TryParse(value, out var interestId) || interestId is < 1 or > 9)
        {
            await BotClient.AnswerCallbackQuery(
                callbackQuery.Id,
                "Вариант интереса не найден.",
                cancellationToken: cancellationToken);

            return true;
        }

        if (state.SelectedInterestIds.Contains(interestId))
        {
            state.SelectedInterestIds.Remove(interestId);
        }
        else
        {
            state.SelectedInterestIds.Add(interestId);
        }

        await SaveStateAsync(chatId, state, cancellationToken);

        if (callbackQuery.Message is not null)
        {
            await BotClient.SendRequest(
                new EditMessageReplyMarkupRequest
                {
                    ChatId = chatId,
                    MessageId = callbackQuery.Message.Id,
                    ReplyMarkup = BuildInterestKeyboard(state.SelectedInterestIds)
                },
                cancellationToken);
        }

        await BotClient.AnswerCallbackQuery(
            callbackQuery.Id,
            state.SelectedInterestIds.Contains(interestId)
                ? "Интерес выбран. Можно выбрать ещё или нажать «Готово»."
                : "Выбор интереса отменён.",
            cancellationToken: cancellationToken);

        return true;
    }

    private static InlineKeyboardMarkup BuildInterestKeyboard(
        IReadOnlyCollection<int> selectedInterestIds)
    {
        var rows = SurveyInterestCatalog.Labels
            .Select(interest => new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    $"{(selectedInterestIds.Contains(interest.Key) ? "✅" : "☐")} {interest.Value}",
                    $"interest:{interest.Key}")
            })
            .Append(new[]
            {
                InlineKeyboardButton.WithCallbackData("Готово", "interest:done")
            });

        return new InlineKeyboardMarkup(rows);
    }

    internal async Task<bool> HandleInterestFormAsync(
        CallbackQuery callbackQuery,
        string data,
        SurveyState state,
        long chatId,
        CancellationToken cancellationToken)
    {
        const string prefix = "interest-vibe:";

        if (!data.StartsWith(prefix, StringComparison.Ordinal))
        {
            return false;
        }

        state.DesiredVibe = data[prefix.Length..];
        await SaveStateAsync(chatId, state, cancellationToken);

        await BotClient.AnswerCallbackQuery(
            callbackQuery.Id,
            "Ответ сохранён.",
            cancellationToken: cancellationToken);

        await SendMessageAsync(
            chatId,
            "Социальный профиль:\n\nСтиль знакомства:",
            new InlineKeyboardMarkup(
            [
                    [InlineKeyboardButton.WithCallbackData(SurveyAnswerCatalog.ContactStyles["easy"], "social-style:easy")],
                    [InlineKeyboardButton.WithCallbackData(SurveyAnswerCatalog.ContactStyles["need-time"], "social-style:need-time")],
                    [InlineKeyboardButton.WithCallbackData(SurveyAnswerCatalog.ContactStyles["shy"], "social-style:shy")]
            ]),
            cancellationToken);

        return true;
    }

    internal async Task<bool> HandleSocialProfileAsync(
        CallbackQuery callbackQuery,
        string data,
        SurveyState state,
        long chatId,
        CancellationToken cancellationToken)
    {
        if (data.StartsWith("social-style:", StringComparison.Ordinal))
        {
            state.ContactStyle = data["social-style:".Length..];
            await SaveStateAsync(chatId, state, cancellationToken);
            await AnswerAndSendAsync(
                callbackQuery,
                chatId,
                "Стиль знакомства сохранён.",
                "Размер компании:",
                new InlineKeyboardMarkup(
                [
                    [InlineKeyboardButton.WithCallbackData(SurveyAnswerCatalog.GroupSizes["one-two"], "social-size:one-two")],
                    [InlineKeyboardButton.WithCallbackData(SurveyAnswerCatalog.GroupSizes["three-five"], "social-size:three-five")],
                    [InlineKeyboardButton.WithCallbackData(SurveyAnswerCatalog.GroupSizes["large"], "social-size:large")],
                    [InlineKeyboardButton.WithCallbackData(SurveyAnswerCatalog.GroupSizes["unknown"], "social-size:unknown")]
                ]),
                cancellationToken);
            return true;
        }

        if (data.StartsWith("social-size:", StringComparison.Ordinal))
        {
            state.GroupSize = data["social-size:".Length..];
            await SaveStateAsync(chatId, state, cancellationToken);
            await AnswerAndSendAsync(
                callbackQuery,
                chatId,
                "Размер компании сохранён.",
                "Регулярность походов на мероприятия:",
                new InlineKeyboardMarkup(
                [
                    [InlineKeyboardButton.WithCallbackData(SurveyAnswerCatalog.VisitFrequencies["weekly"], "social-frequency:weekly")],
                    [InlineKeyboardButton.WithCallbackData(SurveyAnswerCatalog.VisitFrequencies["two-three-weekly"], "social-frequency:two-three-weekly")],
                    [InlineKeyboardButton.WithCallbackData(SurveyAnswerCatalog.VisitFrequencies["monthly"], "social-frequency:monthly")],
                    [InlineKeyboardButton.WithCallbackData(SurveyAnswerCatalog.VisitFrequencies["by-mood"], "social-frequency:by-mood")]
                ]),
                cancellationToken);
            return true;
        }

        if (data.StartsWith("social-frequency:", StringComparison.Ordinal))
        {
            state.VisitFrequency = data["social-frequency:".Length..];
            await SaveStateAsync(chatId, state, cancellationToken);
            await AnswerAndSendAsync(
                callbackQuery,
                chatId,
                "Регулярность сохранена.",
                "Удобное время:",
                new InlineKeyboardMarkup(
                [
                    [InlineKeyboardButton.WithCallbackData(SurveyAnswerCatalog.PreferredTimes["weekday-day"], "social-time:weekday-day")],
                    [InlineKeyboardButton.WithCallbackData(SurveyAnswerCatalog.PreferredTimes["weekday-evening"], "social-time:weekday-evening")],
                    [InlineKeyboardButton.WithCallbackData(SurveyAnswerCatalog.PreferredTimes["weekend-day"], "social-time:weekend-day")],
                    [InlineKeyboardButton.WithCallbackData(SurveyAnswerCatalog.PreferredTimes["weekend-evening"], "social-time:weekend-evening")]
                ]),
                cancellationToken);
            return true;
        }

        if (data.StartsWith("social-time:", StringComparison.Ordinal))
        {
            state.PreferredTime = data["social-time:".Length..];
            state.CurrentStep = SurveyStep.SocialProfile;
            await SaveStateAsync(chatId, state, cancellationToken);
            await AnswerAndSendAsync(
                callbackQuery,
                chatId,
                "Удобное время сохранено.",
                "Логистика и бюджет:\n\nБюджет на одно мероприятие:",
                new InlineKeyboardMarkup(
                [
                    [InlineKeyboardButton.WithCallbackData(SurveyAnswerCatalog.Budgets["free"], "budget:free")],
                    [InlineKeyboardButton.WithCallbackData(SurveyAnswerCatalog.Budgets["up-to-500"], "budget:up-to-500")],
                    [InlineKeyboardButton.WithCallbackData(SurveyAnswerCatalog.Budgets["500-1500"], "budget:500-1500")],
                    [InlineKeyboardButton.WithCallbackData(SurveyAnswerCatalog.Budgets["1500-3000"], "budget:1500-3000")],
                    [InlineKeyboardButton.WithCallbackData(SurveyAnswerCatalog.Budgets["up-to-5000"], "budget:up-to-5000")],
                    [InlineKeyboardButton.WithCallbackData(SurveyAnswerCatalog.Budgets["unlimited"], "budget:unlimited")]
                ]),
                cancellationToken: cancellationToken);
            return true;
        }

        return false;
    }

    internal async Task<bool> HandleBudgetAsync(
        CallbackQuery callbackQuery,
        string data,
        SurveyState state,
        long chatId,
        CancellationToken cancellationToken)
    {
        const string prefix = "budget:";

        if (!data.StartsWith(prefix, StringComparison.Ordinal))
        {
            return false;
        }

        state.Budget = data[prefix.Length..];
        state.CurrentStep = SurveyStep.Wishes;
        await SaveStateAsync(chatId, state, cancellationToken);

        await AnswerAndSendAsync(
            callbackQuery,
            chatId,
            "Бюджет сохранён.",
            "Особые пожелания:\n\nНапишите ваши пожелания одним сообщением или отправьте «нет».",
                new InlineKeyboardMarkup(
                [
                    [InlineKeyboardButton.WithCallbackData(
                        "Нет",
                        "special-wishes:no")]
                ]),
            cancellationToken: cancellationToken);

        return true;
    }

    private async Task AnswerAndSendAsync(
        CallbackQuery callbackQuery,
        long chatId,
        string callbackText,
        string messageText,
        InlineKeyboardMarkup? replyMarkup,
        CancellationToken cancellationToken)
    {
        await BotClient.AnswerCallbackQuery(
            callbackQuery.Id,
            callbackText,
            cancellationToken: cancellationToken);

        await SendMessageAsync(
            chatId,
            messageText,
            replyMarkup,
            cancellationToken);
    }

    internal async Task<bool> HandleCourseAsync(
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

        await SendMessageAsync(
            chatId,
            "Направление: выберите вариант или напишите своё.",
            cancellationToken);

        foreach (var group in StudyDirectionCatalog.Groups)
        {
            var keyboard = new InlineKeyboardMarkup(
                group.Directions.Select(direction =>
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(
                            direction.DisplayName,
                            $"direction:{direction.Key}")
                    }));

            await SendMessageAsync(
                chatId,
                group.Name,
                keyboard,
                cancellationToken);
        }

        return true;
    }

    internal async Task<bool> HandleConsentAsync(
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
                "Ссылка на ник в Telegram:\n\n" +
                "Введите username или @username. Можно пропустить.",
                new InlineKeyboardMarkup(
                [
                    [InlineKeyboardButton.WithCallbackData(
                        "Пропустить",
                        "telegram-nick:skip")]
                ]),
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

    internal async Task<bool> HandleSpecialWishesAsync(
        CallbackQuery callbackQuery,
        string data,
        SurveyState state,
        long chatId,
        CancellationToken cancellationToken)
    {
        if (!data.Equals("special-wishes:no", StringComparison.Ordinal))
        {
            return false;
        }

        state.SpecialWishes = null;
        state.CurrentStep = SurveyStep.Consent;

        await SaveStateAsync(chatId, state, cancellationToken);

        await BotClient.AnswerCallbackQuery(
            callbackQuery.Id,
            "Пожелания пропущены.",
            cancellationToken: cancellationToken);

        await SendMessageAsync(
            chatId,
            SurveySummaryFormatter.Build(state),
            cancellationToken);

        await SendMessageAsync(
            chatId,
            "Согласие на обработку персональных данных:",
            new InlineKeyboardMarkup(
            [
                [InlineKeyboardButton.WithCallbackData("Согласен", "consent:yes")],
                [InlineKeyboardButton.WithCallbackData("Не согласен", "consent:no")]
            ]),
            cancellationToken);

        return true;
    }

    internal async Task<bool> HandleTelegramNickSkipAsync(
        CallbackQuery callbackQuery,
        string data,
        SurveyState state,
        long chatId,
        CancellationToken cancellationToken)
    {
        if (!data.Equals("telegram-nick:skip", StringComparison.Ordinal))
        {
            return false;
        }

        state.ContactTelegramNick = null;
        state.CurrentStep = SurveyStep.Thanks;

        await SaveStateAsync(chatId, state, cancellationToken);
        await CompleteRegistrationAsync(state, cancellationToken);

        await BotClient.AnswerCallbackQuery(
            callbackQuery.Id,
            "Шаг пропущен.",
            cancellationToken: cancellationToken);

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