# Student Survey Bot

Telegram-бот-опросник для студентов академии с сохранением анкет, согласия на обработку персональных данных и подготовкой сценария подбора сообществ.

## Что включено
- .NET 8 + C# 12-ready project structure
- EF Core + PostgreSQL configuration
- Telegram.Bot 22.x polling service
- Serilog logging
- Survey state snapshot in memory
- Core entities and enums
- Sample seed data for interests and communities
- Unit test project
- Docker support

## Быстрый запуск

1. Скопируйте `.env.example` в `.env` и заполните токен бота.
2. Запустите PostgreSQL:
   ```bash
   docker compose up -d postgres
   ```
3. Подготовьте миграции:
   ```bash
   dotnet ef migrations add InitialCreate --project src/StudentSurveyBot.Data/StudentSurveyBot.Data.csproj --startup-project src/StudentSurveyBot.Api/StudentSurveyBot.Api.csproj
   ```
4. Примените миграции:
   ```bash
   dotnet ef database update --project src/StudentSurveyBot.Data/StudentSurveyBot.Data.csproj --startup-project src/StudentSurveyBot.Api/StudentSurveyBot.Api.csproj
   ```
5. Запустите приложение:
   ```bash
   dotnet run --project src/StudentSurveyBot.Api/StudentSurveyBot.Api.csproj
   ```

## Структура решения
- `StudentSurveyBot.Api` — точка входа, DI, конфигурация
- `StudentSurveyBot.Bot` — Telegram polling и FSM
- `StudentSurveyBot.Core` — сущности, валидаторы, сервисы
- `StudentSurveyBot.Data` — EF Core и seed
- `StudentSurveyBot.UnitTests` — Unit tests

## Примечание
Этот проект является рабочей базой и шаблоном для дальнейшей доработки по вашему сценарию опроса, включая 152-ФЗ, подбор сообществ и админские команды.
