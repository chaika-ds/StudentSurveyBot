namespace StudentSurveyBot.Core.Abstractions;

public interface IConsentService
{
    Task GiveConsentAsync(long tgId, string version, string consentTextHash, CancellationToken cancellationToken = default);
    Task DeclineAsync(long tgId, CancellationToken cancellationToken = default);
    Task RevokeAsync(long tgId, CancellationToken cancellationToken = default);
}
