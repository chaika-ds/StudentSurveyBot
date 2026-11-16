namespace StudentSurveyBot.Bot.Handlers.Survey;

public sealed record StudyDirection(string Key, string Code, string Name)
{
    public string DisplayName => $"{Code} {Name}";
}

public sealed record StudyDirectionGroup(string Name, IReadOnlyList<StudyDirection> Directions);

public static class StudyDirectionCatalog
{
    public static IReadOnlyList<StudyDirectionGroup> Groups { get; } =
    [
        new(
            "Экономика, менеджмент и бизнес",
            [
                new("economics", "38.03.01", "Экономика"),
                new("management", "38.03.02", "Менеджмент"),
                new("hr-management", "38.03.03", "Управление персоналом"),
                new("business-informatics", "38.03.05", "Бизнес-информатика"),
                new("commerce", "38.03.06", "Торговое дело"),
                new("innovation", "27.03.05", "Инноватика")
            ]),
        new(
            "Государственное управление, политика и международные отношения",
            [
                new("public-administration", "38.03.04", "Государственное и муниципальное управление"),
                new("political-science", "41.03.04", "Политология"),
                new("international-relations", "41.03.05", "Международные отношения"),
                new("foreign-regional-studies", "41.03.01", "Зарубежное регионоведение"),
                new("russian-regional-studies", "41.03.02", "Регионоведение России"),
                new("public-policy", "41.03.06", "Публичная политика и социальные науки")
            ]),
        new(
            "Право и безопасность",
            [
                new("law", "40.03.01", "Юриспруденция"),
                new("economic-security", "38.03.05", "Экономическая безопасность")
            ]),
        new(
            "IT и точные науки",
            [
                new("applied-mathematics", "01.03.02", "Прикладная математика и информатика"),
                new("information-systems", "09.03.02", "Информационные системы и технологии"),
                new("applied-informatics", "09.03.03", "Прикладная информатика")
            ]),
        new(
            "Социальные и гуманитарные науки",
            [
                new("psychology", "37.03.01", "Психология"),
                new("sociology", "39.03.01", "Социология"),
                new("history", "46.03.01", "История"),
                new("arts-and-humanities", "50.03.01", "Искусства и гуманитарные науки"),
                new("advertising", "42.03.01", "Реклама и связи с общественностью"),
                new("journalism", "42.03.02", "Журналистика"),
                new("media-communications", "42.03.05", "Медиакоммуникации")
            ]),
        new(
            "Сервис, туризм и другие",
            [
                new("tourism", "43.03.02", "Туризм"),
                new("hospitality", "43.03.03", "Гостиничное дело"),
                new("service", "43.03.01", "Сервис"),
                new("design", "54.03.01", "Дизайн")
            ])
    ];

    public static StudyDirection? Find(string key) =>
        Groups.SelectMany(group => group.Directions)
            .FirstOrDefault(direction => direction.Key.Equals(key, StringComparison.Ordinal));
}
