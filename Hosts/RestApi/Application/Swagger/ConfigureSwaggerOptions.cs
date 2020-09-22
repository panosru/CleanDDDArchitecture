namespace CleanDDDArchitecture.Hosts.RestApi.Application.Swagger
{
    using Microsoft.Extensions.Options;
    using Swashbuckle.AspNetCore.Swagger;

    /// <inheritdoc />
    public sealed class ConfigureSwaggerOptions : IConfigureOptions<SwaggerOptions>
    {
        private readonly SwaggerSettings _settings;

        /// <summary>
        /// </summary>
        /// <param name="settings"></param>
        public ConfigureSwaggerOptions(IOptions<SwaggerSettings> settings) =>
            _settings = settings?.Value ?? new SwaggerSettings();

        #region IConfigureOptions<SwaggerOptions> Members

        /// <inheritdoc />
        public void Configure(SwaggerOptions options)
        {
            options.RouteTemplate = _settings.RoutePrefixWithSlash + "{documentName}/swagger.json";
        }

        #endregion
    }
}