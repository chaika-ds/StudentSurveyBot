using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StudentSurveyBot.Bot;
using StudentSurveyBot.Core.Services;
using StudentSurveyBot.Data;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();

builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console();

    if (!string.IsNullOrWhiteSpace(context.Configuration["Seq:Url"]))
    {
        loggerConfiguration.WriteTo.Seq(context.Configuration["Seq:Url"]!);
    }

    loggerConfiguration.WriteTo.File(
        path: Path.Combine(AppContext.BaseDirectory, "logs", "student-bot-.log"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7);
});

builder.Services.Configure<BotOptions>(builder.Configuration.GetSection("Bot"));

builder.Services.AddSingleton<ITelegramBotClient>(_ =>
    new TelegramBotClient(builder.Configuration["Bot:Token"] ?? throw new InvalidOperationException("Bot:Token is not configured.")));

builder.Services.AddCoreServices();
builder.Services.AddDataServices(builder.Configuration);
builder.Services.AddBotServices();

builder.Services.AddValidatorsFromAssembly(typeof(StudentSurveyBot.Core.Validators.FullNameValidator).Assembly);

builder.Services.AddHostedService<TelegramPollingService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();

    var interestSeed = scope.ServiceProvider.GetRequiredService<InterestSeed>();
    await interestSeed.SeedAsync();

    var communitySeed = scope.ServiceProvider.GetRequiredService<CommunitySeed>();
    await communitySeed.SeedAsync();
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

await app.RunAsync();
