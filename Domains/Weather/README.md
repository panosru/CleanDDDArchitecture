# Weather Domain

The Weather domain is a **minimal DDD reference example** — the simplest possible domain showing how to wire up Aviant's building blocks end-to-end. It is the best starting point for understanding the template.

## Patterns Used

- **DDD** — Aggregate, Value Objects, Repository
- **CQRS** — commands and queries via MediatR
- **API Versioning** — demonstrates v1.0 and v1.1 side-by-side using namespace conventions

## Key Use Cases

| Use Case | Version | Description |
|---|---|---|
| Get Weather Forecasts | v1.0 | Returns a list of forecasts |
| Get Weather Forecasts | v1.1 | Enhanced response with additional metadata |

## Structure

```
Domains/Weather/
├── Core/               # WeatherForecast entity, IWeatherRepository
├── Application/        # Queries, Handlers, DTOs
├── Infrastructure/     # In-memory / EF Core repository
├── CrossCutting/       # AddWeatherDomain()
└── Hosts/
    └── RestApi/
        └── Presentation/
            ├── V1_0/   # v1.0 controllers
            └── V1_1/   # v1.1 controllers (added metadata)
```

The namespace `V1_0` / `V1_1` convention is picked up automatically by `VersionByNamespaceConvention` from `Asp.Versioning`.

## Running Tests

```bash
# Behaviour / acceptance tests
dotnet test Domains/Weather/Tests/Behaviour/Behaviour.csproj
```

## Adding a New API Version

1. Create a `V1_2/` subdirectory under `Hosts/RestApi/Presentation/`
2. Add controllers in that namespace — versioning is automatic
3. Register `AddOpenApi("v1.2")` in the host's `Program.cs`
