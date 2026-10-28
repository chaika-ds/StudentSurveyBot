using StudentSurveyBot.Core.Entities;

namespace StudentSurveyBot.Core.Services;

public sealed class StudentService : IStudentService
{
    public Task<Student> GetOrCreateAsync(long tgId, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Use the StudentService implementation registered by StudentSurveyBot.Data.");
    }

    public Task<Student?> GetByTgIdAsync(long tgId, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Use the StudentService implementation registered by StudentSurveyBot.Data.");
    }
}
