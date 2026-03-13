using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

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
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Name         = "Authorization",
            BearerFormat = "JWT",
            Scheme       = "bearer",
            Description  = "Specify the authorization token.",
            In           = ParameterLocation.Header,
            Type         = SecuritySchemeType.Http,
        };

        var requirement = new OpenApiSecurityRequirement
        {
            [new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme }
            }] = []
        };

        foreach (var path in document.Paths.Values)
            foreach (var operation in path.Operations.Values)
                operation.Security.Add(requirement);

        return Task.CompletedTask;
    }
}
