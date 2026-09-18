# ADR 005: .NET Aspire AppHost for Development Orchestration

**Status:** Accepted (revised 2026-09-18)
**Date:** 2025-01-01

## Context

The project supports two deployment shapes: a monolith (`Hosts/RestApi/`) and microservices (one service per domain behind a YARP gateway). Both need PostgreSQL, Kafka, EventStoreDB and an SMTP sink (Mailpit), and every host needs connection strings, JWT settings and telemetry wired consistently.

Starting that by hand (`docker compose`, then each process, then copying connection strings between YAML files) is slow and easy to get wrong.

## Decision

A **.NET Aspire AppHost** (`Hosts/AppHost`) starts the infrastructure and one deployment shape at a time:

```bash
dotnet run --project Hosts/AppHost                          # monolith: RestApi + Worker
dotnet run --project Hosts/AppHost -- --mode microservices  # Account, Todo, Weather services + Gateway
```

- Aspire ships as an MSBuild SDK (`Aspire.AppHost.Sdk`, pinned in `global.json`); no workload is needed.
- Each host receives its settings **under the names it already reads**: `ConnectionStrings:PGSQLConnection`, `:kafka`, `:eventstore`, `EmailSettings:*`. The services contain no Aspire-specific configuration code.
- Every host calls `AddServiceDefaults()` (`Hosts/ServiceDefaults`), which adds OpenTelemetry logs, metrics and traces, `/health` (readiness) and `/alive` (liveness), HTTP resilience and service discovery. Aspire points the OTLP exporter at its dashboard, `http://localhost:15888`.
- Development JWT settings are the same public dev values `docker/compose.yaml` uses. Aspire's generated secrets, such as the Postgres password, are kept in the AppHost's user secrets, so they keep matching the data volume across restarts.

## Consequences

**Positive:**
- One command starts a working stack in either shape, with traces, logs and health for every resource in one dashboard.
- Choosing the shape is a flag, which shows the same domain code running as a monolith or as services.
- `docker/compose.yaml` remains for container-based environments and CI; both reuse the hosts' existing configuration names.

**Negative:**
- Aspire and compose describe the same topology twice and must be kept in sync.
- The microservices shape is resource-hungry on a laptop.

## Alternatives Considered

- **Docker Compose only:** works, but no integrated dashboard, tracing or health view.
- **Starting both shapes at once** (the first version of this AppHost): the monolith and the services then compete for the same Kafka topics and databases.
