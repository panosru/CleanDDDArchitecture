using System.Net;
using System.Text.Json;
using Aviant.Application.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CleanDDDArchitecture.Hosts.ServiceDefaults.Core.Errors;

public static class ApiExceptionHandlingExtensions
{
    public static IApplicationBuilder UseApiExceptionHandling(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                Exception? exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

                if (exception is null)
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    return;
                }

                context.Response.ContentType = "application/problem+json";

                object payload = exception switch
                {
                    ValidationException validationException => CreateValidationProblemDetails(validationException, context),
                    NotFoundException notFoundException => CreateProblemDetails(
                        context,
                        StatusCodes.Status404NotFound,
                        "The specified resource was not found.",
                        notFoundException.Message,
                        "https://tools.ietf.org/html/rfc7231#section-6.5.4"),
                    _ => CreateProblemDetails(
                        context,
                        StatusCodes.Status500InternalServerError,
                        "An error occurred while processing your request.",
                        app.ApplicationServices.GetRequiredService<IHostEnvironment>().IsDevelopment() ? exception.Message : null,
                        "https://tools.ietf.org/html/rfc7231#section-6.6.1")
                };

                context.Response.StatusCode = payload switch
                {
                    ProblemDetails problemDetails when problemDetails.Status is not null => problemDetails.Status.Value,
                    _ => (int)HttpStatusCode.InternalServerError
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(payload))
                    .ConfigureAwait(false);
            });
        });

        return app;
    }

    private static ValidationProblemDetails CreateValidationProblemDetails(
        ValidationException exception,
        HttpContext context)
    {
        var details = new ValidationProblemDetails(exception.Failures ?? new Dictionary<string, string[]>())
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "One or more validation failures have occurred.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Instance = context.Request.Path
        };

        return details;
    }

    private static ProblemDetails CreateProblemDetails(
        HttpContext context,
        int statusCode,
        string title,
        string? detail,
        string type)
    {
        return new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = type,
            Instance = context.Request.Path
        };
    }
}
