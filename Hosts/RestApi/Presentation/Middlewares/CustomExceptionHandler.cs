using System.Net;
using Aviant.Application.Exceptions;
using Newtonsoft.Json;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.Middlewares;

/// <summary>
/// </summary>
internal sealed class CustomExceptionHandler
{
    /// <summary>
    /// </summary>
    private readonly RequestDelegate _next;

    /// <summary>
    /// </summary>
    /// <param name="next"></param>
    public CustomExceptionHandler(RequestDelegate next) => _next = next;

    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex)
               .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="exception"></param>
    /// <returns></returns>
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;

        var result = string.Empty;

        switch (exception)
        {
            case ValidationException validationException:
                code   = HttpStatusCode.BadRequest;
                result = JsonConvert.SerializeObject(validationException.Failures);

                break;

            case NotFoundException _:
                code = HttpStatusCode.NotFound;

                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode  = (int)code;

        if (string.IsNullOrEmpty(result)) result = JsonConvert.SerializeObject(new { error = exception.Message });

        return context.Response.WriteAsync(result);
    }
}

/// <summary>
/// </summary>
internal static class CustomExceptionHandlerExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder) =>
        builder.UseMiddleware<CustomExceptionHandler>();
}
