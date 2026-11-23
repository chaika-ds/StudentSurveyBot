using System.Collections.Frozen;
using StudentSurveyBot.Bot.Abstractions;

namespace StudentSurveyBot.Bot.Handlers.Survey.FrozenDictionaries;

internal static class SurveyCallbackStrategies
{
    public static FrozenDictionary<SurveyCallbackKey, ISurveyCallbackStrategy> Create(
        SurveyCallbackHandler handler)
    {
        return new Dictionary<SurveyCallbackKey, ISurveyCallbackStrategy>
        {
            [SurveyCallbackKey.Direction] = new SurveyCallbackHandlerStrategy(handler.HandleDirectionAsync),
            [SurveyCallbackKey.Interest] = new SurveyCallbackHandlerStrategy(handler.HandleInterestSelectionAsync),
            [SurveyCallbackKey.InterestVibe] = new SurveyCallbackHandlerStrategy(handler.HandleInterestFormAsync),
            [SurveyCallbackKey.SocialStyle] = new SurveyCallbackHandlerStrategy(handler.HandleSocialProfileAsync),
            [SurveyCallbackKey.SocialSize] = new SurveyCallbackHandlerStrategy(handler.HandleSocialProfileAsync),
            [SurveyCallbackKey.SocialFrequency] = new SurveyCallbackHandlerStrategy(handler.HandleSocialProfileAsync),
            [SurveyCallbackKey.SocialTime] = new SurveyCallbackHandlerStrategy(handler.HandleSocialProfileAsync),
            [SurveyCallbackKey.Budget] = new SurveyCallbackHandlerStrategy(handler.HandleBudgetAsync),
            [SurveyCallbackKey.Course] = new SurveyCallbackHandlerStrategy(handler.HandleCourseAsync),
            [SurveyCallbackKey.Consent] = new SurveyCallbackHandlerStrategy(handler.HandleConsentAsync),
            [SurveyCallbackKey.TelegramNick] = new SurveyCallbackHandlerStrategy(handler.HandleTelegramNickSkipAsync),
            [SurveyCallbackKey.SpecialWishes] = new SurveyCallbackHandlerStrategy(handler.HandleSpecialWishesAsync),
            [SurveyCallbackKey.NextStep] = new SurveyCallbackHandlerStrategy(handler.HandleNextStepAsync)
        }.ToFrozenDictionary();
    }
}
