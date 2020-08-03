namespace Aviant.DDD.Application.Behaviour.Request
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Identity;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using IIdentityService = Identity.IService;

    public class Performance<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ICurrentUserService _currentUserService;

        private readonly IIdentityService _identityService;

        private readonly ILogger<TRequest> _logger;

        private readonly Stopwatch _timer;

        public Performance(
            ILogger<TRequest> logger,
            ICurrentUserService currentUserService,
            IIdentityService identityService)
        {
            _timer = new Stopwatch();

            _logger = logger;
            _currentUserService = currentUserService;
            _identityService = identityService;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            _timer.Start();
            var response = await next();
            _timer.Stop();

            Console.WriteLine("£@$!@!£!$!!$!@");
            Console.WriteLine(_timer.ElapsedMilliseconds);
            Console.WriteLine("£@$!@!£!$!!$!@");

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            if (500 < elapsedMilliseconds)
            {
                string requestName = typeof(TRequest).Name;
                var userId = _currentUserService?.UserId ?? Guid.Empty;
                string username = string.Empty;

                if (Guid.Empty == userId) username = await _identityService.GetUserNameAsync(userId);

                _logger.LogWarning(
                    "Long Running Request detected: {Name} ({ElapsedMilliseconds} milliseconds), UserId: {@UserId}, Username: {@username}, Request: {@Request}",
                    requestName, elapsedMilliseconds, userId, username, request);
            }

            return response;
        }
    }
}