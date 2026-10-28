using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace StudentSurveyBot.Bot;

public static class BotServiceCollectionExtensions
{
    public static IServiceCollection AddBotServices(this IServiceCollection services)
    {
        services.AddSingleton<IUserStateStore, InMemoryUserStateStore>();
        services.AddSingleton<UpdateHandler>();
        services.AddSingleton<StartCommandHandler>();
        services.AddSingleton<SurveyHandler>();
        services.AddSingleton<AdminHandler>();
        services.AddSingleton<MatchingHandler>();
        services.AddSingleton<BotOptions>();
        return services;
    }
}
