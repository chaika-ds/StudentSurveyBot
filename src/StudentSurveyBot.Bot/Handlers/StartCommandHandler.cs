using StudentSurveyBot.Bot.Abstractions;
using StudentSurveyBot.Bot.States;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace StudentSurveyBot.Bot.Handlers;

public sealed class StartCommandHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly IUserStateStore _userStateStore;

    public StartCommandHandler(ITelegramBotClient botClient, IUserStateStore userStateStore)
    {
        _botClient = botClient;
        _userStateStore = userStateStore;
    }

    public async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var existingState = await _userStateStore.GetAsync(chatId, cancellationToken);

        if (existingState is null)
        {
            var newState = new SurveyState
            {
                ChatId = chatId,
                CurrentStep = SurveyStep.FormApplicant
            };

            await _userStateStore.SaveAsync(chatId, newState, cancellationToken);
        }

        await _botClient.SendRequest(
            new SendMessageRequest
            {
                ChatId = chatId,
                Text = "Привет! Мы начинаем анкету. Введи ФИО (2-120 символов)."
            },
            cancellationToken);
    }
}
