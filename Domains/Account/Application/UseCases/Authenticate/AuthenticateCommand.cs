using Aviant.Application.Commands;
using Aviant.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Authenticate;

internal sealed record AuthenticateCommand(
    string Username,
    string Password,
    string? TwoFactorCode,
    string? RecoveryCode) : Command<object?>
{
    private string Username { get; } = Username;

    private string Password { get; } = Password;

    private string? TwoFactorCode { get; } = TwoFactorCode;

    private string? RecoveryCode { get; } = RecoveryCode;

    #region Nested type: AuthenticateCommandHandler

    internal sealed class AuthenticateCommandHandler : CommandHandler<AuthenticateCommand, object?>
    {
        private readonly IIdentityService _identityIdentityService;

        public AuthenticateCommandHandler(IIdentityService identityIdentityService) =>
            _identityIdentityService = identityIdentityService;

        public override async Task<object?> Handle(AuthenticateCommand command, CancellationToken cancellationToken)
        {
            return await _identityIdentityService
               .AuthenticateAsync(
                   command.Username,
                   command.Password,
                   command.TwoFactorCode,
                   command.RecoveryCode,
                   cancellationToken)
               .ConfigureAwait(false);
        }
    }

    #endregion
}
