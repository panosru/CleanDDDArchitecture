using System.Diagnostics.CodeAnalysis;
using Aviant.Core.Collections.Extensions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.Swagger;

/// <inheritdoc />
/// <summary>
///     Implementation of IConfigureOptions{SwaggerGenOptions}
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
        _provider = versionDescriptionProvider;
        _settings = swaggerSettings.Value ?? new SwaggerSettings();
    }

    #region IConfigureOptions<SwaggerGenOptions> Members

    /// <inheritdoc />
    public void Configure(SwaggerGenOptions options)
    {
        options.OperationFilter<SwaggerDefaultValues>();
        options.IgnoreObsoleteActions();

        options.IgnoreObsoleteProperties();

        options.AddSecurityDefinition(
            "Bearer",
            new OpenApiSecurityScheme
            {
                Name         = "Authorization",
                BearerFormat = "JWT",
                Scheme       = "bearer",  // Scheme should be in lowercase
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
            var versionKey = $"V{description.ApiVersion.MajorVersion}_{description.ApiVersion.MinorVersion}";
            var defaultInfo = new OpenApiInfo
            {
                Title = _settings.Info?.Title,
                Description = _settings.Info?.Description,
                TermsOfService = _settings.Info?.TermsOfService,
                Contact = _settings.Info?.Contact,
                License = _settings.Info?.License,
                Version = description.ApiVersion.ToString()
            };

            if (_settings.VersionInfo != null && 
                _settings.VersionInfo.TryGetValue(versionKey, out var overrideInfo))
            {
                // Override specific fields if they exist
                defaultInfo.Title = overrideInfo.Title ?? defaultInfo.Title;
                defaultInfo.Description = overrideInfo.Description ?? defaultInfo.Description;
                defaultInfo.TermsOfService = overrideInfo.TermsOfService ?? defaultInfo.TermsOfService;
                defaultInfo.Contact = overrideInfo.Contact ?? defaultInfo.Contact;
                defaultInfo.License = overrideInfo.License ?? defaultInfo.License;
            }

            if (description.IsDeprecated)
                defaultInfo.Description += " - DEPRECATED";

            options.SwaggerDoc(description.GroupName, defaultInfo);
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="options"></param>
    private static void SetCommentsPathForSwaggerJsonAndUi(SwaggerGenOptions options)
    {
        // For multiple Api's
        DirectoryInfo baseDir = new(AppContext.BaseDirectory);

        baseDir.EnumerateFiles("*.RestApi.Presentation.xml")
           .ForAll(
                file => options.IncludeXmlComments(file.FullName));
    }
}
