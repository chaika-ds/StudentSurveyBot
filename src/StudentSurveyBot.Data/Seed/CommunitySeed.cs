using Microsoft.EntityFrameworkCore;
using StudentSurveyBot.Core.Entities;

namespace StudentSurveyBot.Data;

public sealed class CommunitySeed
{
    private readonly AppDbContext _dbContext;

    public CommunitySeed(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var communities = new[]
        {
            new Community
            {
                Name = "Бизнес-клуб Президентской академии",
                Description = "События для построения контактов и деловых знакомств.",
                TelegramUrl = "https://t.me/business_club_pa",
                InterestCodes = new[] { "conferences", "project_activity", "excursions" }
            },
            new Community
            {
                Name = "Спорт. Президентская академия",
                Description = "Спортивные мероприятия и активный отдых.",
                TelegramUrl = "https://t.me/sport_pa",
                InterestCodes = new[] { "sports", "nature_active" }
            },
            new Community
            {
                Name = "ЦПР «Просто Будущее»",
                Description = "Проектная деятельность и события для молодых инициатив.",
                TelegramUrl = "https://t.me/prosto_budushee",
                InterestCodes = new[] { "project_activity", "conferences", "cinema" }
            },
            new Community
            {
                Name = "Внутри_Академии",
                Description = "Культурные и образовательные события внутри академии.",
                TelegramUrl = "https://t.me/vnutri_akademii",
                InterestCodes = new[] { "theater", "concerts", "exhibitions_museums", "excursions" }
            },
            new Community
            {
                Name = "Институт управления РАНХиГС",
                Description = "Дискуссии, экспертные встречи и общественные проекты.",
                TelegramUrl = "https://t.me/ranhigs",
                InterestCodes = new[] { "conferences", "project_activity", "excursions" }
            },
            new Community
            {
                Name = "СЕНАТОР | Президентская академия",
                Description = "Культурная и событийная среда для общения.",
                TelegramUrl = "https://t.me/senator_pa",
                InterestCodes = new[] { "theater", "concerts", "cinema", "exhibitions_museums" }
            }
        };

        foreach (var community in communities)
        {
            if (!await _dbContext.Communities.AnyAsync(x => x.Name == community.Name, cancellationToken))
            {
                _dbContext.Communities.Add(community);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
