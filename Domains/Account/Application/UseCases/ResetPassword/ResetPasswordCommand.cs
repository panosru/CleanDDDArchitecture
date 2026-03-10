using Aviant.Application.Commands;
using Aviant.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ResetPassword;

internal sealed record ResetPasswordCommand(string Email, string Token, string Password) : Command<IdentityResult>
{
    private string Email { get; } = Email;

    private string Token { get; } = Token;

    private string Password { get; } = Password;

    internal sealed class ResetPasswordCommandHandler : CommandHandler<ResetPasswordCommand, IdentityResult>
    {
        private readonly IIdentityService _identityService;

        public ResetPasswordCommandHandler(IIdentityService identityService) => _identityService = identityService;

        public override async Task<IdentityResult> Handle(
            ResetPasswordCommand command,
            CancellationToken cancellationToken) =>
            await _identityService
                .ResetPasswordAsync(command.Email, command.Token, command.Password, cancellationToken)
                .ConfigureAwait(false);
    }
}
