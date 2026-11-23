namespace StudentSurveyBot.Bot.Handlers.Survey;

public sealed record StudyDirection(string Key, string Name)
{
    public string DisplayName => Name;
}

public sealed record StudyDirectionGroup(
    string Name,
    IReadOnlyList<StudyDirection> Directions);

public static class StudyDirectionCatalog
{
    public static IReadOnlyList<StudyDirectionGroup> Groups { get; } =
    [
        new(
            "Факультет менеджмента",
            [
                new("project-management", "Управление проектами"),
                new("business-management", "Управление бизнесом"),
                new("production-management", "Производственный менеджмент"),
                new("sports-industry-management", "Менеджмент спортивной индустрии"),
                new("innovation-management", "Инновационный менеджмент"),
                new("production-management-control", "Управление производством"),
                new("sports-management", "Спортивный менеджмент")
            ]),
        new(
            "Факультет международных отношений и бизнеса",
            [
                new("international-business", "Международный бизнес"),
                new("fashion-industry-management", "Менеджмент в индустрии моды"),
                new("strategic-company-management", "Стратегическое управление компанией"),
                new(
                    "global-governance-leadership",
                    "Лидерство и управление в глобальном мире (Global Governance & Leadership)"),
                new(
                    "international-relations-business",
                    "Международные отношения: политика, экономика, бизнес")
            ]),
        new(
            "Факультет маркетинга",
            [
                new("marketing", "Маркетинг"),
                new("digital-business-logistics", "Цифровой бизнес и логистика"),
                new("hotel-restaurant-business", "Гостиничный и ресторанный бизнес"),
                new(
                    "tourism-territory-development",
                    "Индустрия туризма и развитие территорий")
            ]),
        new(
            "Факультет финансов",
            [
                new(
                    "financial-management-investments",
                    "Финансовый менеджмент и инвестиции"),
                new(
                    "entrepreneurship-business-efficiency",
                    "Предпринимательство и управление эффективностью бизнеса"),
                new("banks-financial-markets", "Банки и финансовые рынки"),
                new(
                    "corporate-finance-accounting-audit",
                    "Корпоративные финансы: учет, анализ и аудит"),
                new("finance-credit", "Финансы и кредит")
            ])
    ];

    public static StudyDirection? Find(string key) =>
        Groups.SelectMany(group => group.Directions)
            .FirstOrDefault(direction => direction.Key.Equals(key, StringComparison.Ordinal));
}
