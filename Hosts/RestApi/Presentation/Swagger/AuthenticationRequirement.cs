using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.Swagger;

/// <summary>
///     Swagger filter to add the bearer into each request.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class AuthenticationRequirement : IOperationFilter
{
    #region IOperationFilter Members

    /// <summary>
    ///     Adds the bearer token into each request.
    /// </summary>
    /// <param name="operation">The swagger operation.</param>
    /// <param name="context">The filter context.</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Security ??= new List<OpenApiSecurityRequirement>();

        if (!(context.ApiDescription.ActionDescriptor is ControllerActionDescriptor cad))
            return;

        var controllerAuth = cad.ControllerTypeInfo.GetCustomAttributes(true)
           .Any(t => t is AuthorizeAttribute);

        var methodAuth = cad.MethodInfo.GetCustomAttributes(false)
           .Any(t => t is AuthorizeAttribute);

        var methodAnonymous = cad.MethodInfo.GetCustomAttributes(false)
           .Any(t => t is AllowAnonymousAttribute);

        if ((!controllerAuth || methodAnonymous)
         && (controllerAuth  || !methodAuth))
            return;

        var scheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Id   = "Bearer",
                Type = ReferenceType.SecurityScheme,
            }
        };

        operation.Security.Add(
            new OpenApiSecurityRequirement
            {
                [scheme] = new List<string>()
            });
    }

    #endregion
}
