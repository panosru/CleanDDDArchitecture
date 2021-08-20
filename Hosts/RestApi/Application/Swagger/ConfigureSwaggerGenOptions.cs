namespace CleanDDDArchitecture.Hosts.RestApi.Application.Swagger
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using AutoMapper.Internal;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    /// <inheritdoc />
    /// <summary>
    ///     Implementation of IConfigureOptions&lt;SwaggerGenOptions&gt;
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed class ConfigureSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions>
    {
        /// <summary>
        /// </summary>
        private readonly IApiVersionDescriptionProvider _provider;

        /// <summary>
        /// </summary>
        private readonly SwaggerSettings _settings;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigureSwaggerGenOptions" /> class.
        /// </summary>
        /// <param name="versionDescriptionProvider">IApiVersionDescriptionProvider</param>
        /// <param name="swaggerSettings">App Settings for Swagger</param>
        public ConfigureSwaggerGenOptions(
            IApiVersionDescriptionProvider versionDescriptionProvider,
            IOptions<SwaggerSettings>      swaggerSettings)
        {
            Debug.Assert(
                versionDescriptionProvider != null,
                $"{nameof(versionDescriptionProvider)} != null");

            Debug.Assert(
                swaggerSettings != null,
                $"{nameof(swaggerSettings)} != null");

            _provider = versionDescriptionProvider;
            _settings = swaggerSettings.Value ?? new SwaggerSettings();
        }

        #region IConfigureOptions<SwaggerGenOptions> Members

        /// <inheritdoc />
        public void Configure(SwaggerGenOptions options)
        {
            options.DocumentFilter<YamlDocumentFilter>();
            options.OperationFilter<SwaggerDefaultValues>();
            options.IgnoreObsoleteActions();

            options.IgnoreObsoleteProperties();

            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Name         = "Authorization",
                    BearerFormat = "JWT",
                    Scheme       = "Bearer",
                    Description  = "Specify the authorization token.",
                    In           = ParameterLocation.Header,
                    Type         = SecuritySchemeType.Http,
                });
            // Add auth header filter
            options.OperationFilter<AuthenticationRequirement>();

            AddSwaggerDocumentForEachDiscoveredApiVersion(options);
            SetCommentsPathForSwaggerJsonAndUi(options);
        }

        #endregion

        /// <summary>
        /// </summary>
        /// <param name="options"></param>
        private void AddSwaggerDocumentForEachDiscoveredApiVersion(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                if (description.IsDeprecated) _settings.Info!.Description += " - DEPRECATED";

                options.SwaggerDoc(
                    description.GroupName,
                    new OpenApiInfo
                    {
                        Title          = _settings.Info!.Title,
                        Description    = _settings.Info.Description,
                        TermsOfService = _settings.Info.TermsOfService,
                        Contact        = _settings.Info.Contact,
                        License        = _settings.Info.License,
                        Version        = description.ApiVersion.ToString()
                    });
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="options"></param>
        private static void SetCommentsPathForSwaggerJsonAndUi(SwaggerGenOptions options)
        {
            // For multiple Api's
            DirectoryInfo baseDir = new(AppContext.BaseDirectory);

            baseDir.EnumerateFiles("*.RestApi.Application.xml")
               .ForAll(
                    file => options.IncludeXmlComments(file.FullName));
        }
    }
}