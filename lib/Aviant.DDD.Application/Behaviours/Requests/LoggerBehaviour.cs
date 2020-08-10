namespace Aviant.DDD.Application.Behaviour.Request
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Identity;
    using MediatR.Pipeline;
    using Microsoft.Extensions.Logging;

    public abstract class LoggerBehaviour<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly ICurrentUserService _currentUserService;

        private readonly IIdentityService _identityIdentityService;
        private readonly ILogger _logger;

        public LoggerBehaviour(
            ILogger<TRequest> logger,
            ICurrentUserService currentUserService,
            IIdentityService identityIdentityService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
            _identityIdentityService = identityIdentityService;
        }

        public async Task Process(TRequest request, CancellationToken cancellationToken)
        {
            string requestName = typeof(TRequest).Name;
            var userId = _currentUserService?.UserId ?? Guid.Empty;
            string username = string.Empty;

            if (Guid.Empty != userId) username = await _identityIdentityService.GetUserNameAsync(userId);

            _logger.LogInformation("Request: {Name} {@UserId} {@UserName} {@Request}",
                requestName, userId, username, request);

            await Task.CompletedTask;
        }
    }
}