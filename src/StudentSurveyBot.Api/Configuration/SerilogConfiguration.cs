using Serilog;
using StudentSurveyBot.Api.Configuration;

namespace StudentSurveyBot.Api.Configuration;

public static class SerilogConfiguration
{
    public static void Configure(WebApplicationBuilder builder)
    {
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
                loggerConfiguration.WriteTo.Seq(
                    context.Configuration["Seq:Url"]!);
            }

            loggerConfiguration.WriteTo.File(
                path: Path.Combine(
                    AppContext.BaseDirectory,
                    "logs",
                    "student-bot-.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7);
        });
    }
}