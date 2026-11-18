using StudentSurveyBot.Core.Catalogs;

namespace StudentSurveyBot.Bot.Handlers.Survey;

internal static class SurveyInterestCatalog
{
    public static IReadOnlyDictionary<int, string> Labels { get; } =
        InterestCatalog.Definitions.ToDictionary(
            definition => definition.Id,
            definition => definition.Label);
}
