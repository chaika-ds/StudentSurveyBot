using StudentSurveyBot.Core.Enums;

namespace StudentSurveyBot.Core.Models;

public sealed record RegistrationData
{
    public required long TelegramId { get; init; }
    public required string FullName { get; init; }
    public required string Direction { get; init; }
    public required string Course { get; init; }
    public required IReadOnlyCollection<int> InterestIds { get; init; }
    public string? TelegramNick { get; init; }
    public string? SpecialWishes { get; init; }
    public DesiredVibe? DesiredVibe { get; init; }
    public ContactStyle? ContactStyle { get; init; }
    public GroupSize? GroupSize { get; init; }
    public VisitFrequency? VisitFrequency { get; init; }
    public PreferredTime PreferredTime { get; init; }
    public Budget? Budget { get; init; }
    public bool ConsentGiven { get; init; }
    public string? ConsentVersion { get; init; }
    public string? ConsentTextHash { get; init; }
    public DateTimeOffset? ConsentGivenAt { get; init; }
}
