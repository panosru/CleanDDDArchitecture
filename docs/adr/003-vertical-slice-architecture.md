# ADR 003: Vertical Slice Architecture

**Status:** Accepted
**Date:** 2024-01-01

## Context

Traditional layered architecture (Controller → Service → Repository) scatters a single feature across many files in separate layer folders. This creates high coupling between unrelated features and makes adding or changing a feature complex.

## Decision

Organise code by **feature/use-case (vertical slice)** rather than by technical layer. Each slice owns its own handler, validator, DTO, and repository interaction. Slices within a domain are grouped into Commands and Queries.

```
Application/
├── Commands/
│   └── CreateWeatherForecast/
│       ├── CreateWeatherForecastCommand.cs
│       ├── CreateWeatherForecastCommandHandler.cs
│       └── CreateWeatherForecastCommandValidator.cs
└── Queries/
    └── GetWeatherForecasts/
        ├── GetWeatherForecastsQuery.cs
        ├── GetWeatherForecastsQueryHandler.cs
        └── WeatherForecastDto.cs
```

## Consequences

**Positive:**
- Adding a new feature means adding a new folder — no existing files need to change
- High cohesion: everything a feature needs is co-located
- Easy to delete a feature without risk of breaking others
- Natural fit for MediatR — each slice maps to one request/handler pair

**Negative:**
- Some duplication across similar slices (e.g., shared validation logic)
- Requires discipline to keep slices truly independent (resist the temptation of shared "services")
- Unfamiliar to developers accustomed to traditional layered architecture

## Alternatives Considered

- **Layered architecture** — familiar but couples unrelated features; harder to reason about blast radius of changes
- **Hexagonal / Ports & Adapters** — more explicit about boundaries but adds more abstraction overhead
