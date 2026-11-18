using StudentSurveyBot.Bot.Extensions;
using StudentSurveyBot.Bot.Abstractions;
using StudentSurveyBot.Bot.States;
using StudentSurveyBot.Core.Abstractions;
using StudentSurveyBot.Core.Enums;
using StudentSurveyBot.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace StudentSurveyBot.Bot.Handlers.Survey;

public abstract class SurveyHandlerBase
{
    protected const string ConsentVersion = "v1.0";
    protected const string ConsentTextHash = "sha256:placeholder";

    protected readonly ITelegramBotClient BotClient;
    protected readonly IUserStateStore UserStateStore;
    private readonly IServiceScopeFactory _scopeFactory;

    protected SurveyHandlerBase(
        ITelegramBotClient botClient,
        IUserStateStore userStateStore,
        IServiceScopeFactory scopeFactory)
    {
        BotClient = botClient;
        UserStateStore = userStateStore;
        _scopeFactory = scopeFactory;
    }

    protected async Task CompleteRegistrationAsync(
        SurveyState state,
        CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var registrationService = scope.ServiceProvider
            .GetRequiredService<IRegistrationService>();

        await registrationService.CompleteAsync(
            new RegistrationData
            {
                TelegramId = state.ChatId,
                FullName = state.FullName ?? string.Empty,
                Direction = state.Direction ?? string.Empty,
                Course = state.Course ?? string.Empty,
                InterestIds = state.SelectedInterestIds,
                TelegramNick = state.ContactTelegramNick,
                SpecialWishes = state.SpecialWishes,
                DesiredVibe = ParseDesiredVibe(state.DesiredVibe),
                ContactStyle = ParseContactStyle(state.ContactStyle),
                GroupSize = ParseGroupSize(state.GroupSize),
                VisitFrequency = ParseVisitFrequency(state.VisitFrequency),
                PreferredTime = ParsePreferredTime(state.PreferredTime),
                Budget = ParseBudget(state.Budget),
                ConsentGiven = state.ConsentGiven,
                ConsentVersion = state.ConsentVersion,
                ConsentTextHash = state.ConsentTextHash,
                ConsentGivenAt = state.ConsentGivenAt
            },
            cancellationToken);
    }

    private static DesiredVibe? ParseDesiredVibe(string? value) => value switch
    {
        "meet-people" => DesiredVibe.MeetPeople,
        "impressions" => DesiredVibe.GetImpressions,
        "discuss" => DesiredVibe.Discuss,
        "just-go-out" => DesiredVibe.JustGoOut,
        _ => null
    };

    private static ContactStyle? ParseContactStyle(string? value) => value switch
    {
        "easy" => ContactStyle.Easy,
        "need-time" => ContactStyle.NeedTime,
        "shy" => ContactStyle.ShyButWant,
        _ => null
    };

    private static GroupSize? ParseGroupSize(string? value) => value switch
    {
        "one-two" => GroupSize.OneTwo,
        "three-five" => GroupSize.ThreeFive,
        "large" => GroupSize.Large,
        "unknown" => GroupSize.Unknown,
        _ => null
    };

    private static VisitFrequency? ParseVisitFrequency(string? value) => value switch
    {
        "weekly" => VisitFrequency.Weekly,
        "two-three-weekly" => VisitFrequency.TwoThreeWeekly,
        "monthly" => VisitFrequency.Monthly,
        "by-mood" => VisitFrequency.ByMood,
        _ => null
    };

    private static PreferredTime ParsePreferredTime(string? value) => value switch
    {
        "weekday-day" => PreferredTime.WeekdayDay,
        "weekday-evening" => PreferredTime.WeekdayEvening,
        "weekend-day" => PreferredTime.WeekendDay,
        "weekend-evening" => PreferredTime.WeekendEvening,
        _ => PreferredTime.None
    };

    private static Budget? ParseBudget(string? value) => value switch
    {
        "free" => Budget.Free,
        "up-to-500" => Budget.UpTo500,
        "500-1500" => Budget.From500To1500,
        "1500-3000" => Budget.From1500To3000,
        "up-to-5000" => Budget.UpTo5000,
        "unlimited" => Budget.Unlimited,
        _ => null
    };

    protected async Task<SurveyState> GetStateAsync(
        long chatId,
        CancellationToken cancellationToken)
    {
        return await UserStateStore.GetAsync(
                   chatId,
                   cancellationToken)
               ?? new SurveyState
               {
                   ChatId = chatId
               };
    }

    protected Task SaveStateAsync(
        long chatId,
        SurveyState state,
        CancellationToken cancellationToken)
    {
        return UserStateStore.SaveAsync(
            chatId,
            state,
            cancellationToken);
    }

    protected Task SendMessageAsync(
        long chatId,
        string text,
        CancellationToken cancellationToken)
    {
        return BotClient.SendMessageAsync(chatId, text, cancellationToken);
    }

    protected Task SendMessageAsync(
        long chatId,
        string text,
        InlineKeyboardMarkup? replyMarkup,
        CancellationToken cancellationToken)
    {
        return BotClient.SendRequest(
            new SendMessageRequest
            {
                ChatId = chatId,
                Text = text,
                ReplyMarkup = replyMarkup
            },
            cancellationToken);
    }

    protected static long GetChatId(CallbackQuery callbackQuery)
    {
        return callbackQuery.Message?.Chat.Id
               ?? callbackQuery.From.Id;
    }
}