namespace StudentSurveyBot.Bot.Handlers.Survey;

internal static class SurveyInterestCatalog
{
    public static IReadOnlyDictionary<int, string> Labels { get; } =
        new Dictionary<int, string>
        {
            [1] = "Театр",
            [2] = "Выставки и музеи",
            [3] = "Кино",
            [4] = "Концерты",
            [5] = "Экскурсии и городские прогулки",
            [6] = "Природа и активный отдых",
            [7] = "Конференции",
            [8] = "Спортивные мероприятия",
            [9] = "Проектная деятельность"
        };
}
