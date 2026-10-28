namespace StudentSurveyBot.Core.Entities;

public class Interest
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public ICollection<StudentInterest> StudentInterests { get; set; } = new List<StudentInterest>();
}
