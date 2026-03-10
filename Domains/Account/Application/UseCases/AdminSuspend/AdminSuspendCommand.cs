using Aviant.Application.Commands;
using Aviant.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminSuspend;

internal sealed record AdminSuspendCommand(Guid AccountId, string? Reason) : Command<IdentityResult>
{
    private Guid AccountId { get; } = AccountId;

    private string? Reason { get; } = Reason;

    internal sealed class AdminSuspendCommandHandler : CommandHandler<AdminSuspendCommand, IdentityResult>
    {
        private readonly IAccountAdministrationService _accountAdministrationService;
        private readonly ICurrentUserService _currentUserService;

        public AdminSuspendCommandHandler(
            IAccountAdministrationService accountAdministrationService,
            ICurrentUserService currentUserService)
        {
            _accountAdministrationService = accountAdministrationService;
            _currentUserService = currentUserService;
        }

        public override async Task<IdentityResult> Handle(
            AdminSuspendCommand command,
            CancellationToken cancellationToken) =>
            await _accountAdministrationService
                .SuspendAsync(_currentUserService.UserId, command.AccountId, command.Reason, cancellationToken)
                .ConfigureAwait(false);
    }
}
