using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace StudentSurveyBot.Data;

public static class DataServiceCollectionExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<InterestSeed>();
        services.AddScoped<CommunitySeed>();
        return services;
    }
}
