using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StudentSurveyBot.Core.Services;
using StudentSurveyBot.Data.Services;

namespace StudentSurveyBot.Data;

public static class DataServiceCollectionExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IConsentService, ConsentService>();
        services.AddScoped<InterestSeed>();
        services.AddScoped<CommunitySeed>();

        return services;
    }
}
