# Copilot Instructions — Telegram Bot (C# / .NET)

## Scope

These instructions apply to the entire `StudentSurveyBot.sln` solution.

## Coding style

## 1. Project overview

Telegram bot written in C# (.NET 8), using the Telegram.Bot library and long polling.
It is deployed to a Linux VPS (Ubuntu 22.04+) as a systemd service.
Deployment is performed only through GitHub Actions on pushes to `main`.
Tokens and secrets must only be supplied through environment variables.

## 2. Fixed technology stack

| Layer | Technology | Restrictions |
|---|---|---|
| Runtime | .NET 8 (LTS) | Do not suggest .NET 9+ without an explicit request |
| Language | C# 12 | `nullable enable`, `implicit usings enable` |
| Telegram | Telegram.Bot (latest stable) | Use only `ITelegramBotClient` and polling |
| Hosting | Generic Host (`Microsoft.Extensions.Hosting`) | Use `BackgroundService` for polling |
| DI | `Microsoft.Extensions.DependencyInjection` | No third-party containers |
| Configuration | `IOptions<T>` and environment variables | `ValidateOnStart()` is required |
| Logging | `Microsoft.Extensions.Logging` | Structured logging; no `Console.WriteLine` |
| Retry | Polly | Only for external calls |
| Tests | xUnit, FluentAssertions, NSubstitute | Moq is acceptable if already used in the project |
| CI/CD | GitHub Actions | `appleboy/scp-action`, `appleboy/ssh-action` |
| Host OS | Ubuntu and systemd | No Docker/Kubernetes |

Adding a dependency is allowed only when:

1. It is actively maintained, with commits within the last six months.
2. It uses an MIT, Apache-2.0, or BSD license.
3. There is no equivalent in the BCL or in an existing dependency.
4. Copilot explicitly states the package name, version, purpose, and alternatives.

New directories require discussion first. Do not scatter code across the repository root.

## 4. Code conventions

### 4.1. General

- One public type per file. The file name must match the type name.
- Files must be no longer than 300 lines. Decompose longer files.
- Methods must be no longer than 40 lines. Decompose longer methods.
- Maximum cyclomatic complexity per method is 10.
- Do not use `#region`.
- Do not leave commented-out code blocks in pull requests.
- Do not add a `TODO` without an issue number: `// TODO(#123): ...`.

### 4.2. Naming

- Classes, methods, and properties use `PascalCase`.
- Private fields use `_camelCase`.
- Local variables and parameters use `camelCase`.
- Interfaces use `I` plus `PascalCase`.
- Asynchronous methods use the `Async` suffix.
- Constants use `PascalCase`, not `SCREAMING_CASE`.
- Identifiers must be in English. Comments, XML documentation, and UI strings must be in Russian.

### 4.3. Asynchrony

- All I/O must use `async`/`await`. `.Result`, `.Wait()`, and `GetAwaiter().GetResult()` are forbidden.
- `CancellationToken` is required as the last parameter of asynchronous methods that may run longer than an instantaneous operation.
- Use `ConfigureAwait(false)` throughout library logic, but not in handlers where a context is required.
- Do not use `Task.Run` for I/O; use it only for CPU-bound work.

### 4.4. Nullable reference types

- `nullable enable` is required in every project.
- The null-forgiving operator `!` is allowed only with a comment explaining why it is safe.
- Public APIs must not accept `null` without an explicit `ArgumentNullException.ThrowIfNull` check.

### 4.5. Resources

- Use `IHttpClientFactory` with named clients for `HttpClient`.
- Dispose `IDisposable` and `IAsyncDisposable` resources with `using` or `await using`.
- Use file streams only inside `using` blocks.

## 5. Architecture rules

### 5.1. Layers

- `Program.cs` is the composition root only: DI, configuration, and hosting. It must not contain business logic.
- `PollingService` receives updates and passes them to `UpdateDispatcher`. It must not contain business logic.
- `UpdateDispatcher` routes updates by type to the appropriate handler.
- `Handlers` contain business logic. They must not know about Telegram transport directly and must use `ITelegramSender`, a wrapper around `ITelegramBotClient`.
- `Middleware` contains cross-cutting logic such as errors, rate limiting, and metrics.

### 5.2. Handlers

- Each command and callback must have a separate class implementing `ICommandHandler` or `ICallbackHandler`.
- Register handlers through DI (`services.AddScoped<ICommandHandler, XHandler>()`) and resolve them by key.
- Handlers must not contain `try`/`catch` blocks solely for logging. Catch exceptions only when they can be handled meaningfully.

### 5.3. Forbidden patterns

- God classes such as an `UpdateHandler` containing all business logic.
- Static stateful services or singletons, except for `ILogger`.
- Cyclic dependencies between projects.
- Service locator usage (`serviceProvider.GetService`) outside the composition root.

## 6. Error handling and resilience

- Top-level middleware catches all exceptions during update processing, logs them with `LogError(ex, ...)`, and sends the user a generic message.
- External Telegram API calls must use Polly:
  - three retries with exponential backoff (1s, 2s, 4s) and jitter;
  - a 10-second timeout;
  - a circuit breaker after five errors within 30 seconds, open for 60 seconds.
- Do not log `OperationCanceledException` as an error during shutdown.
- Respect Telegram API `RetryAfter` values and avoid sending excessive requests.
- `429 Too Many Requests` must be handled.

## 7. Configuration and secrets

- The token must come only from the `TELEGRAM_BOT_TOKEN` environment variable. Never store it in code or `appsettings`.
- All settings must be strongly typed through `IOptions<BotConfiguration>` and validated with `ValidateDataAnnotations()` and `ValidateOnStart()`.
- `.env` must be listed in `.gitignore`. On the server, use `/etc/mybot.env` with mode 600.
- Never log tokens, complete `Update` objects, `Message.Text` content (PII), or unmasked chat IDs.
- Check secrets in CI with `gitleaks` or `trufflehog` when configured.

## 8. Logging and observability

- Use structured logging with named message-template parameters.
- Levels:
  - `Debug` — polling-cycle details; may be disabled in production.
  - `Information` — service start/stop and registered commands.
  - `Warning` — expected failures, retries, rate limits, and invalid input.
  - `Error` — unhandled exceptions and dependency failures.
  - `Critical` — the service cannot start.
- Every error log must include `UpdateId`, a masked `ChatId`, and the update type.
- Propagate correlation through `Activity` or `TraceId`; OpenTelemetry is optional.
- Provide a health check using `BackgroundService` and a heartbeat log every N minutes.
- When metrics are enabled, track processed updates, errors, and Telegram API call latency.

## 9. Security

- Do not store secrets in the repository, logs, or error messages.
- Validate all input received from Telegram: text length, type, and allowed `callback_data` values.
- Validate that `callback_data` is no longer than 64 bytes before sending it.
- Do not use dynamic `eval` or `Process.Start` with user input.
- Use parameterized SQL and safely escaped shell commands.
- Check dependencies with `dotnet list package --vulnerable` when configured in CI.
- HTTPS is required for all external HTTP calls.

## 10. Testing

- Business logic in handlers and services must have at least 70% coverage.
- Coverage of `Program.cs` and the composition root is not required.
- Every command handler must have tests for:
  - the happy path;
  - invalid input;
  - dependency failure using a mock.
- Integration tests against the real Telegram API are forbidden.
- Use `ITelegramBotClient` for mocks.
- Tests must be deterministic: no `Thread.Sleep`, real network calls, or dependencies on time of day or locale.
- Test names must follow `MethodName_Scenario_ExpectedResult`.
- Use one assertion concept per test; multiple `Should()` calls are acceptable for one fact.
- Provide test data through a builder pattern such as `UpdateBuilder`.

## 11. Versioning and commits

- Use SemVer for release tags.
- Use Conventional Commits: `feat:`, `fix:`, `docs:`, `refactor:`, `test:`, `chore:`, `ci:`, `perf:`, `build:`.
- Use a module scope, for example `feat(handlers): add /help command`.
- Use `!` for breaking changes and include `BREAKING CHANGE:` in the body.
- Branches: `main` for stable code, plus `feature/*`, `fix/*`, and `chore/*`.
- Use squash merge for pull requests. The title must be a Conventional Commit.
- One pull request should contain one logical task. Keep the diff below 400 lines except for refactoring.

## 12. Documentation

- `README.md` must describe purpose, local setup, environment variables, deployment, and structure.
- Every public class and method must have XML documentation in Russian with at least `<summary>`; complex APIs also require `<param>`, `<returns>`, and `<exception>`.
- When behavior changes, update `README.md` and/or `CHANGELOG.md`.
- Record significant architectural decisions in `/docs/adr/`.

## 13. CI/CD and deployment

- Build and tests run on every push to every branch.
- Deployment runs only on pushes to `main`.
- The pipeline in `.github/workflows/` performs:
  1. restore, build, test, and `format --verify-no-changes`;
  2. publish with `-c Release -o ./publish`;
  3. SCP to `/opt/mybot`;
  4. `systemctl restart mybot`;
  5. health check with `systemctl is-active mybot` and a `started` log entry.
- GitHub Actions secrets must be referenced only through `${{ secrets.* }}`. Never echo secrets to logs.
- The target service is `mybot.service` with `Restart=always`, `RestartSec=5`, and `EnvironmentFile=/etc/mybot.env`.
- Keep the previous version in `/opt/mybot.prev` and deploy using an atomic rename.
- Do not run manual `dotnet publish` commands on the server. Do not use `docker-compose`.

## 14. What Copilot should and should not do

### Do

- Follow the structure and layers described in sections 3 and 5.
- Propose unit tests together with new logic.
- Use existing interfaces such as `ICommandHandler` and `ITelegramSender` for new code.
- Add XML documentation for public APIs.
- When proposing a dependency, provide the justification required by section 2.
- Ask a clarifying question instead of guessing when requirements are unclear.

### Do not

- Use `Console.WriteLine`, `Thread.Sleep`, `.Result`, `.Wait()`, or `async void` (there are no event handlers in this project).
- Hard-code tokens, chat IDs, secret paths, or absolute paths.
- Suggest webhooks without an explicit request; this project uses polling.
- Suggest Docker, Kubernetes, or cloud PaaS.
- Suggest obsolete Telegram.Bot APIs such as event-based models or `OnMessage`.
- Put business logic in `Program.cs` beyond the composition root.
- Add `#region`, commented-out code, or `TODO` without an issue number.
- Introduce static stateful singleton services.
- Ignore `CancellationToken`.

## 15. Pre-code checklist

- [ ] The code compiles with `nullable enable` without warnings.
- [ ] All applicable asynchronous methods accept `CancellationToken`.
- [ ] No forbidden constructs from section 14 are present.
- [ ] Logging is structured and contains no secrets or PII.
- [ ] External calls have timeout and retry handling.
- [ ] Unit tests cover new business logic.
- [ ] Public APIs have XML documentation.
- [ ] Configuration uses `IOptions` rather than hard-coded values.
- [ ] No dependency was added without justification.
- [ ] The diff follows the Conventional Commit scope.

## 16. Useful commands

```bash
dotnet build
dotnet run --project src/StudentSurveyBot.Api
dotnet test
dotnet format
dotnet format --verify-no-changes
dotnet publish src/StudentSurveyBot.Api -c Release -o ./publish
dotnet list package --vulnerable
sudo systemctl status mybot
sudo journalctl -u mybot -f