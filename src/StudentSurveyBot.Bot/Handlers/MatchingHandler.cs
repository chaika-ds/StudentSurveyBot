using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Requests;

namespace StudentSurveyBot.Bot.Handlers;

public sealed class MatchingHandler
{
    private readonly ITelegramBotClient _botClient;

    public MatchingHandler(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        await _botClient.SendRequest(
            new SendMessageRequest
            {
                ChatId = message.Chat.Id,
                Text = "Подбор мероприятий будет реализован в следующих версиях."
            },
            cancellationToken);
    }
}
