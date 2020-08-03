namespace Aviant.DDD.Application.Behaviour
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Domain.Exception;
    using MediatR;
    using Microsoft.Extensions.Logging;

    public class UnhandledException<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<TRequest> _logger;

        public UnhandledException(ILogger<TRequest> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            Console.WriteLine("************");
            Console.WriteLine(typeof(TRequest).Name);
            Console.WriteLine(request.ToString());
            Console.WriteLine("************");
            
            try
            {
                return await next();
            }
            catch (Create ex)
            {
                var requestName = typeof(TRequest).Name;

                _logger.LogError(ex, "Unhandled Exception for Request {Name} {@Request}",
                    requestName, request);

                throw;
            }
        }
    }
}