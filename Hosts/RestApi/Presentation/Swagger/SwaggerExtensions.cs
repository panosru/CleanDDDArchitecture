using System.Diagnostics.CodeAnalysis;
using Aviant.Infrastructure.CrossCutting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.Swagger;

/// <summary>
///     Service Collection(IServiceCollection) Extensions
/// </summary>
[ExcludeFromCodeCoverage]
internal static class SwaggerExtensions
{
    /// <summary>
    ///     Add AddVersionedApiExplorer and AddApiVersioning middlewares
    /// </summary>
    /// <param name="services"></param>
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection AddApiVersionWithExplorer(this IServiceCollection services)
    {
        var settings = DependencyInjectionRegistry.DefaultConfiguration
           .GetSection(nameof(SwaggerSettings));

        services.Configure<SwaggerSettings>(settings);

        return services
           .AddVersionedApiExplorer(
                options =>
                {
                    options.GroupNameFormat           = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                })
           .AddApiVersioning(
                options =>
                {
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.ReportApiVersions                   = true;

                    options.DefaultApiVersion = new ApiVersion(
                        settings.GetValue<int>("DefaultApiVersion:Major"),
                        settings.GetValue<int>("DefaultApiVersion:Minor"));

                    options.ApiVersionReader = new HeaderApiVersionReader(
                        "x-api-version");

                    options.Conventions.Add(new VersionByNamespaceConvention());
                });
    }

    /// <summary>
    ///     Add swagger services
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection" />/></param>
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection AddSwaggerOptions(this IServiceCollection services) => services
       .AddTransient<IConfigureOptions<SwaggerOptions>, ConfigureSwaggerOptions>()
       .AddTransient<IConfigureOptions<SwaggerUIOptions>, ConfigureSwaggerUiOptions>()
       .AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();
}
