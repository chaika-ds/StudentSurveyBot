using StudentSurveyBot.Core.Models;

namespace StudentSurveyBot.Core.Abstractions;

public interface IRegistrationService
{
    Task CompleteAsync(
        RegistrationData registration,
        CancellationToken cancellationToken = default);
}
