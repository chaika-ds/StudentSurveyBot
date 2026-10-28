namespace StudentSurveyBot.Bot;

public sealed class BotOptions
{
    public string Token { get; set; } = string.Empty;
    public List<long> AdminIds { get; set; } = new();
    public int RateLimitSeconds { get; set; } = 2;
    public string ConsentVersion { get; set; } = "v1.0";
    public string ConsentTextPath { get; set; } = "Resources/consent_text.txt";
}
