using StudentSurveyBot.Core.Enums;

namespace StudentSurveyBot.Core.Entities;

public class StudentPreference
{
    public Guid StudentId { get; set; }
    public DesiredVibe DesiredVibe { get; set; }
    public ContactStyle ContactStyle { get; set; }
    public GroupSize GroupSize { get; set; }
    public VisitFrequency VisitFrequency { get; set; }
    public PreferredTime PreferredTime { get; set; }
    public Budget Budget { get; set; }

    public Student Student { get; set; } = null!;
}
