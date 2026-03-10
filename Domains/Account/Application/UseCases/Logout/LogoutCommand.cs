using Aviant.Application.Commands;
using CleanDDDArchitecture.Domains.Account.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Logout;

internal sealed record LogoutCommand(string RefreshToken) : Command<bool>
{
    private string RefreshToken { get; } = RefreshToken;

    internal sealed class LogoutCommandHandler : CommandHandler<LogoutCommand, bool>
    {
        private readonly IAccountAuthenticationService _authenticationService;

        public LogoutCommandHandler(IAccountAuthenticationService authenticationService) =>
            _authenticationService = authenticationService;

        public override async Task<bool> Handle(
            LogoutCommand command,
            CancellationToken cancellationToken) =>
            await _authenticationService
                .RevokeRefreshTokenAsync(command.RefreshToken, cancellationToken)
                .ConfigureAwait(false);
    }
}
