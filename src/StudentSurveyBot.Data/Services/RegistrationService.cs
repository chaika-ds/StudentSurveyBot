using Microsoft.EntityFrameworkCore;
using StudentSurveyBot.Core.Abstractions;
using StudentSurveyBot.Core.Entities;
using StudentSurveyBot.Core.Models;

namespace StudentSurveyBot.Data.Services;

public sealed class RegistrationService : IRegistrationService
{
    private readonly AppDbContext _dbContext;

    public RegistrationService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CompleteAsync(
        RegistrationData registration,
        CancellationToken cancellationToken = default)
    {
        var student = await _dbContext.Students
            .Include(x => x.Consent)
            .Include(x => x.Preference)
            .Include(x => x.AdditionalInfo)
            .Include(x => x.StudentInterests)
            .SingleOrDefaultAsync(
                x => x.TgId == registration.TelegramId,
                cancellationToken);

        if (student is null)
        {
            student = new Student
            {
                Id = Guid.NewGuid(),
                TgId = registration.TelegramId,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.Students.Add(student);
        }

        student.FullName = registration.FullName;
        student.Direction = registration.Direction;
        student.Course = registration.Course;
        student.TgUsername = registration.TelegramNick;
        student.UpdatedAt = DateTime.UtcNow;

        if (registration.DesiredVibe.HasValue ||
            registration.ContactStyle.HasValue ||
            registration.GroupSize.HasValue ||
            registration.VisitFrequency.HasValue ||
            registration.Budget.HasValue)
        {
            student.Preference ??= new StudentPreference { StudentId = student.Id };
            student.Preference.DesiredVibe = registration.DesiredVibe ?? default;
            student.Preference.ContactStyle = registration.ContactStyle ?? default;
            student.Preference.GroupSize = registration.GroupSize ?? default;
            student.Preference.VisitFrequency = registration.VisitFrequency ?? default;
            student.Preference.PreferredTime = registration.PreferredTime;
            student.Preference.Budget = registration.Budget ?? default;
        }

        student.AdditionalInfo ??= new AdditionalInfo { StudentId = student.Id };
        student.AdditionalInfo.SpecialWishes = registration.SpecialWishes;
        student.AdditionalInfo.ContactTelegramNick = registration.TelegramNick;

        if (registration.ConsentGiven)
        {
            student.Consent ??= new Consent { StudentId = student.Id };
            student.Consent.GivenAt = registration.ConsentGivenAt?.UtcDateTime ?? DateTime.UtcNow;
            student.Consent.Version = registration.ConsentVersion;
            student.Consent.TextHash = registration.ConsentTextHash;
            student.Consent.RevokedAt = null;
        }

        _dbContext.StudentInterests.RemoveRange(student.StudentInterests);
        student.StudentInterests = [.. registration.InterestIds
            .Distinct()
            .Select(interestId => new StudentInterest
            {
                StudentId = student.Id,
                InterestId = interestId
            })];

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
