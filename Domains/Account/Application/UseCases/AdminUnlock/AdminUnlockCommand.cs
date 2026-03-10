using Aviant.Application.Commands;
using Aviant.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminUnlock;

internal sealed record AdminUnlockCommand(Guid AccountId) : Command<IdentityResult>
{
    private Guid AccountId { get; } = AccountId;

    internal sealed class AdminUnlockCommandHandler : CommandHandler<AdminUnlockCommand, IdentityResult>
    {
        private readonly IAccountAdministrationService _accountAdministrationService;
        private readonly ICurrentUserService _currentUserService;

        public AdminUnlockCommandHandler(
            IAccountAdministrationService accountAdministrationService,
            ICurrentUserService currentUserService)
        {
            _accountAdministrationService = accountAdministrationService;
            _currentUserService = currentUserService;
        }

        public override async Task<IdentityResult> Handle(
            AdminUnlockCommand command,
            CancellationToken cancellationToken) =>
            await _accountAdministrationService
                .UnlockAsync(_currentUserService.UserId, command.AccountId, cancellationToken)
                .ConfigureAwait(false);
    }
}
