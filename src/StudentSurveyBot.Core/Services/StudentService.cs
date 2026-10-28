using Microsoft.EntityFrameworkCore;
using StudentSurveyBot.Core.Entities;

namespace StudentSurveyBot.Core.Services;

public sealed class StudentService : IStudentService
{
    private readonly StudentSurveyBot.Data.AppDbContext _dbContext;

    public StudentService(StudentSurveyBot.Data.AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Student> GetOrCreateAsync(long tgId, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.Students
            .FirstOrDefaultAsync(x => x.TgId == tgId, cancellationToken);

        if (existing is not null)
        {
            return existing;
        }

        var student = new Student
        {
            Id = Guid.NewGuid(),
            TgId = tgId,
            FullName = string.Empty,
            Direction = string.Empty,
            Course = string.Empty,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Students.Add(student);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return student;
    }

    public Task<Student?> GetByTgIdAsync(long tgId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Students
            .FirstOrDefaultAsync(x => x.TgId == tgId, cancellationToken);
    }
}
