using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StudentSurveyBot.Bot.Abstractions;
using StudentSurveyBot.Bot.Handlers;
using Telegram.Bot;

namespace StudentSurveyBot.Bot;

public static class BotServiceCollectionExtensions
{
    public static IServiceCollection AddBotServices(this IServiceCollection services)
    {
        services.AddSingleton<IUserStateStore, InMemoryUserStateStore>();
        services.AddSingleton<IChatRequestSynchronizer, ChatRequestSynchronizer>();
        services.AddSingleton<UpdateHandler>();
        services.AddSingleton<IUpdateStrategy, StartCommandStrategy>();
        services.AddSingleton<IUpdateStrategy, AdminCommandStrategy>();
        services.AddSingleton<IUpdateStrategy, MatchingCommandStrategy>();
        services.AddSingleton<IUpdateStrategy, SurveyCallbackStrategy>();
        services.AddSingleton<IUpdateStrategy, SurveyMessageStrategy>();
        services.AddSingleton<ExceptionHandlingMiddleware>();
        services.AddSingleton<StartCommandHandler>();
        services.AddSingleton<AdminHandler>();
        services.AddSingleton<MatchingHandler>();
        services.AddSingleton<BotOptions>();
        services.AddSingleton<SurveyTextHandler>();
        services.AddSingleton<SurveyCallbackHandler>();
        return services;
    }
}
    