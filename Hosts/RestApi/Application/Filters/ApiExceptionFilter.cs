namespace CleanDDDArchitecture.Hosts.RestApi.Application.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aviant.DDD.Application.Exceptions;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Core.Services;
    using Aviant.DDD.Infrastructure.CrossCutting;
    using Events;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Hosting;

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    internal sealed class ApiExceptionFilter : ExceptionFilterAttribute
    {
        /// <summary>
        /// </summary>
        private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;

        /// <summary>
        /// </summary>
        public ApiExceptionFilter() =>
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
            Mediator.Send(new ExceptionRaised(context.Exception.Message));

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

            // var details = new ValidationProblemDetails(exception?.Failures)
            // {
            //     Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            // };

            var details = new OrchestratorResponse
            {
                Succeeded = false
            };

            foreach (var failureValue in exception?.Failures!
               .SelectMany(failure => failure.Value))
                details.Messages.Add(failureValue);

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
}