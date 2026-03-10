using Aviant.Application.Commands;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.RefreshToken;

internal sealed record RefreshTokenCommand(string RefreshToken) : Command<AuthResult?>
{
    private string RefreshToken { get; } = RefreshToken;

    internal sealed class RefreshTokenCommandHandler : CommandHandler<RefreshTokenCommand, AuthResult?>
    {
        private readonly IAccountAuthenticationService _authenticationService;

        public RefreshTokenCommandHandler(IAccountAuthenticationService authenticationService) =>
            _authenticationService = authenticationService;

        public override async Task<AuthResult?> Handle(
            RefreshTokenCommand command,
            CancellationToken cancellationToken) =>
            await _authenticationService
                .RefreshAsync(command.RefreshToken, cancellationToken)
                .ConfigureAwait(false);
    }
}
