# CLAUDE.md

## Project Overview

Demo codebase for the "All Our Aggregates Are Wrong" talk. Demonstrates distributed microservices with proper aggregate segregation using NServiceBus messaging and ServiceComposer for UI composition.

## Solution Structure

All projects live under `src/`. Solution file: `src/All-our-Aggregates-are-Wrong-Demos.sln`

Four business domains, each following the same layered pattern:

| Domain | Api | Service | Data | Messages | ViewModelComposition |
|--------|-----|---------|------|----------|----------------------|
| Sales | ✓ | ✓ | ✓ | ✓ (+Events) | ✓ |
| Marketing | ✓ | ✓ | ✓ | — | ✓ (+Events) |
| Warehouse | ✓ | ✓ | ✓ | ✓ | ✓ |
| Shipping | ✓ | ✓ | ✓ | ✓ | ✓ |

Infrastructure projects: `WebApp`, `WebApp.Tests`, `EndToEndTests`, `ITOps.Middlewares`, `ITOps.ViewModelComposition`, `NServiceBus.Shared`, `JsonUtils`, `CreateRequiredDatabases`

## Key Technologies

- **.NET 10** — target framework across all projects
- **NServiceBus 10.1** — messaging, sagas, long-running processes
- **ServiceComposer.AspNetCore 5.1** — distributed UI/ViewModel composition
- **Entity Framework Core + Npgsql 10** — PostgreSQL ORM
- **PostgreSQL** — all service databases (port 7432 in dev)
- **xUnit** — test framework
- **Testcontainers.PostgreSql** — isolated DB containers in tests

## Build & Run

```bash
# Build entire solution
dotnet build src/All-our-Aggregates-are-Wrong-Demos.sln

# Run all tests
dotnet test src/All-our-Aggregates-are-Wrong-Demos.sln

# Create databases (first-time setup)
dotnet run --project src/CreateRequiredDatabases/CreateRequiredDatabases.csproj

# Start individual service
dotnet run --project src/Sales.Service/Sales.Service.csproj
```

## NuGet Package Sources

Custom feeds in `src/nuget.config`:
- `particular packages` — NServiceBus / Particular Software (feedz.io)
- `ServiceComposer packages` — pre-releases (feedz.io)
- `Mauro's pre-releases feed` — owner's pre-releases (feedz.io)

Always run `dotnet restore` from `src/` so nuget.config is picked up.

## Architecture Patterns

### Service Isolation
Each domain owns its API, service host, and database. No cross-service DB access.

### Messaging (NServiceBus)
- Commands/Events defined in `*.Messages` and `*.Messages.Events` projects
- Handlers in `*.Service/Handlers/` implement `IHandleMessages<T>`
- Sagas for long-running workflows
- Learning transport in dev (`.learningtransport/` folder)

### UI Composition (ServiceComposer)
- `*.ViewModelComposition` projects contain composition handlers
- `ITOps.ViewModelComposition` holds cross-cutting composition logic
- `WebApp` renders composed view models — no direct service coupling

### Middleware
Custom ASP.NET Core middleware in `ITOps.Middlewares` (e.g., `ShoppingCartMiddleware`, `TransactionalSessionMiddleware`)

## Testing

- `WebApp.Tests` — integration tests using `WebApplicationFactory`
- `EndToEndTests` — end-to-end tests with real Testcontainers PostgreSQL
- Tests disable parallelization: `[CollectionBehavior(DisableTestParallelization)]`

## Dev Database Config

Connection strings are hard-coded in `DbContext.OnConfiguring()`:
```
Host=localhost;Port=7432;Username=db_user;Password=P@ssw0rd;Database={service}_database
```
Ensure a PostgreSQL instance is running on port 7432 with the above credentials.
