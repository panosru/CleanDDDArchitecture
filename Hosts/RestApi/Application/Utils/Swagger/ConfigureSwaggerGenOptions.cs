namespace CleanDDDArchitecture.Hosts.RestApi.Application.Utils.Swagger
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    /// <inheritdoc />
    /// <summary>
    ///     Implementation of IConfigureOptions&lt;SwaggerGenOptions&gt;
    /// </summary>
    public sealed class ConfigureSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

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
            Debug.Assert(versionDescriptionProvider != null, $"{nameof(versionDescriptionProvider)} != null");
            Debug.Assert(swaggerSettings            != null, $"{nameof(swaggerSettings)} != null");

            _provider = versionDescriptionProvider;
            _settings = swaggerSettings.Value ?? new SwaggerSettings();
        }

        #region IConfigureOptions<SwaggerGenOptions> Members

        /// <inheritdoc />
        public void Configure(SwaggerGenOptions options)
        {
            options.DocumentFilter<YamlDocumentFilter>();
            options.OperationFilter<SwaggerDefaultValues>();

            //options.DescribeAllEnumsAsStrings();
            options.IgnoreObsoleteActions();
            options.IgnoreObsoleteProperties();

            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    In          = ParameterLocation.Header,
                    Description = "Type into the textbox: Bearer {your JWT token}.",
                    Name        = "Authorization",
                    Type        = SecuritySchemeType.ApiKey
                });

            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id   = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });

            AddSwaggerDocumentForEachDiscoveredApiVersion(options);
            SetCommentsPathForSwaggerJsonAndUi(options);
        }

        #endregion

        private void AddSwaggerDocumentForEachDiscoveredApiVersion(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                _settings.Info.Version = description.ApiVersion.ToString();

                if (description.IsDeprecated) _settings.Info.Description += " - DEPRECATED";

                options.SwaggerDoc(description.GroupName, _settings.Info);
            }
        }

        private static void SetCommentsPathForSwaggerJsonAndUi(SwaggerGenOptions options)
        {
            //TODO: Fix
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            //options.IncludeXmlComments(xmlPath);
        }
    }
}