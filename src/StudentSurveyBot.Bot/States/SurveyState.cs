namespace StudentSurveyBot.Bot;

public sealed class SurveyState
{
    public long ChatId { get; set; }
    public SurveyStep CurrentStep { get; set; } = SurveyStep.FormApplicant;
    public string? FullName { get; set; }
    public string? Direction { get; set; }
    public string? Course { get; set; }
    public List<int> SelectedInterestIds { get; set; } = new();
    public string? DesiredVibe { get; set; }
    public string? ContactStyle { get; set; }
    public string? GroupSize { get; set; }
    public string? VisitFrequency { get; set; }
    public string? PreferredTime { get; set; }
    public string? Budget { get; set; }
    public string? SpecialWishes { get; set; }
    public bool ConsentGiven { get; set; }
    public string? ConsentVersion { get; set; }
    public string? ConsentTextHash { get; set; }
    public DateTimeOffset? ConsentGivenAt { get; set; }
    public string? ContactTelegramNick { get; set; }
    public bool LookingForCompany { get; set; }
}
