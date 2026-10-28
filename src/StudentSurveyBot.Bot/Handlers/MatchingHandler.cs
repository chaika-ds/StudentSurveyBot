using Telegram.Bot;
using Telegram.Bot.Types;

namespace StudentSurveyBot.Bot;

public sealed class MatchingHandler
{
    private readonly ITelegramBotClient _botClient;

    public MatchingHandler(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        await _botClient.SendTextMessage(
            chatId: message.Chat.Id,
            text: "Подбор мероприятий будет реализован в следующих версиях.",
            cancellationToken: cancellationToken);
    }
}
