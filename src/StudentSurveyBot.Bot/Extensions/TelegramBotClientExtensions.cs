using Telegram.Bot;
using Telegram.Bot.Requests;

namespace StudentSurveyBot.Bot.Extensions;

public static class TelegramBotClientExtensions
{
    public static Task SendMessageAsync(
        this ITelegramBotClient botClient,
        long chatId,
        string text,
        CancellationToken cancellationToken = default)
    {
        return botClient.SendRequest(
            new SendMessageRequest
            {
                ChatId = chatId,
                Text = text
            },
            cancellationToken);
    }
}
