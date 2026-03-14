using System.Diagnostics.CodeAnalysis;
using Asp.Versioning;
using Asp.Versioning.Conventions;
using Aviant.Infrastructure.CrossCutting;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.Swagger;

/// <summary>
///     Service Collection (IServiceCollection) Extensions for API versioning and OpenAPI.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class SwaggerExtensions
{
    /// <summary>
    ///     Registers API versioning with namespace-based conventions and API Explorer.
    /// </summary>
    public static IServiceCollection AddApiVersionWithExplorer(this IServiceCollection services)
    {
        var settings = DependencyInjectionRegistry.DefaultConfiguration
           .GetSection(nameof(SwaggerSettings));

        services.Configure<SwaggerSettings>(settings);

        services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions                   = true;

                options.DefaultApiVersion = new ApiVersion(
                    settings.GetValue<int>("DefaultApiVersion:Major"),
                    settings.GetValue<int>("DefaultApiVersion:Minor"));

                options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
            })
           .AddMvc(options =>
            {
                options.Conventions.Add(new VersionByNamespaceConvention());
            })
           .AddApiExplorer(options =>
            {
                options.GroupNameFormat           = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        return services;
    }

    /// <summary>
    ///     Registers one OpenAPI document per API version with JWT security transformer.
    /// </summary>
    public static IServiceCollection AddOpenApiOptions(this IServiceCollection services)
    {
        services.AddOpenApi("v1", options =>
            options.AddDocumentTransformer<JwtBearerSecuritySchemeTransformer>());

        services.AddOpenApi("v1.1", options =>
            options.AddDocumentTransformer<JwtBearerSecuritySchemeTransformer>());

        return services;
    }
}
