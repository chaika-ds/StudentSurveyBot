using Microsoft.EntityFrameworkCore;
using StudentSurveyBot.Core.Entities;
using StudentSurveyBot.Core.Catalogs;

namespace StudentSurveyBot.Data.Seed;

public sealed class InterestSeed
{
    private readonly AppDbContext _dbContext;

    public InterestSeed(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        foreach (var definition in InterestCatalog.Definitions)
        {
            if (!await _dbContext.Interests.AnyAsync(
                    x => x.Code == definition.Code,
                    cancellationToken))
            {
                _dbContext.Interests.Add(new Interest
                {
                    Id = definition.Id,
                    Code = definition.Code,
                    Label = definition.Label
                });
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
