namespace StudentSurveyBot.Bot;

public interface IChatRequestSynchronizer
{
    Task ExecuteAsync(
        long chatId,
        Func<CancellationToken, Task> action,
        CancellationToken cancellationToken);
}