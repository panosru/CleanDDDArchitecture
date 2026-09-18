using Aviant.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CleanDDDArchitecture.Hosts.ServiceDefaults.Core.Errors;

/// <summary>
///     Turns exceptions that escape a request into RFC 9457 problem details.
/// </summary>
/// <remarks>
///     <list type="bullet">
///         <item><see cref="ValidationException" /> → 400 with the field errors.</item>
///         <item><see cref="NotFoundException" /> → 404.</item>
///         <item>Anything else → 500, logged; the message is only shown in Development.</item>
///     </list>
///     Domain refusals never get here: the Aviant orchestrator returns them as failed
///     responses, which the use cases report as 400s.
/// </remarks>
public sealed partial class ProblemDetailsExceptionHandler(
    IProblemDetailsService                  problemDetails,
    IHostEnvironment                        environment,
    ILogger<ProblemDetailsExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext       httpContext,
        Exception         exception,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(exception);

        ProblemDetails details = exception switch
        {
            ValidationException validation => new ValidationProblemDetails(
                validation.Failures ?? new Dictionary<string, string[]>())
            {
                Status = StatusCodes.Status400BadRequest,
                Title  = "One or more validation failures have occurred."
            },
            NotFoundException notFound => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title  = "The specified resource was not found.",
                Detail = notFound.Message
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title  = "An error occurred while processing your request.",
                Detail = environment.IsDevelopment() ? exception.Message : null
            }
        };

        if (details.Status == StatusCodes.Status500InternalServerError)
            LogUnhandled(exception, httpContext.Request.Method, httpContext.Request.Path);

        httpContext.Response.StatusCode = details.Status!.Value;

        return await problemDetails.TryWriteAsync(
                new ProblemDetailsContext
                {
                    HttpContext    = httpContext,
                    ProblemDetails = details,
                    Exception      = exception
                })
           .ConfigureAwait(false);
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Unhandled exception for {Method} {Path}")]
    private partial void LogUnhandled(Exception exception, string method, PathString path);
}
