using System.Net;
using CleanDDDArchitecture.Domains.Account.Core.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Identity;

public static class JwtBearerEventsConfigurator
{
    public static JwtBearerEvents GetJwtBearerEvents(IEnumerable<string> MandatoryClaims) => new()
    {
        OnTokenValidated = context =>
        {
            // Ensure that mandatory claims are set
            if (!MandatoryClaims.All(
                    mandatoryClaim => context.Principal!.HasClaim(
                        claim => claim.Type == mandatoryClaim)))
                context.Fail("Missing claims.");

            return Task.CompletedTask;
        },

        OnForbidden = context =>
        {
            context.Fail(
                new IdentityException(
                    "You are not authorised to access this resource",
                    HttpStatusCode.Forbidden));

            return Task.CompletedTask;
        },

        OnChallenge = async context =>
        {
            // this is a default method
            // the response statusCode and headers are set here
            context.HandleResponse();

            if (context.AuthenticateFailure is not null)
            {
                JObject payload = new()
                {
                    ["error"] = context.Error,
                    ["error_description"] = context.ErrorDescription,
                    ["message"] = context.AuthenticateFailure.Message
                };

                context.Response.ContentType = "application/json";

                context.Response.StatusCode = context.AuthenticateFailure is IdentityException
                    exception
                    ? exception.ErrorCode
                    : (int)HttpStatusCode.Unauthorized;

                await context.HttpContext.Response.WriteAsync(payload.ToString())
                    .ConfigureAwait(false);
            }
        },

        OnAuthenticationFailed = context =>
        {
            // Check if token has expired
            if (context.Exception is SecurityTokenExpiredException)
            {
                context.Response.Headers.Add("Token-Expired", "true");
            }

            return Task.CompletedTask;
        },
    };
}