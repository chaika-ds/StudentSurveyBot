using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Requests;

namespace StudentSurveyBot.Bot.Handlers;

public sealed class AdminHandler
{
    private readonly ITelegramBotClient _botClient;

    public AdminHandler(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        await _botClient.SendRequest(
            new SendMessageRequest
            {
                ChatId = message.Chat.Id,
                Text = "Админский сценарий готов к расширению."
            },
            cancellationToken);
    }
}
