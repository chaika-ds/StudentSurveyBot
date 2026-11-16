using StudentSurveyBot.Bot.Extensions;
using StudentSurveyBot.Bot.Abstractions;
using StudentSurveyBot.Bot.States;
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

    protected SurveyHandlerBase(
        ITelegramBotClient botClient,
        IUserStateStore userStateStore)
    {
        BotClient = botClient;
        UserStateStore = userStateStore;
    }

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