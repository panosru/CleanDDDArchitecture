# ADR 004: Microsoft.AspNetCore.OpenApi + Scalar over Swashbuckle

**Status:** Accepted
**Date:** 2025-01-01

## Context

The project previously used `Swashbuckle.AspNetCore` (5.x) for API documentation. Swashbuckle is no longer actively maintained for .NET 9+, and Microsoft has shipped a built-in OpenAPI implementation in `Microsoft.AspNetCore.OpenApi` as part of .NET 9/10.

Additionally, `Asp.Versioning.Mvc` (the community successor to `Microsoft.AspNetCore.Mvc.Versioning`) dropped compatibility with Swashbuckle in favour of the built-in OpenAPI.

## Decision

Replace:
- `Swashbuckle.AspNetCore.*` → `Microsoft.AspNetCore.OpenApi`
- Swagger UI → `Scalar.AspNetCore` (Scalar UI)
- Custom `IOperationFilter` / `IConfigureOptions<SwaggerGenOptions>` → `IOpenApiDocumentTransformer`

## Consequences

**Positive:**
- No third-party dependency for core API documentation
- First-class support in .NET 10 with active Microsoft maintenance
- `IOpenApiDocumentTransformer` API is simpler and more testable
- Scalar UI is modern, well-maintained, and feature-rich

**Negative:**
- `IOpenApiDocumentTransformer` API is less mature than Swashbuckle's `IOperationFilter` ecosystem
- Some Swashbuckle-specific annotations (`[SwaggerOperation]`, `[SwaggerResponse]`) are not available
- Teams familiar with Swagger UI need to learn the Scalar UI layout

## Alternatives Considered

- **NSwag** — actively maintained alternative to Swashbuckle; rejected because Microsoft's built-in solution is now preferred for .NET 10
- **Stay on Swashbuckle** — no longer viable for .NET 10 without unofficial forks
