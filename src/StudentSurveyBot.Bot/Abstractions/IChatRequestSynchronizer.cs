namespace StudentSurveyBot.Bot.Abstractions;

public interface IChatRequestSynchronizer
{
    Task ExecuteAsync(
        long chatId,
        Func<CancellationToken, Task> action,
        CancellationToken cancellationToken);
}
