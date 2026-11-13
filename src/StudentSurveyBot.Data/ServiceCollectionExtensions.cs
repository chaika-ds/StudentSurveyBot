using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StudentSurveyBot.Core.Abstractions;
using StudentSurveyBot.Core.Services;
using StudentSurveyBot.Data.Seed;
using DataConsentService = StudentSurveyBot.Data.Services.ConsentService;

namespace StudentSurveyBot.Data;

public static class DataServiceCollectionExtensions
{
    public static IServiceCollection AddDataServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not found.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsqlOptions =>
                    npgsqlOptions.MigrationsAssembly(
                        "StudentSurveyBot.Migrations")));

        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IConsentService, DataConsentService>();
        services.AddScoped<InterestSeed>();
        services.AddScoped<CommunitySeed>();

        return services;
    }
}
