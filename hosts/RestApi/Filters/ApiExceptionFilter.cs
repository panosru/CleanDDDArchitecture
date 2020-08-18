namespace CleanDDDArchitecture.RestApi.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Application.Common.Events;
    using Aviant.DDD.Application.Exceptions;
    using Aviant.DDD.Application.Orchestration;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// </summary>
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;
        private IMediator? _mediator;

        /// <summary>
        /// </summary>
        public ApiExceptionFilter()
        {
            // Register known exception types and handlers.
            _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
            {
                {typeof(ValidationException), HandleValidationException},
                {typeof(NotFoundException), HandleNotFoundException}
            };
        }

        /// <summary>
        /// </summary>
        protected IMediator Mediator =>
            _mediator ??= new ControllerContext().HttpContext.RequestServices.GetService<IMediator>();

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(ExceptionContext context)
        {
            Console.WriteLine("###########");
            Console.WriteLine(context.Exception);
            Console.WriteLine("###########");
            _mediator?.Send(new ExceptionRaised(context.Exception.Message));

            HandleException(context);

            base.OnException(context);
        }

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

        private void HandleUnknownException(ExceptionContext context)
        {
            var details = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An error occurred while processing your request.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            };

            context.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

            context.ExceptionHandled = true;
        }

        private void HandleValidationException(ExceptionContext context)
        {
            var exception = context.Exception as ValidationException;

            // var details = new ValidationProblemDetails(exception?.Failures)
            // {
            //     Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            // };

            var details = new RequestResult
            {
                Success = false
            };

            foreach (var failure in exception?.Failures)
            {
                foreach (var failureValue in failure.Value)
                {
                    details.Messages.Add(failureValue);
                }
            } 

            context.Result = new BadRequestObjectResult(details);

            context.ExceptionHandled = true;
        }

        private void HandleNotFoundException(ExceptionContext context)
        {
            var exception = context.Exception as NotFoundException;

            var details = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "The specified resource was not found.",
                Detail = exception?.Message
            };

            context.Result = new NotFoundObjectResult(details);

            context.ExceptionHandled = true;
        }
    }
}