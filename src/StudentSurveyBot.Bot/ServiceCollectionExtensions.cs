using System.Collections.Frozen;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StudentSurveyBot.Bot.Abstractions;
using StudentSurveyBot.Bot.Handlers;
using StudentSurveyBot.Bot.Handlers.Survey;
using StudentSurveyBot.Bot.Middleware;
using StudentSurveyBot.Bot.States;
using StudentSurveyBot.Bot.FrozenDictionaries;
using StudentSurveyBot.Bot.Handlers.Survey.FrozenDictionaries;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace StudentSurveyBot.Bot;

public static class BotServiceCollectionExtensions
{
    public static IServiceCollection AddBotServices(this IServiceCollection services)
    {
        services.AddSingleton<IUserStateStore, InMemoryUserStateStore>();
        services.AddSingleton<IChatRequestSynchronizer, ChatRequestSynchronizer>();
        services.AddSingleton<UpdateHandler>();
        services.AddSingleton(_ =>
            UpdateStrategyDictionary.Create(
                _.GetRequiredService<IEnumerable<IUpdateStrategy>>()));
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

        services.AddSingleton<
            FrozenDictionary<
                SurveyStep,
                Func<SurveyTextHandler, Message, SurveyState, long, CancellationToken, Task<bool>>[]>>(
            _ => SurveyTextStepHandlers.Create());
        return services;
    }
}
