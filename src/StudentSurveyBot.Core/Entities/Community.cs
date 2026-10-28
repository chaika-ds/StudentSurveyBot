namespace StudentSurveyBot.Core.Entities;

public class Community
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TelegramUrl { get; set; } = string.Empty;
    public string[] InterestCodes { get; set; } = Array.Empty<string>();

    public ICollection<CommunityJoinIntent> JoinIntents { get; set; } = new List<CommunityJoinIntent>();
}
