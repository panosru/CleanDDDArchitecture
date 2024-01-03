using System.Security.Authentication;
using Aviant.Application.Commands;
using Aviant.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Authenticate;

internal sealed record AuthenticateCommand(string Username, string Password) : Command<object>
{
    private string Username { get; } = Username;

    private string Password { get; } = Password;

    #region Nested type: AuthenticateCommandHandler

    internal sealed class AuthenticateCommandHandler : CommandHandler<AuthenticateCommand, object>
    {
        private readonly IIdentityService _identityIdentityService;

        public AuthenticateCommandHandler(IIdentityService identityIdentityService) =>
            _identityIdentityService = identityIdentityService;

        public override async Task<object> Handle(AuthenticateCommand command, CancellationToken cancellationToken)
        {
            var user = await _identityIdentityService
               .AuthenticateAsync(command.Username, command.Password, cancellationToken)
               .ConfigureAwait(false);

            return user ?? throw new AuthenticationException();
        }
    }

    #endregion
}
