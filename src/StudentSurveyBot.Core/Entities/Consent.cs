namespace StudentSurveyBot.Core.Entities;

public class Consent
{
    public Guid StudentId { get; set; }
    public DateTime? GivenAt { get; set; }
    public string? Version { get; set; }
    public string? TextHash { get; set; }
    public DateTime? RevokedAt { get; set; }

    public Student Student { get; set; } = null!;
}
