using StudentSurveyBot.Bot.States;

namespace StudentSurveyBot.Bot.Handlers.Survey;

internal static class SurveySummaryFormatter
{
    public static string Build(SurveyState state)
    {
        var interests = state.SelectedInterestIds.Count == 0
            ? "не выбраны"
            : string.Join(", ", state.SelectedInterestIds.Select(GetInterestLabel));

        return "Ваши ответы:\n\n" +
               $"ФИО: {GetValue(state.FullName)}\n" +
               $"Курс: {GetCourseLabel(state.Course)}\n" +
               $"Направление: {GetValue(state.Direction)}\n" +
               $"Интересы: {interests}\n" +
               $"Желаемый вайб: {GetDesiredVibeLabel(state.DesiredVibe)}\n" +
               $"Стиль знакомства: {GetContactStyleLabel(state.ContactStyle)}\n" +
               $"Размер компании: {GetGroupSizeLabel(state.GroupSize)}\n" +
               $"Регулярность посещения: {GetVisitFrequencyLabel(state.VisitFrequency)}\n" +
               $"Удобное время: {GetPreferredTimeLabel(state.PreferredTime)}\n" +
               $"Бюджет: {GetBudgetLabel(state.Budget)}\n" +
               $"Особые пожелания: {GetValue(state.SpecialWishes)}";
    }

    private static string GetValue(string? value) =>
        string.IsNullOrWhiteSpace(value) ? "не указано" : value;

    private static string GetCourseLabel(string? course) =>
        course is not null && SurveyCourseCatalog.Labels.TryGetValue(course, out var label)
            ? label
            : "не указан";

    private static string GetInterestLabel(int interestId)
    {
        return SurveyInterestCatalog.Labels.TryGetValue(interestId, out var label)
            ? label
            : $"неизвестный вариант ({interestId})";
    }

    private static string GetDesiredVibeLabel(string? value)
    {
        return GetLabel(SurveyAnswerCatalog.DesiredVibes, value);
    }

    private static string GetContactStyleLabel(string? value)
    {
        return GetLabel(SurveyAnswerCatalog.ContactStyles, value);
    }

    private static string GetGroupSizeLabel(string? value)
    {
        return GetLabel(SurveyAnswerCatalog.GroupSizes, value);
    }

    private static string GetVisitFrequencyLabel(string? value)
    {
        return GetLabel(SurveyAnswerCatalog.VisitFrequencies, value);
    }

    private static string GetPreferredTimeLabel(string? value)
    {
        return GetLabel(SurveyAnswerCatalog.PreferredTimes, value);
    }

    private static string GetBudgetLabel(string? value)
    {
        return GetLabel(SurveyAnswerCatalog.Budgets, value);
    }

    private static string GetLabel(
        IReadOnlyDictionary<string, string> labels,
        string? value) =>
        value is not null && labels.TryGetValue(value, out var label)
            ? label
            : GetValue(value);
}
