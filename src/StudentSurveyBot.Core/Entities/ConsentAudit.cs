namespace StudentSurveyBot.Core.Entities;

public class ConsentAudit
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public long TgId { get; set; }
    public DateTime At { get; set; } = DateTime.UtcNow;
    public string Action { get; set; } = string.Empty;
    public string? Details { get; set; }
}
