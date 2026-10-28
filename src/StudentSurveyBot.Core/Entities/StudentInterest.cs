namespace StudentSurveyBot.Core.Entities;

public class StudentInterest
{
    public Guid StudentId { get; set; }
    public int InterestId { get; set; }

    public Student Student { get; set; } = null!;
    public Interest Interest { get; set; } = null!;
}
