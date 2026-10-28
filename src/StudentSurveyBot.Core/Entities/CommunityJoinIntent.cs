namespace StudentSurveyBot.Core.Entities;

public class CommunityJoinIntent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid StudentId { get; set; }
    public int CommunityId { get; set; }
    public DateTime ClickedAt { get; set; } = DateTime.UtcNow;

    public Student Student { get; set; } = null!;
    public Community Community { get; set; } = null!;
}
