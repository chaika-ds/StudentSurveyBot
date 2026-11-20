using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StudentSurveyBot.Api.Configuration;
using StudentSurveyBot.Bot;
using StudentSurveyBot.Core;
using StudentSurveyBot.Core.Entities;
using StudentSurveyBot.Core.Services;
using StudentSurveyBot.Data;
using Telegram.Bot; 

using var fallbackLoggerFactory = LoggerFactory.Create(logging => logging.AddConsole());
var fallbackLogger = fallbackLoggerFactory.CreateLogger("StudentSurveyBot.Api");
ILogger? applicationLogger = null;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();

    SerilogConfiguration.Configure(builder);

    builder.Services.Configure<BotOptions>(builder.Configuration.GetSection("Bot"));

    builder.Services.AddSingleton<ITelegramBotClient>(_ =>
        new TelegramBotClient(builder.Configuration["Bot:Token"] ?? throw new InvalidOperationException("Bot:Token is not configured.")));

    builder.Services.AddCoreServices();
    builder.Services.AddDataServices(builder.Configuration);
    builder.Services.AddBotServices();

    builder.Services.AddValidatorsFromAssembly(
        typeof(StudentSurveyBot.Core.Validators.FullNameValidator).Assembly,
        ServiceLifetime.Singleton);

    builder.Services.AddHostedService<TelegramPollingService>();

    var app = builder.Build();
    applicationLogger = app.Logger;

//using (var scope = app.Services.CreateScope())
//{
//    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    await dbContext.Database.MigrateAsync();

//    var interestSeed = scope.ServiceProvider.GetRequiredService<InterestSeed>();
//    await interestSeed.SeedAsync();

//    var communitySeed = scope.ServiceProvider.GetRequiredService<CommunitySeed>();
//    await communitySeed.SeedAsync();
//}

    app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

    await app.RunAsync();
}
catch (Exception exception)
{
    (applicationLogger ?? fallbackLogger).LogError(
        exception,
        "Ошибка при запуске приложения.");
    Environment.ExitCode = 1;
}
finally
{
    (applicationLogger ?? fallbackLogger).LogInformation(
        "Процесс приложения завершён.");
}
