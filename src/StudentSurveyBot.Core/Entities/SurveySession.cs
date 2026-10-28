namespace StudentSurveyBot.Core.Entities;

public class SurveySession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid StudentId { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? FinishedAt { get; set; }
    public string CurrentStep { get; set; } = string.Empty;

    public Student Student { get; set; } = null!;
}
