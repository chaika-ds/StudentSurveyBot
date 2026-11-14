using System.Collections.Frozen;
using StudentSurveyBot.Bot.Abstractions;
using StudentSurveyBot.Bot.States;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace StudentSurveyBot.Bot.Handlers;

public class SurveyCallbackHandler : SurveyHandlerBase
{
    private static readonly FrozenDictionary<string, SurveyCallbackKey> CallbackKeys =
        new Dictionary<string, SurveyCallbackKey>
        {
            ["direction"] = SurveyCallbackKey.Direction,
            ["interest"] = SurveyCallbackKey.Interest,
            ["interest-vibe"] = SurveyCallbackKey.InterestVibe,
            ["social-style"] = SurveyCallbackKey.SocialStyle,
            ["social-size"] = SurveyCallbackKey.SocialSize,
            ["social-frequency"] = SurveyCallbackKey.SocialFrequency,
            ["social-time"] = SurveyCallbackKey.SocialTime,
            ["course"] = SurveyCallbackKey.Course,
            ["consent"] = SurveyCallbackKey.Consent
        }.ToFrozenDictionary();

    private readonly FrozenDictionary<SurveyCallbackKey, ISurveyCallbackStrategy> _strategies;

    public SurveyCallbackHandler(
        ITelegramBotClient botClient,
        IUserStateStore userStateStore)
        : base(botClient, userStateStore)
    {
        _strategies = new Dictionary<SurveyCallbackKey, ISurveyCallbackStrategy>
        {
            [SurveyCallbackKey.Direction] = new SurveyCallbackHandlerStrategy(HandleDirectionAsync),
            [SurveyCallbackKey.Interest] = new SurveyCallbackHandlerStrategy(HandleInterestSelectionAsync),
            [SurveyCallbackKey.InterestVibe] = new SurveyCallbackHandlerStrategy(HandleInterestFormAsync),
            [SurveyCallbackKey.SocialStyle] = new SurveyCallbackHandlerStrategy(HandleSocialProfileAsync),
            [SurveyCallbackKey.SocialSize] = new SurveyCallbackHandlerStrategy(HandleSocialProfileAsync),
            [SurveyCallbackKey.SocialFrequency] = new SurveyCallbackHandlerStrategy(HandleSocialProfileAsync),
            [SurveyCallbackKey.SocialTime] = new SurveyCallbackHandlerStrategy(HandleSocialProfileAsync),
            [SurveyCallbackKey.Course] = new SurveyCallbackHandlerStrategy(HandleCourseAsync),
            [SurveyCallbackKey.Consent] = new SurveyCallbackHandlerStrategy(HandleConsentAsync)
        }.ToFrozenDictionary();
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
            !CallbackKeys.TryGetValue(data[..separatorIndex], out var key))
        {
            return SurveyCallbackKey.Unknown;
        }

        return key;
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

    private async Task<bool> HandleInterestSelectionAsync(
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
                    [InlineKeyboardButton.WithCallbackData("Познакомиться с людьми", "interest-vibe:meet-people")],
                    [InlineKeyboardButton.WithCallbackData("Получить впечатления", "interest-vibe:impressions")],
                    [InlineKeyboardButton.WithCallbackData("Пообщаться и обсудить интересы", "interest-vibe:discuss")],
                    [InlineKeyboardButton.WithCallbackData("Просто провести время", "interest-vibe:just-go-out")]
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
        var interests = new[]
        {
            (Id: 1, Label: "Театр"),
            (Id: 2, Label: "Выставки и музеи"),
            (Id: 3, Label: "Кино"),
            (Id: 4, Label: "Концерты"),
            (Id: 5, Label: "Экскурсии и прогулки"),
            (Id: 6, Label: "Природа и активный отдых"),
            (Id: 7, Label: "Конференции"),
            (Id: 8, Label: "Спортивные мероприятия"),
            (Id: 9, Label: "Проектная деятельность")
        };

        var rows = interests
            .Select(interest => new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    $"{(selectedInterestIds.Contains(interest.Id) ? "✅" : "☐")} {interest.Label}",
                    $"interest:{interest.Id}")
            })
            .Append(new[]
            {
                InlineKeyboardButton.WithCallbackData("Готово", "interest:done")
            });

        return new InlineKeyboardMarkup(rows);
    }

    private async Task<bool> HandleInterestFormAsync(
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
                [InlineKeyboardButton.WithCallbackData("Легко знакомлюсь", "social-style:easy")],
                [InlineKeyboardButton.WithCallbackData("Мне нужно время", "social-style:need-time")],
                [InlineKeyboardButton.WithCallbackData("Стесняюсь, но хочу знакомиться", "social-style:shy")]
            ]),
            cancellationToken);

        return true;
    }

    private async Task<bool> HandleSocialProfileAsync(
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
                    [InlineKeyboardButton.WithCallbackData("1–2 человека", "social-size:one-two")],
                    [InlineKeyboardButton.WithCallbackData("3–5 человек", "social-size:three-five")],
                    [InlineKeyboardButton.WithCallbackData("Большая компания", "social-size:large")],
                    [InlineKeyboardButton.WithCallbackData("Не имеет значения", "social-size:unknown")]
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
                    [InlineKeyboardButton.WithCallbackData("Каждую неделю", "social-frequency:weekly")],
                    [InlineKeyboardButton.WithCallbackData("2–3 раза в неделю", "social-frequency:two-three-weekly")],
                    [InlineKeyboardButton.WithCallbackData("Раз в месяц", "social-frequency:monthly")],
                    [InlineKeyboardButton.WithCallbackData("По настроению", "social-frequency:by-mood")]
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
                    [InlineKeyboardButton.WithCallbackData("Будни днём", "social-time:weekday-day")],
                    [InlineKeyboardButton.WithCallbackData("Будни вечером", "social-time:weekday-evening")],
                    [InlineKeyboardButton.WithCallbackData("Выходные днём", "social-time:weekend-day")],
                    [InlineKeyboardButton.WithCallbackData("Выходные вечером", "social-time:weekend-evening")]
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
                "Социальный профиль заполнен.",
                replyMarkup: null,
                cancellationToken: cancellationToken);
            return true;
        }

        return false;
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