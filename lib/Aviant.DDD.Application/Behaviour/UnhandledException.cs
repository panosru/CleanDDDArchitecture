using System.Threading;
using System.Threading.Tasks;
using ApplicationException = Aviant.DDD.Domain.Exception.Create;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Aviant.DDD.Application.Behaviour
{
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
            try
            {
                return await next();
            }
            catch (ApplicationException ex)
            {
                var requestName = typeof(TRequest).Name;

                _logger.LogError(ex, "Unhandled Exception for Request {Name} {@Request}", 
                    requestName, request);

                throw;
            }
        }
    }
}