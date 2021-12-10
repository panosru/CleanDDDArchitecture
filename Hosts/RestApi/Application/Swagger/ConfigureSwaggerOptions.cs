namespace CleanDDDArchitecture.Hosts.RestApi.Application.Swagger;

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

/// <inheritdoc />
[ExcludeFromCodeCoverage]
internal sealed class ConfigureSwaggerOptions : IConfigureOptions<SwaggerOptions>
{
    /// <summary>
    /// </summary>
    private readonly SwaggerSettings _settings;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ConfigureSwaggerOptions" /> class.
    /// </summary>
    /// <param name="settings"></param>
    public ConfigureSwaggerOptions(IOptions<SwaggerSettings> settings) =>
        _settings = settings.Value ?? new SwaggerSettings();

    #region IConfigureOptions<SwaggerOptions> Members

    /// <inheritdoc />
    public void Configure(SwaggerOptions options)
    {
        options.RouteTemplate = _settings.RoutePrefix + "/{documentName}/swagger.json";

        options.PreSerializeFilters.Add(
            (swaggerDoc, httpReq) =>
                swaggerDoc.Servers = new List<OpenApiServer>
                {
                    new OpenApiServer
                    {
                        Url = $"{httpReq.Scheme}://{httpReq.Host.Value}"
                    }
                });
    }

    #endregion
}
