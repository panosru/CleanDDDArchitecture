using Aviant.Application.Exceptions;
using Aviant.Core.Services;
using Aviant.Core.Timing;
using Aviant.Infrastructure.CrossCutting;
using CleanDDDArchitecture.Hosts.RestApi.Presentation.Events;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.Filters;

/// <inheritdoc />
/// <summary>
/// </summary>
internal sealed class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    /// <summary>
    /// </summary>
    private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;

    /// <summary>
    /// </summary>
    public ApiExceptionFilterAttribute() =>
        // Register known exception types and handlers.
        _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
        {
            { typeof(ValidationException), HandleValidationException },
            { typeof(NotFoundException), HandleNotFoundException }
        };

    /// <summary>
    /// </summary>
    private static IMediator Mediator =>
        ServiceLocator.ServiceContainer.GetRequiredService<IMediator>(
            typeof(IMediator));

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    public override void OnException(ExceptionContext context)
    {
        Console.WriteLine("###########");
        Console.WriteLine(context.Exception);
        Console.WriteLine("###########");

        Mediator.Publish(
            new ExceptionRaised(
                context.Exception.Source,
                context.Exception.Message,
                context.Exception.StackTrace,
                JsonConvert.SerializeObject(context.Exception))
            {
                Occured = context.Exception.Occurred()
            });

        HandleException(context);

        base.OnException(context);
    }

    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    private void HandleException(ExceptionContext context)
    {
        Type type = context.Exception.GetType();

        if (_exceptionHandlers.ContainsKey(type))
        {
            _exceptionHandlers[type].Invoke(context);

            return;
        }

        HandleUnknownException(context);
    }

    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    private static void HandleUnknownException(ExceptionContext context)
    {
        var details = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title  = "An error occurred while processing your request.",
            Type   = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };

        if (DependencyInjectionRegistry.CurrentEnvironment.IsDevelopment())
            details.Detail = context.Exception.Message;

        context.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };

        context.ExceptionHandled = true;
    }

    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    private static void HandleValidationException(ExceptionContext context)
    {
        var exception = context.Exception as ValidationException;

        var details = new ValidationProblemDetails(exception?.Failures)
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        };

        context.Result = new BadRequestObjectResult(details);

        context.ExceptionHandled = true;
    }

    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    private static void HandleNotFoundException(ExceptionContext context)
    {
        var exception = context.Exception as NotFoundException;

        var details = new ProblemDetails
        {
            Type   = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Title  = "The specified resource was not found.",
            Detail = exception?.Message
        };

        context.Result = new NotFoundObjectResult(details);

        context.ExceptionHandled = true;
    }
}

/// <summary>
/// Context Extension Exception
/// </summary>
internal static class ExceptionContextExtension
{
    /// <summary>
    /// Datetime of the Occurred Exception
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static DateTime Occurred(this Exception exception) =>
        (DateTime?)exception
           .GetType()
           .GetProperty("Occurred")
          ?.GetValue(exception)
     ?? Clock.Now;
}
