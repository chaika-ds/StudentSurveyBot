using System.Collections.Concurrent;

namespace StudentSurveyBot.Bot;

using StudentSurveyBot.Bot.Abstractions;

public class ChatRequestSynchronizer : IChatRequestSynchronizer
{
    private readonly ConcurrentDictionary<long, SemaphoreSlim> _semaphores = new();

    public async Task ExecuteAsync(
        long chatId,
        Func<CancellationToken, Task> action,
        CancellationToken cancellationToken)
    {
        var semaphore = _semaphores.GetOrAdd(
            chatId,
            static _ => new SemaphoreSlim(1, 1));

        await semaphore.WaitAsync(cancellationToken);

        try
        {
            await action(cancellationToken);
        }
        finally
        {
            semaphore.Release();
        }
    }
}