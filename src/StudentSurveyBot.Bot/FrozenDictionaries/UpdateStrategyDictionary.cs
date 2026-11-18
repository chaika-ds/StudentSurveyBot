using System.Collections.Frozen;
using StudentSurveyBot.Bot.Abstractions;
using StudentSurveyBot.Bot.Handlers;

namespace StudentSurveyBot.Bot.FrozenDictionaries;

internal static class UpdateStrategyDictionary
{
    public static FrozenDictionary<string, IUpdateStrategy> Create(
        IEnumerable<IUpdateStrategy> strategies)
    {
        return strategies
            .SelectMany(strategy => strategy.Keys.Select(key => new { key, strategy }))
            .ToFrozenDictionary(item => item.key, item => item.strategy);
    }
}
