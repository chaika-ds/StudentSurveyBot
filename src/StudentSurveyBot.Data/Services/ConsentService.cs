using Microsoft.EntityFrameworkCore;
using StudentSurveyBot.Core.Entities;
using StudentSurveyBot.Core.Services;

namespace StudentSurveyBot.Data.Services;

public sealed class ConsentService : IConsentService
{
    private readonly AppDbContext _dbContext;

    public ConsentService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task GiveConsentAsync(long tgId, string version, string consentTextHash, CancellationToken cancellationToken = default)
    {
        var student = await _dbContext.Students
            .Include(x => x.Consent)
            .FirstOrDefaultAsync(x => x.TgId == tgId, cancellationToken);

        if (student is null)
        {
            throw new InvalidOperationException($"Student with tgId={tgId} not found.");
        }

        if (student.Consent is null)
        {
            student.Consent = new Consent
            {
                StudentId = student.Id,
                GivenAt = DateTime.UtcNow,
                Version = version,
                TextHash = consentTextHash
            };
        }
        else
        {
            student.Consent.GivenAt = DateTime.UtcNow;
            student.Consent.Version = version;
            student.Consent.TextHash = consentTextHash;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeclineAsync(long tgId, CancellationToken cancellationToken = default)
    {
        _dbContext.ConsentAudits.Add(new ConsentAudit
        {
            TgId = tgId,
            At = DateTime.UtcNow,
            Action = "Declined"
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeAsync(long tgId, CancellationToken cancellationToken = default)
    {
        var student = await _dbContext.Students
            .Include(x => x.Consent)
            .FirstOrDefaultAsync(x => x.TgId == tgId, cancellationToken);

        if (student is null)
        {
            return;
        }

        if (student.Consent is not null)
        {
            student.Consent.RevokedAt = DateTime.UtcNow;
        }

        _dbContext.ConsentAudits.Add(new ConsentAudit
        {
            TgId = tgId,
            At = DateTime.UtcNow,
            Action = "Revoked"
        });

        _dbContext.Students.Remove(student);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
