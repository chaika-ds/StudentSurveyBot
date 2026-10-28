namespace StudentSurveyBot.Core.Enums;

public enum StudentCourse
{
    First = 1,
    Second = 2,
    Third = 3,
    Fourth = 4,
    Master = 5,
    Postgraduate = 6
}

public enum DesiredVibe
{
    MeetPeople = 1,
    GetImpressions = 2,
    Discuss = 3,
    JustGoOut = 4
}

public enum ContactStyle
{
    Easy = 1,
    NeedTime = 2,
    ShyButWant = 3
}

public enum GroupSize
{
    OneTwo = 1,
    ThreeFive = 2,
    Large = 3,
    Unknown = 4
}

public enum VisitFrequency
{
    Weekly = 1,
    TwoThreeWeekly = 2,
    Monthly = 3,
    ByMood = 4
}

[Flags]
public enum PreferredTime
{
    None = 0,
    WeekdayDay = 1 << 0,
    WeekdayEvening = 1 << 1,
    WeekendDay = 1 << 2,
    WeekendEvening = 1 << 3
}

public enum Budget
{
    Free = 1,
    UpTo500 = 2,
    From500To1500 = 3,
    From1500To3000 = 4,
    UpTo5000 = 5,
    Unlimited = 6
}
