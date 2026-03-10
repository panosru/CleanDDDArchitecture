using Aviant.Application.Commands;
using Aviant.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.RevokeSession;

internal sealed record RevokeSessionCommand(Guid SessionId) : Command<bool>
{
    private Guid SessionId { get; } = SessionId;

    internal sealed class RevokeSessionCommandHandler : CommandHandler<RevokeSessionCommand, bool>
    {
        private readonly IAccountAuthenticationService _authenticationService;
        private readonly ICurrentUserService _currentUserService;

        public RevokeSessionCommandHandler(
            IAccountAuthenticationService authenticationService,
            ICurrentUserService currentUserService)
        {
            _authenticationService = authenticationService;
            _currentUserService = currentUserService;
        }

        public override async Task<bool> Handle(
            RevokeSessionCommand command,
            CancellationToken cancellationToken) =>
            await _authenticationService
                .RevokeSessionAsync(_currentUserService.UserId, command.SessionId, cancellationToken)
                .ConfigureAwait(false);
    }
}
