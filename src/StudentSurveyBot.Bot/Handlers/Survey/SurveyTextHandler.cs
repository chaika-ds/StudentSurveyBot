using StudentSurveyBot.Bot.Abstractions;
using StudentSurveyBot.Bot.States;
using FluentValidation;
using System.Collections.Frozen;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace StudentSurveyBot.Bot.Handlers.Survey;

public class SurveyTextHandler : SurveyHandlerBase
{
    private static readonly Regex FullNameRegex = new(
        "^[A-Za-zА-Яа-яЁё\\s-]{2,120}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private readonly IValidator<string?> _nicknameValidator;
    private readonly FrozenDictionary<
        SurveyStep,
        Func<SurveyTextHandler, Message, SurveyState, long, CancellationToken, Task<bool>>[]> _stepHandlers;

    public SurveyTextHandler(
        ITelegramBotClient botClient,
        IUserStateStore userStateStore,
        IValidator<string?> nicknameValidator,
        FrozenDictionary<
            SurveyStep,
            Func<SurveyTextHandler, Message, SurveyState, long, CancellationToken, Task<bool>>[]> stepHandlers)
        : base(botClient, userStateStore)
    {
        _nicknameValidator = nicknameValidator;
        _stepHandlers = stepHandlers;
    }

    internal static FrozenDictionary<
        SurveyStep,
        Func<SurveyTextHandler, Message, SurveyState, long, CancellationToken, Task<bool>>[]>
        CreateStepHandlers()
    {
        return new Dictionary<
            SurveyStep,
            Func<SurveyTextHandler, Message, SurveyState, long, CancellationToken, Task<bool>>[]>
        {
            [SurveyStep.Wishes] =
            [
                static (handler, message, state, chatId, cancellationToken) =>
                    handler.HandleSpecialWishesAsync(
                        message,
                        state,
                        chatId,
                        cancellationToken)
            ],

            [SurveyStep.FormApplicant] =
            [
                static (handler, message, state, chatId, cancellationToken) =>
                    handler.HandleApplicantNameAsync(
                        message,
                        state,
                        chatId,
                        cancellationToken),

                static (handler, message, state, chatId, cancellationToken) =>
                    handler.HandleDirectionAsync(
                        message,
                        state,
                        chatId,
                        cancellationToken)
            ],

            [SurveyStep.TelegramNick] =
            [
                static (handler, message, state, chatId, cancellationToken) =>
                    handler.HandleTelegramNickAsync(
                        message,
                        state,
                        chatId,
                        cancellationToken)
            ]
        }.ToFrozenDictionary();
    }

    public async Task HandleAsync(
        Message message,
        CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var state = await GetStateAsync(chatId, cancellationToken);

        if (_stepHandlers.TryGetValue(state.CurrentStep, out var handlers))
        {
            foreach (var handler in handlers)
            {
                if (await handler(this, message, state, chatId, cancellationToken))
                {
                    return;
                }
            }
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
            !string.IsNullOrWhiteSpace(state.FullName))
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
            "Выберите курс:",
            new InlineKeyboardMarkup(
            [
                [InlineKeyboardButton.WithCallbackData("1 курс", "course:1")],
                [InlineKeyboardButton.WithCallbackData("2 курс", "course:2")],
                [InlineKeyboardButton.WithCallbackData("3 курс", "course:3")],
                [InlineKeyboardButton.WithCallbackData("4 курс", "course:4")]
            ]),
            cancellationToken);

        return true;
    }

    private async Task<bool> HandleSpecialWishesAsync(
        Message message,
        SurveyState state,
        long chatId,
        CancellationToken cancellationToken)
    {
        if (state.CurrentStep != SurveyStep.Wishes)
        {
            return false;
        }

        var wishes = message.Text?.Trim();

        if (string.IsNullOrWhiteSpace(wishes))
        {
            await SendMessageAsync(
                chatId,
                "Пожалуйста, напишите пожелания или отправьте «нет».",
                cancellationToken);

            return true;
        }

        if (wishes.Equals("нет", StringComparison.OrdinalIgnoreCase))
        {
            state.SpecialWishes = null;
        }
        else if (wishes.Length > 500)
        {
            await SendMessageAsync(
                chatId,
                "Особые пожелания не должны превышать 500 символов.",
                cancellationToken);

            return true;
        }
        else
        {
            state.SpecialWishes = wishes;
        }

        await SaveStateAsync(chatId, state, cancellationToken);

        state.CurrentStep = SurveyStep.Consent;
        await SaveStateAsync(chatId, state, cancellationToken);

        await SendMessageAsync(
            chatId,
            SurveySummaryFormatter.Build(state),
            cancellationToken);

        await SendMessageAsync(
            chatId,
            "Особые пожелания сохранены.\n\n" +
            "Согласие на обработку персональных данных:",
            new InlineKeyboardMarkup(
            [
                [InlineKeyboardButton.WithCallbackData("Согласен", "consent:yes")],
                [InlineKeyboardButton.WithCallbackData("Не согласен", "consent:no")]
            ]),
            cancellationToken);

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

            var validationResult = await _nicknameValidator.ValidateAsync(
                nick,
                cancellationToken);

            if (!validationResult.IsValid)
            {
                await SendMessageAsync(
                    chatId,
                    validationResult.Errors[0].ErrorMessage,
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
