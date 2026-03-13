# CleanDDDArchitecture Template

[![Build And Test](https://github.com/panosru/CleanDDDArchitecture/actions/workflows/dotnetcore.yml/badge.svg)](https://github.com/panosru/CleanDDDArchitecture/actions/workflows/dotnetcore.yml)
[![CodeQL](https://github.com/panosru/CleanDDDArchitecture/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/panosru/CleanDDDArchitecture/actions/workflows/codeql-analysis.yml)

A reference template demonstrating how to build production-quality .NET 10 applications using the [Aviant Library](https://github.com/panosru/Aviant) — a framework for DDD, CQRS, and Event Sourcing.

## Architecture Overview

```
CleanDDDArchitecture/
├── Domains/
│   ├── Account/      # Event Sourcing + Identity (user auth, registration)
│   ├── Todo/         # Subdomain pattern (TodoItem + TodoList subdomains)
│   ├── Weather/      # Simple DDD reference domain
│   └── Shared/       # Cross-cutting domain concerns
├── Hosts/
│   ├── AppHost/      # .NET Aspire orchestration (dev inner loop)
│   ├── RestApi/      # Monolith mode: all domains in one process
│   ├── Services/     # Microservices mode: one process per domain
│   │   ├── AccountService/
│   │   ├── TodoService/
│   │   └── WeatherService/
│   ├── WebApp/       # Blazor WebApp
│   ├── Worker/       # Background worker
│   ├── Gateway/      # YARP reverse proxy
│   └── ServiceDefaults/ # Shared host infrastructure
└── Library/
    └── Aviant/       # Git submodule — the framework itself
```

Each domain follows the same structure:
- **Core** — Entities, Value Objects, Domain Events, Repository interfaces
- **Application** — Commands/Queries (CQRS via MediatR)
- **Infrastructure** — EF Core + PostgreSQL
- **CrossCutting** — DI registration
- **Hosts/RestApi/Presentation** — Versioned controllers

## Domains

| Domain | Pattern | Key concepts |
|---|---|---|
| **Account** | Event Sourcing + Identity | JWT auth, registration, password reset |
| **Todo** | DDD with Subdomains | TodoList → TodoItem hierarchy, subdomain isolation |
| **Weather** | Simple DDD | Minimal example, good starting point |
| **Shared** | Cross-cutting | Base types shared across all domains |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/products/docker-desktop)
- Make (optional, for convenience targets)

## Getting Started

### Option 1: .NET Aspire (recommended for development)

Aspire orchestrates all infrastructure (PostgreSQL, Redis, Mailpit) and services automatically:

```bash
git clone --recurse-submodules https://github.com/panosru/CleanDDDArchitecture.git
cd CleanDDDArchitecture
dotnet run --project Hosts/AppHost/AppHost.csproj
```

The Aspire dashboard opens at `http://localhost:15888` and shows all services.

### Option 2: Docker Compose

```bash
git clone --recurse-submodules https://github.com/panosru/CleanDDDArchitecture.git
cd CleanDDDArchitecture

# Start infrastructure (PostgreSQL, Redis, Mailpit)
docker compose --profile core up -d

# Run the monolith REST API
make RestApi
# or: dotnet run --project Hosts/RestApi/Presentation/Presentation.csproj
```

API docs (Scalar UI) available at: `http://localhost:5000/scalar/v1`

### Deployment Modes

| Mode | Command | Description |
|---|---|---|
| Aspire | `dotnet run --project Hosts/AppHost/AppHost.csproj` | Orchestrated dev environment |
| Monolith | `make RestApi` | All domains in one process |
| Microservices | `docker compose --profile microservices up` | Each domain as a separate service |

## Running Tests

```bash
# All tests with coverage report
./scripts/test-and-coverage.sh

# Single project
dotnet test Domains/Weather/Tests/Behaviour/Behaviour.csproj

# Integration tests (require Docker)
dotnet test Domains/Account/Tests/Integration/Integration.csproj
```

## Key Technology Choices

| Concern | Technology |
|---|---|
| Framework | .NET 10 / ASP.NET Core |
| CQRS | MediatR 12 |
| Validation | FluentValidation |
| ORM | Entity Framework Core 10 + Npgsql |
| Background Jobs | Hangfire (PostgreSQL storage) |
| API Versioning | Asp.Versioning.Mvc 8.x |
| API Docs | Microsoft.AspNetCore.OpenApi + Scalar |
| Auth | ASP.NET Core Identity + JWT Bearer |
| Caching | EasyCaching (Redis / in-memory) |
| Messaging | Confluent.Kafka |
| Object Mapping | AutoMapper |
| Dev Orchestration | .NET Aspire 9 |
| Testing | xunit v3 + Testcontainers + FluentAssertions |

## Contribution

Pull requests are welcome. Contributions to documentation, tests, and new domain examples are especially encouraged.

## Support

- [GitHub Issues](https://github.com/panosru/CleanDDDArchitecture/issues)

## License

MIT — see [LICENSE](LICENSE) for details.
