using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.Swagger;

/// <summary>
///     Represents the Swagger/Swashbuckle operation filter used to document the implicit API version parameter.
/// </summary>
/// <remarks>
///     This <see cref="IOperationFilter" /> is only required due to bugs in the <see cref="SwaggerGenerator" />.
///     Once they are fixed and published, this class can be removed.
/// </remarks>
[ExcludeFromCodeCoverage]
internal sealed class SwaggerDefaultValues : IOperationFilter
{
    #region IOperationFilter Members

    /// <summary>
    ///     Applies the filter to the specified operation using the given context.
    /// </summary>
    /// <param name="operation">The operation to apply the filter to.</param>
    /// <param name="context">The current operation filter context.</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters is null)
            return;

        foreach (var parameter in operation.Parameters)
        {
            var description = context.ApiDescription.ParameterDescriptions.First(
                p => p.Name == parameter.Name);
            var routeInfo = description.RouteInfo;

            parameter.Description ??= description.ModelMetadata.Description;

            if (routeInfo is null)
                continue;

            parameter.Required |= !routeInfo.IsOptional;
        }
    }

    #endregion
}
