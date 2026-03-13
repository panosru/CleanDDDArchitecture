using System.Diagnostics.CodeAnalysis;
using Scalar.AspNetCore;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.Swagger;

/// <summary>
///     Middleware extensions for OpenAPI / Scalar UI.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class MiddlewareExtensions
{
    /// <summary>
    ///     Maps OpenAPI endpoints and Scalar UI. Skipped in the TEST environment.
    /// </summary>
    public static void UseSwaggerDocuments(this IApplicationBuilder app)
    {
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                          ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (environment == "TEST")
            return;

        // WebApplication implements IEndpointRouteBuilder; the cast is valid at runtime.
        if (app is IEndpointRouteBuilder endpoints)
        {
            endpoints.MapOpenApi();
            endpoints.MapScalarApiReference();
        }
    }
}
