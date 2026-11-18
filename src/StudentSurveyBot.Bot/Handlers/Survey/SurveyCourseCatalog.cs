using StudentSurveyBot.Core.Enums;

namespace StudentSurveyBot.Bot.Handlers.Survey;

internal static class SurveyCourseCatalog
{
    public static IReadOnlyDictionary<string, string> Labels { get; } =
        Enum.GetValues<StudentCourse>()
            .Where(course => course is >= StudentCourse.First and <= StudentCourse.Fourth)
            .ToDictionary(
                course => ((int)course).ToString(),
                course => $"{(int)course} курс");
}
