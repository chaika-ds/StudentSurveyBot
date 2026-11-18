namespace StudentSurveyBot.Core.Catalogs;

public sealed record InterestDefinition(int Id, string Code, string Label);

public static class InterestCatalog
{
    public static IReadOnlyCollection<InterestDefinition> Definitions { get; } =
    [
        new(1, "theater", "Театр"),
        new(2, "exhibitions_museums", "Выставки и музеи"),
        new(3, "cinema", "Кино"),
        new(4, "concerts", "Концерты"),
        new(5, "excursions", "Экскурсии и городские прогулки"),
        new(6, "nature_active", "Природа и активный отдых"),
        new(7, "conferences", "Конференции"),
        new(8, "sports", "Спортивные мероприятия"),
        new(9, "project_activity", "Проектная деятельность")
    ];
}
