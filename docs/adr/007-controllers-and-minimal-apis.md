# ADR 007: Controllers and Minimal APIs side by side

**Status:** Accepted
**Date:** 2026-09-18

## Context

The API is built with MVC controllers organised by use case (`Hosts/RestApi/Presentation/UseCases/V1_0/...`), versioned by namespace with `Asp.Versioning.Mvc`. Current ASP.NET Core guidance and templates lean towards minimal APIs, and readers of a reference architecture want to see how the two relate.

In this architecture, the presentation layer is thin: it turns an HTTP request into a use-case input, runs the use case, and turns the use case's **output port** (`IForecastOutput`, `ICreateAccountOutput`, …) into an HTTP result. Neither style changes the application or domain layers.

## Decision

- **Controllers remain the default** for the existing API. They carry namespace-based versioning, filters, conventions and the `ApiController<TUseCase, TOutput>` base, which implements the output port.
- **Minimal APIs are used for small, self-contained endpoints**, shown by `GET /api/minimal/weather/forecast` (`Domains/Weather/Hosts/RestApi/Presentation/Endpoints/WeatherEndpoints.cs`). It calls the **same** `ForecastUseCase` as the controller at `GET /api/weather`, and implements the output port with a small class that produces typed `Results<...>`.
- An endpoint group is mapped by each host that serves the domain (`app.MapWeatherEndpoints()` in the monolith and in the Weather service).

## Consequences

**Positive:**
- Shows that the use case is independent of the presentation style: switching style touches one file.
- Typed results (`Results<Ok<T>, ProblemHttpResult, NotFound>`) make responses explicit and self-documenting in OpenAPI.

**Negative:**
- Two styles in one codebase need a rule for when to use which: controllers for versioned resource APIs, minimal APIs for small endpoints outside that versioning scheme.
- Minimal endpoints are not versioned by `Asp.Versioning.Mvc`. Versioning them needs `Asp.Versioning.Http`.
