using System.Collections.Frozen;

namespace StudentSurveyBot.Bot.Handlers.Survey.FrozenDictionaries;

internal static class SurveyCallbackKeys
{
    public static FrozenDictionary<string, SurveyCallbackKey> Values { get; } =
        new Dictionary<string, SurveyCallbackKey>
        {
            ["direction"] = SurveyCallbackKey.Direction,
            ["interest"] = SurveyCallbackKey.Interest,
            ["interest-vibe"] = SurveyCallbackKey.InterestVibe,
            ["social-style"] = SurveyCallbackKey.SocialStyle,
            ["social-size"] = SurveyCallbackKey.SocialSize,
            ["social-frequency"] = SurveyCallbackKey.SocialFrequency,
            ["social-time"] = SurveyCallbackKey.SocialTime,
            ["budget"] = SurveyCallbackKey.Budget,
            ["course"] = SurveyCallbackKey.Course,
            ["consent"] = SurveyCallbackKey.Consent,
            ["telegram-nick"] = SurveyCallbackKey.TelegramNick,
            ["special-wishes"] = SurveyCallbackKey.SpecialWishes
        }.ToFrozenDictionary();
}
