using Telegram.Bot;
using Telegram.Bot.Types;

namespace StudentSurveyBot.Bot;

public sealed class AdminHandler
{
    private readonly ITelegramBotClient _botClient;

    public AdminHandler(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        await _botClient.SendTextMessage(
            chatId: message.Chat.Id,
            text: "Админский сценарий готов к расширению.",
            cancellationToken: cancellationToken);
    }
}
