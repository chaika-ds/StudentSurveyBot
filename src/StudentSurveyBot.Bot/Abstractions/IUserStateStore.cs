namespace StudentSurveyBot.Bot;

public interface IUserStateStore
{
    Task<SurveyState?> GetAsync(long chatId, CancellationToken cancellationToken = default);
    Task SaveAsync(long chatId, SurveyState state, CancellationToken cancellationToken = default);
    Task DeleteAsync(long chatId, CancellationToken cancellationToken = default);
}
