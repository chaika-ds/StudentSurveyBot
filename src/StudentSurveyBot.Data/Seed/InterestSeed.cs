using Microsoft.EntityFrameworkCore;
using StudentSurveyBot.Core.Entities;

namespace StudentSurveyBot.Data;

public sealed class InterestSeed
{
    private readonly AppDbContext _dbContext;

    public InterestSeed(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var interests = new[]
        {
            new Interest { Code = "theater", Label = "Театр" },
            new Interest { Code = "exhibitions_museums", Label = "Выставки и музеи" },
            new Interest { Code = "cinema", Label = "Кино" },
            new Interest { Code = "concerts", Label = "Концерты" },
            new Interest { Code = "excursions", Label = "Экскурсии и городские прогулки" },
            new Interest { Code = "nature_active", Label = "Природа и активный отдых" },
            new Interest { Code = "conferences", Label = "Конференции" },
            new Interest { Code = "sports", Label = "Спортивные мероприятия" },
            new Interest { Code = "project_activity", Label = "Проектная деятельность" }
        };

        foreach (var interest in interests)
        {
            if (!await _dbContext.Interests.AnyAsync(x => x.Code == interest.Code, cancellationToken))
            {
                _dbContext.Interests.Add(interest);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
