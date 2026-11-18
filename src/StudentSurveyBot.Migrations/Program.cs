using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentSurveyBot.Data;
using StudentSurveyBot.Data.Seed;

using var loggerFactory = LoggerFactory.Create(logging => logging.AddConsole());
var logger = loggerFactory.CreateLogger("StudentSurveyBot.Migrations");

try
{
    var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
        ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
        ?? "Development";

    var configuration = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: true)
        .AddJsonFile($"appsettings.{environment}.json", optional: true)
        .AddEnvironmentVariables()
        .Build();

    var connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? args.FirstOrDefault(argument => !argument.StartsWith("--"));

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException(
            "Строка подключения 'ConnectionStrings:DefaultConnection' не найдена " +
            "в appsettings или аргументах запуска.");
    }

    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseNpgsql(
            connectionString,
            npgsqlOptions => npgsqlOptions.MigrationsAssembly(
                typeof(Program).Assembly.GetName().Name))
        .Options;

    await using var dbContext = new AppDbContext(options);

    logger.LogInformation("Применение миграций PostgreSQL...");
    await dbContext.Database.MigrateAsync();
    logger.LogInformation("Миграции успешно применены.");

    logger.LogInformation("Заполнение справочников...");
    await new InterestSeed(dbContext).SeedAsync();
    await new CommunitySeed(dbContext).SeedAsync();
    logger.LogInformation("Справочники успешно заполнены.");
}
catch (Exception exception)
{
    logger.LogError(exception, "Ошибка при применении миграций PostgreSQL.");
    Environment.ExitCode = 1;
}
finally
{
    logger.LogInformation("Процесс применения миграций завершён.");
}

