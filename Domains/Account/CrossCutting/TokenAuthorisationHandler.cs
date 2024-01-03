using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace CleanDDDArchitecture.Domains.Account.CrossCutting;

public sealed class TokenAuthorizationHandler
    : AuthorizationHandler<IAuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        IAuthorizationRequirement requirement)
    {
        var httpContext = (context.Resource as AuthorizationFilterContext)?.HttpContext;

        if (httpContext == null)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var endpoint          = httpContext.GetEndpoint();
        var hasAllowAnonymous = endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null;

        if (!hasAllowAnonymous && !httpContext.Request.Headers.ContainsKey("Authorization"))
        {
            httpContext.Response.StatusCode  = StatusCodes.Status401Unauthorized;
            httpContext.Response.ContentType = "application/json";

            var response = new
            {
                status = StatusCodes.Status401Unauthorized,
                message = "Unauthorized access"
            };

            httpContext.Response.WriteAsync(JsonConvert.SerializeObject(response));
            context.Fail();
        }
        else
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

