using StudentSurveyBot.Core.Entities;

namespace StudentSurveyBot.Core.Abstractions;

public interface IStudentService
{
    Task<Student> GetOrCreateAsync(long tgId, CancellationToken cancellationToken = default);
    Task<Student?> GetByTgIdAsync(long tgId, CancellationToken cancellationToken = default);
}
