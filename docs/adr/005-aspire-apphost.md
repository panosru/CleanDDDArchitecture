# ADR 005: .NET Aspire AppHost for Development Orchestration

**Status:** Accepted
**Date:** 2025-01-01

## Context

The project supports two deployment modes: monolith (single `Hosts/RestApi/` process) and microservices (separate service per domain). Both modes require several infrastructure services: PostgreSQL, Redis, Mailpit (SMTP), and optionally Kafka.

Previously, developers had to:
1. Run `docker compose --profile core up -d` manually
2. Start each service process separately or use `make RestApi`
3. Track connection strings across multiple `appsettings.yaml` files

This friction slowed the inner development loop and made it easy to forget a step.

## Decision

Add a **.NET Aspire AppHost** at `Hosts/AppHost/AppHost.csproj` that orchestrates all infrastructure and application services.

Running `dotnet run --project Hosts/AppHost/AppHost.csproj` starts:
- PostgreSQL (with per-service databases)
- Redis
- Mailpit
- All three microservices (AccountService, TodoService, WeatherService)
- The monolith RestApi

The Aspire dashboard at `http://localhost:15888` shows health, logs, and traces for all services.

## Consequences

**Positive:**
- Single command to start the entire stack during development
- Aspire injects connection strings automatically — no manual `appsettings.yaml` edits needed
- Integrated distributed tracing via OpenTelemetry
- `docker/compose.yaml` is retained for production/CI environments without Aspire

**Negative:**
- Aspire requires .NET Aspire workload to be installed (`dotnet workload install aspire`)
- Running all services simultaneously is resource-intensive on developer machines
- Both Aspire (dev) and Docker Compose (prod) configurations must be kept in sync

## Alternatives Considered

- **Tye** (Microsoft.Tye) — predecessor to Aspire, now deprecated
- **Docker Compose only** — sufficient but no integrated dashboard, tracing, or health UI
- **Makefile targets** — simple but requires manual management of each service
