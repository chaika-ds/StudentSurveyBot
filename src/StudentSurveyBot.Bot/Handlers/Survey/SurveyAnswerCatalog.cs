namespace StudentSurveyBot.Bot.Handlers.Survey;

internal static class SurveyAnswerCatalog
{
    public static IReadOnlyDictionary<string, string> DesiredVibes { get; } =
        new Dictionary<string, string>
        {
            ["meet-people"] = "Познакомиться с людьми",
            ["impressions"] = "Получить впечатления",
            ["discuss"] = "Пообщаться и обсудить интересы",
            ["just-go-out"] = "Просто провести время"
        };

    public static IReadOnlyDictionary<string, string> ContactStyles { get; } =
        new Dictionary<string, string>
        {
            ["easy"] = "Легко знакомлюсь",
            ["need-time"] = "Мне нужно время",
            ["shy"] = "Стесняюсь, но хочу знакомиться"
        };

    public static IReadOnlyDictionary<string, string> GroupSizes { get; } =
        new Dictionary<string, string>
        {
            ["one-two"] = "1–2 человека",
            ["three-five"] = "3–5 человек",
            ["large"] = "Большая компания",
            ["unknown"] = "Не имеет значения"
        };

    public static IReadOnlyDictionary<string, string> VisitFrequencies { get; } =
        new Dictionary<string, string>
        {
            ["weekly"] = "Каждую неделю",
            ["two-three-weekly"] = "2–3 раза в неделю",
            ["monthly"] = "Раз в месяц",
            ["by-mood"] = "По настроению"
        };

    public static IReadOnlyDictionary<string, string> PreferredTimes { get; } =
        new Dictionary<string, string>
        {
            ["weekday-day"] = "Будни днём",
            ["weekday-evening"] = "Будни вечером",
            ["weekend-day"] = "Выходные днём",
            ["weekend-evening"] = "Выходные вечером"
        };

    public static IReadOnlyDictionary<string, string> Budgets { get; } =
        new Dictionary<string, string>
        {
            ["free"] = "Бесплатно",
            ["up-to-500"] = "До 500 ₽",
            ["500-1500"] = "500–1500 ₽",
            ["1500-3000"] = "1500–3000 ₽",
            ["up-to-5000"] = "До 5000 ₽",
            ["unlimited"] = "Без ограничений"
        };
}
