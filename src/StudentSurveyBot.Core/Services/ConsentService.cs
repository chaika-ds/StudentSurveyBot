using StudentSurveyBot.Core.Entities;

namespace StudentSurveyBot.Core.Services;

public sealed class ConsentService : IConsentService
{
    public Task GiveConsentAsync(long tgId, string version, string consentTextHash, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Use the ConsentService implementation registered by StudentSurveyBot.Data.");
    }

    public Task DeclineAsync(long tgId, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Use the ConsentService implementation registered by StudentSurveyBot.Data.");
    }

    public Task RevokeAsync(long tgId, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Use the ConsentService implementation registered by StudentSurveyBot.Data.");
    }
}
