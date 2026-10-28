namespace StudentSurveyBot.Core.Entities;

public class AdditionalInfo
{
    public Guid StudentId { get; set; }
    public string? SpecialWishes { get; set; }
    public string? ContactTelegramNick { get; set; }

    public Student Student { get; set; } = null!;
}
