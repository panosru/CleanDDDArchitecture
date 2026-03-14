using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.Swagger;

/// <summary>
///     OpenAPI document transformer that adds JWT Bearer security scheme and applies it to all operations.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class JwtBearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    /// <inheritdoc />
    public Task TransformAsync(
        OpenApiDocument                  document,
        OpenApiDocumentTransformerContext context,
        CancellationToken                cancellationToken)
    {
        // Ensure Components and its SecuritySchemes dict exist before writing
        document.Components ??= new OpenApiComponents();
        var securitySchemes = document.Components.SecuritySchemes
            ??= new Dictionary<string, IOpenApiSecurityScheme>();

        securitySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Name         = "Authorization",
            BearerFormat = "JWT",
            Scheme       = "bearer",
            Description  = "Specify the authorization token.",
            In           = ParameterLocation.Header,
            Type         = SecuritySchemeType.Http,
        };

        // In Microsoft.OpenApi 2.0 the key type for OpenApiSecurityRequirement is
        // OpenApiSecuritySchemeReference (not OpenApiSecurityScheme with a Reference property)
        var requirement = new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", null, null)] = []
        };

        foreach (var path in document.Paths.Values)
        foreach (var operation in (path.Operations ?? []).Values)
            (operation.Security ??= []).Add(requirement);

        return Task.CompletedTask;
    }
}
