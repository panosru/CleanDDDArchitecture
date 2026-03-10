using Aviant.Application.Commands;
using Aviant.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminUnsuspend;

internal sealed record AdminUnsuspendCommand(Guid AccountId) : Command<IdentityResult>
{
    private Guid AccountId { get; } = AccountId;

    internal sealed class AdminUnsuspendCommandHandler : CommandHandler<AdminUnsuspendCommand, IdentityResult>
    {
        private readonly IAccountAdministrationService _accountAdministrationService;
        private readonly ICurrentUserService _currentUserService;

        public AdminUnsuspendCommandHandler(
            IAccountAdministrationService accountAdministrationService,
            ICurrentUserService currentUserService)
        {
            _accountAdministrationService = accountAdministrationService;
            _currentUserService = currentUserService;
        }

        public override async Task<IdentityResult> Handle(
            AdminUnsuspendCommand command,
            CancellationToken cancellationToken) =>
            await _accountAdministrationService
                .UnsuspendAsync(_currentUserService.UserId, command.AccountId, cancellationToken)
                .ConfigureAwait(false);
    }
}
