using Aviant.Application.Commands;
using Aviant.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.DeleteAccount;

internal sealed record DeleteAccountConfirmCommand(string Email, string Token) : Command<IdentityResult>
{
    internal sealed class DeleteAccountConfirmCommandHandler : CommandHandler<DeleteAccountConfirmCommand, IdentityResult>
    {
        private readonly IAccountAdministrationService _accountAdministrationService;

        public DeleteAccountConfirmCommandHandler(IAccountAdministrationService accountAdministrationService) =>
            _accountAdministrationService = accountAdministrationService;

        public override async Task<IdentityResult> Handle(
            DeleteAccountConfirmCommand command,
            CancellationToken cancellationToken) =>
            await _accountAdministrationService
                .ConfirmAccountDeletionAsync(command.Email, command.Token, cancellationToken)
                .ConfigureAwait(false);
    }
}
