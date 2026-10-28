namespace StudentSurveyBot.Core.Entities;

public class Student
{
    public Guid Id { get; set; }
    public long TgId { get; set; }
    public string? TgUsername { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty;
    public string Course { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Consent? Consent { get; set; }
    public StudentPreference? Preference { get; set; }
    public AdditionalInfo? AdditionalInfo { get; set; }
    public ICollection<StudentInterest> StudentInterests { get; set; } = new List<StudentInterest>();
    public ICollection<CommunityJoinIntent> CommunityJoinIntents { get; set; } = new List<CommunityJoinIntent>();
    public SurveySession? SurveySession { get; set; }
}
