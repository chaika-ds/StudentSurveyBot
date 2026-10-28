using Microsoft.Extensions.DependencyInjection;
using StudentSurveyBot.Core.Services;

namespace StudentSurveyBot.Core;

public static class CoreServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IConsentService, ConsentService>();
        return services;
    }
}
