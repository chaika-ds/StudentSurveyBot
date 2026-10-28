using System.Collections.Concurrent;

namespace StudentSurveyBot.Bot;

public interface IUserStateStore
{
    Task<SurveyState?> GetAsync(long chatId, CancellationToken cancellationToken = default);
    Task SaveAsync(long chatId, SurveyState state, CancellationToken cancellationToken = default);
    Task DeleteAsync(long chatId, CancellationToken cancellationToken = default);
}

public sealed class InMemoryUserStateStore : IUserStateStore
{
    private readonly ConcurrentDictionary<long, SurveyState> _states = new();

    public Task<SurveyState?> GetAsync(long chatId, CancellationToken cancellationToken = default)
    {
        _states.TryGetValue(chatId, out var state);
        return Task.FromResult(state);
    }

    public Task SaveAsync(long chatId, SurveyState state, CancellationToken cancellationToken = default)
    {
        _states[chatId] = state;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(long chatId, CancellationToken cancellationToken = default)
    {
        _states.TryRemove(chatId, out _);
        return Task.CompletedTask;
    }
}
