namespace Aviant.DDD.Application.Behaviours.Requests
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Identity;
    using MediatR;
    using Microsoft.Extensions.Logging;

    public class PerformanceBehaviour<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ICurrentUserService _currentUserService;

        private readonly IIdentityService _identityIdentityService;

        private readonly ILogger<TRequest> _logger;

        private readonly Stopwatch _timer;

        public PerformanceBehaviour(
            ILogger<TRequest> logger,
            ICurrentUserService currentUserService,
            IIdentityService identityIdentityService)
        {
            _timer = new Stopwatch();

            _logger = logger;
            _currentUserService = currentUserService;
            _identityIdentityService = identityIdentityService;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            _timer.Start();
            var response = await next();
            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            if (500 < elapsedMilliseconds)
            {
                string requestName = typeof(TRequest).Name;
                var userId = _currentUserService?.UserId ?? Guid.Empty;
                string username = string.Empty;

                if (Guid.Empty != userId) username = await _identityIdentityService.GetUserNameAsync(userId);

                _logger.LogWarning(
                    "Long Running Request detected: {Name} ({ElapsedMilliseconds} milliseconds), UserId: {@UserId}, Username: {@username}, Request: {@Request}",
                    requestName, elapsedMilliseconds, userId, username, request);
            }

            return response;
        }
    }
}