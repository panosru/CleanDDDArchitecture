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

### Changed
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

### Removed
- Ten empty test projects, the unused EasyCaching packages, and Redis from the AppHost.
