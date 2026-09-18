# Changelog

Notable changes to this reference application. The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [Unreleased]

### Added
- **Integration events with a transactional outbox:** account deletion reaches the Todo context, in-process in the monolith and over Kafka between the services.
- **Architecture tests** (`Tests/Architecture`) enforcing layer and bounded-context rules, and that route values are bound from the route.
- **Value objects** `EmailAddress` and `PersonName`; behaviour methods on the Todo entities; `DomainRuleException` refusals returned as 400s.
- **Service defaults** for every host: OpenTelemetry, `/health` and `/alive`, resilient HTTP clients.
- **Aspire AppHost** with monolith and microservices modes.
- A minimal API version of the weather forecast (ADR 007).
- ADR 006 (dependency licensing) and ADR 007 (controllers and minimal APIs).
- CONTRIBUTING, SECURITY and this changelog.
- A `dotnet new cleanddd` template.

### Changed
- **Aviant 2:** use cases are registered with `AddAviantUseCases` and no longer depend on a static service locator, repositories are async-only and validate entities, and the library logs through `ILogger<T>`.
- **KurrentDB** (formerly EventStoreDB) 26 over gRPC replaces EventStoreDB 21.10 over TCP in Compose, Aspire and the end-to-end tests. The connection string is `ConnectionStrings:kurrentdb`.
- Todo audit times are `DateTimeOffset`, stamped by Aviant's auditing interceptor, and stored as UTC.
- Domain code logs through `ILogger<T>` instead of the static Serilog logger, so the services' logs reach OpenTelemetry. Serilog is configured only in the RestApi host.
- The Account aggregate and its domain events moved from Application to Core.
- One error pipeline (`IExceptionHandler` + RFC 9457 problem details) for every host, replacing three.
- Hosts register the CQRS pipeline with Aviant's `AddAviantCqrs`, which fails startup when a request has no handler.
- Todo list titles are unique per owner and limited to 200 characters.
- MediatR 12.5 (Apache-2.0); AutoMapper removed in favour of explicit mapping; FluentAssertions replaced by AwesomeAssertions.
- Vulnerable packages fail the build; ASP.NET Core and EF Core 10.0.12; Aspire 13.5.

### Fixed
- `Done` was always false in the todo list and CSV export.
- `PUT …/updatedetails/{id}` ignored the route id, which also broke the OpenAPI document for the whole API.
- Startup errors in the API were swallowed and the process exited with code 0; the Worker did the same.
- The Aspire AppHost did not build or start the services it listed.
- Logs written from domain code were lost in the microservices, which do not configure Serilog.
- Security stamps were written to the log when an email change token was rejected.
- Deleted todo lists and items were still returned by queries: Aviant's soft-delete filter was never applied.

### Removed
- Ten empty test projects, the unused EasyCaching packages, and Redis from the AppHost.
