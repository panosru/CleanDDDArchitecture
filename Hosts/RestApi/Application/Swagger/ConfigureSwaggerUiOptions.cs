﻿namespace CleanDDDArchitecture.Hosts.RestApi.Application.Swagger
{
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.Extensions.Options;
    using Swashbuckle.AspNetCore.SwaggerUI;

    /// <inheritdoc cref="SwaggerUIOptions" />
    [ExcludeFromCodeCoverage]
    internal sealed class ConfigureSwaggerUiOptions : IConfigureOptions<SwaggerUIOptions>
    {
        /// <summary>
        /// </summary>
        private readonly IApiVersionDescriptionProvider _provider;

        /// <summary>
        /// </summary>
        private readonly SwaggerSettings _settings;

        /// <inheritdoc cref="ConfigureSwaggerUiOptions" />
        public ConfigureSwaggerUiOptions(
            IApiVersionDescriptionProvider versionDescriptionProvider,
            IOptions<SwaggerSettings>      settings)
        {
            Debug.Assert(
                versionDescriptionProvider != null,
                $"{nameof(versionDescriptionProvider)} != null");

            Debug.Assert(
                settings != null,
                $"{nameof(versionDescriptionProvider)} != null");

            _provider = versionDescriptionProvider;
            _settings = settings.Value ?? new SwaggerSettings();
        }

        #region IConfigureOptions<SwaggerUIOptions> Members

        /// <inheritdoc />
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
                            $"/{_settings.RoutePrefix}/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant());

                        options.DocumentTitle = _settings.Name;
                        options.RoutePrefix   = _settings.RoutePrefix;
                        options.DocExpansion(DocExpansion.None);
                        options.DefaultModelExpandDepth(0);
                        options.DisplayRequestDuration();
                        options.EnableFilter();
                    });
        }

        #endregion
    }
}