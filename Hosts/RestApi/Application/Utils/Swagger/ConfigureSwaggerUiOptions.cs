namespace CleanDDDArchitecture.Hosts.RestApi.Application.Utils.Swagger
{
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.Extensions.Options;
    using Swashbuckle.AspNetCore.SwaggerUI;

    /// <inheritdoc cref="SwaggerUIOptions" />
    public sealed class ConfigureSwaggerUiOptions : IConfigureOptions<SwaggerUIOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        private readonly SwaggerSettings _settings;

        /// <inheritdoc cref="ConfigureSwaggerUiOptions" />
        public ConfigureSwaggerUiOptions(
            IApiVersionDescriptionProvider versionDescriptionProvider,
            IOptions<SwaggerSettings>      settings)
        {
            Debug.Assert(versionDescriptionProvider != null, $"{nameof(versionDescriptionProvider)} != null");
            Debug.Assert(settings                   != null, $"{nameof(versionDescriptionProvider)} != null");

            _provider = versionDescriptionProvider;
            _settings = settings?.Value ?? new SwaggerSettings();
        }

        #region IConfigureOptions<SwaggerUIOptions> Members

        /// <summary>
        ///     Configure
        /// </summary>
        /// <param name="options"></param>
        public void Configure(SwaggerUIOptions options)
        {
            _provider
               .ApiVersionDescriptions
               .ToList()
               .ForEach(
                    description =>
                    {
                        options.SwaggerEndpoint(
                            $"/{_settings.RoutePrefixWithSlash}{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant());

                        options.RoutePrefix = _settings.RoutePrefix;
                    });
        }

        #endregion
    }
}