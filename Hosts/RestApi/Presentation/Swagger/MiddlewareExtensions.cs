using System.Diagnostics.CodeAnalysis;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.Swagger;

/// <summary>
///     Extending Swagger services
/// </summary>
[ExcludeFromCodeCoverage]
internal static class MiddlewareExtensions
{
    /// <summary>
    ///     Enabling Swagger UI.
    ///     Excluding from test environment
    /// </summary>
    /// <param name="app">IApplicationBuilder</param>
    public static void UseSwaggerDocuments(this IApplicationBuilder app)
    {
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") 
                          ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (environment == "TEST")
            return;

        app.UseSwagger();

        app.UseSwaggerUI();
    }
}
