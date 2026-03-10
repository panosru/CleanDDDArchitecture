using Aviant.Application.Commands;
using Aviant.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.LogoutAll;

internal sealed record LogoutAllCommand : Command<int>
{
    internal sealed class LogoutAllCommandHandler : CommandHandler<LogoutAllCommand, int>
    {
        private readonly IAccountAuthenticationService _authenticationService;
        private readonly ICurrentUserService _currentUserService;

        public LogoutAllCommandHandler(
            IAccountAuthenticationService authenticationService,
            ICurrentUserService currentUserService)
        {
            _authenticationService = authenticationService;
            _currentUserService = currentUserService;
        }

        public override async Task<int> Handle(
            LogoutAllCommand command,
            CancellationToken cancellationToken) =>
            await _authenticationService
                .RevokeAllRefreshTokensAsync(_currentUserService.UserId, cancellationToken)
                .ConfigureAwait(false);
    }
}
