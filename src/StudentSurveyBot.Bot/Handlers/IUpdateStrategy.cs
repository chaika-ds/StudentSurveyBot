using Telegram.Bot.Types;

namespace StudentSurveyBot.Bot.Handlers;

public interface IUpdateStrategy
{
    IReadOnlyCollection<string> Keys { get; }

    bool CanHandle(Update update);

    Task HandleAsync(Update update, CancellationToken cancellationToken);
}

public static class UpdateStrategyKeys
{
    public const string Start = "command:start";
    public const string Export = "command:export";
    public const string Stats = "command:stats";
    public const string Match = "command:match";
    public const string Callback = "callback";
    public const string Message = "message";
}
