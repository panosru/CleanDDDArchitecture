# CLAUDE.md — Developer Guide for AI Assistants

This file provides context for AI assistants (Claude Code, Copilot, etc.) working in this repository.

## Two-Repo Relationship

This repo is the **CleanDDDArchitecture** template — a demo/reference application.
It depends on the **Aviant Library** (a reusable DDD/CQRS/Event Sourcing framework) which lives as a git submodule at `Library/Aviant/`.

```
CleanDDDArchitecture/      ← this repo
└── Library/
    └── Aviant/            ← git submodule (github.com/panosru/Aviant)
```

Both projects are included in a single solution: `CleanDDDArchitecture.sln`.

**Important:** When making changes to Aviant source files (`Library/Aviant/`), remember that the submodule has its own git history. Changes there must be committed separately in the submodule and then the parent repo updated.

## Prerequisites

- **.NET 10 SDK** (all projects target `net10.0`)
- **Docker** (for PostgreSQL, Redis, Kafka, Mailpit)
- **Make** (for convenience targets)

## Build Commands

```bash
# Build the entire solution
dotnet build CleanDDDArchitecture.sln

# Build Aviant library only
dotnet build Library/Aviant/Aviant.sln

# Build in Release mode
dotnet build CleanDDDArchitecture.sln --configuration Release
```

## Run Commands

```bash
# Start infrastructure services (PostgreSQL, Redis, Mailpit)
docker compose --profile core up -d

# Run the monolith REST API host
make RestApi
# or: dotnet run --project Hosts/RestApi/Presentation/Presentation.csproj

# Run the Blazor WebApp
make WebApp

# Run the background Worker
make Worker

# Run with .NET Aspire (orchestrates all services + infrastructure)
dotnet run --project Hosts/AppHost/AppHost.csproj
```

## Test Commands

```bash
# Run all tests with coverage
./scripts/test-and-coverage.sh

# Run a specific domain's tests
dotnet test Domains/Account/Tests/Unit/Unit.csproj
dotnet test Domains/Account/Tests/Integration/Integration.csproj  # requires Docker
dotnet test Domains/Weather/Tests/Behaviour/Behaviour.csproj

# Integration tests use Testcontainers — Docker must be running
```

## Docker Startup Profiles

The `docker/compose.yaml` uses Docker Compose profiles:

| Profile | Services |
|---|---|
| `core` | PostgreSQL, Redis, Mailpit |
| `monolith` | core + single RestApi container |
| `microservices` | core + AccountService, TodoService, WeatherService |
| `eventing` | Zookeeper + Kafka |

```bash
docker compose --profile core up -d
docker compose --profile eventing up -d   # if using Kafka
```

## Package Management

**Central Package Management** is enabled via `Directory.Packages.props` at the repo root.

- **Do not** add `Version="..."` to `<PackageReference>` in `.csproj` files — versions are controlled centrally.
- To add a new package: add a `<PackageVersion>` entry to `Directory.Packages.props`, then reference it without a version in the relevant `.csproj`.
- The Aviant library has its own `Directory.Packages.props` at `Library/Aviant/Directory.Packages.props`.

## Architecture Patterns

### Vertical Slice Architecture

Each domain is self-contained. Code is organised by feature/use-case, not by layer:
```
Domains/
├── Account/           ← Event Sourcing + Identity
├── Todo/              ← Subdomains (TodoItem, TodoList)
├── Weather/           ← Simple DDD domain (reference example)
└── Shared/            ← Cross-cutting domain concerns
```

### Domain Structure (per domain)

```
Domains/<Domain>/
├── Core/              ← Entities, Value Objects, Domain Events, Repository interfaces
├── Application/       ← Commands, Queries, Handlers (CQRS via MediatR)
├── Infrastructure/    ← EF Core DbContext, Repository implementations
├── CrossCutting/      ← DI registration (AddXxxDomain())
└── Hosts/
    └── RestApi/
        └── Presentation/   ← Controllers (versioned via Asp.Versioning)
```

### CQRS / MediatR Pipeline

Commands and Queries flow through a MediatR pipeline with these behaviours (in order):
1. `RequestPreProcessorBehavior` → Pre-processors
2. `PerformanceBehaviour` → Log slow requests
3. `ValidationBehaviour` → FluentValidation
4. `UnhandledExceptionBehaviour` → Log unhandled exceptions
5. `RequestPostProcessorBehavior` → Post-processors
6. `RequestExceptionActionProcessorBehavior` / `RequestExceptionProcessorBehavior` → Exception handlers

Handlers are decorated with `RetryRequestProcessor` for automatic retries.

### API Versioning

Uses `Asp.Versioning.Mvc` 8.x (replacement for the deprecated `Microsoft.AspNetCore.Mvc.Versioning`).

- Controllers in `V1_0/` namespace → version 1.0
- Controllers in `V1_1/` namespace → version 1.1
- Versioning convention: `VersionByNamespaceConvention`
- Version header: `x-api-version`

### OpenAPI / API Documentation

Uses `Microsoft.AspNetCore.OpenApi` + `Scalar.AspNetCore` (replaced Swashbuckle).

- Documents registered: `v1`, `v1.1`
- JWT Bearer security added via `JwtBearerSecuritySchemeTransformer`
- Scalar UI available at `/scalar/v1` and `/scalar/v1.1`

### Authentication

- **AccountService**: Uses ASP.NET Core Identity + custom JWT issuance (cookie-based session internally)
- **WeatherService / TodoService**: JWT Bearer token validation against AccountService-issued tokens
- **RestApi monolith**: Combines both — issues JWTs and validates them

## How to Add a New Domain

1. Create `Domains/<NewDomain>/Core/`, `Application/`, `Infrastructure/`, `CrossCutting/` projects
2. Add a `Add<NewDomain>Domain()` extension method in `CrossCutting/`
3. Create `Domains/<NewDomain>/Hosts/RestApi/Presentation/` with versioned controllers
4. Register `Add<NewDomain>Domain()` in the appropriate `Program.cs` host(s)
5. Add health checks in `CrossCutting/` and register via `AddHealthChecks().Add<NewDomain>Checks()`
6. Add AutoMapper profiles and FluentValidation validators in `CrossCutting/`
7. Add test projects: `Tests/Unit/`, `Tests/Integration/`, `Tests/Behaviour/`
8. Add all test projects to `scripts/test-and-coverage.sh` `TEST_PROJECTS` array

## Deployment Modes

| Mode | Entry Point | Description |
|---|---|---|
| Monolith | `Hosts/RestApi/Presentation/` | All domains in one process |
| Microservices | `Hosts/Services/*/Presentation/` | Each domain as an independent service |
| Aspire | `Hosts/AppHost/` | Orchestrates all services via .NET Aspire |

## CI/CD

- **GitHub Actions**: `.github/workflows/dotnetcore.yml` (Build And Test)
- **CodeQL**: `.github/workflows/codeql-analysis.yml`
- Integration tests use Testcontainers and require Docker in CI (supported on `ubuntu-latest`)
