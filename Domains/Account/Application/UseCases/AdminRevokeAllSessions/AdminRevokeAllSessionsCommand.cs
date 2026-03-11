using Aviant.Application.Commands;
using Aviant.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminRevokeAllSessions;

internal sealed record AdminRevokeAllSessionsCommand(Guid AccountId) : Command<int?>
{
    internal sealed class AdminRevokeAllSessionsCommandHandler : CommandHandler<AdminRevokeAllSessionsCommand, int?>
    {
        private readonly IAccountAdministrationService _accountAdministrationService;
        private readonly ICurrentUserService _currentUserService;

        public AdminRevokeAllSessionsCommandHandler(
            IAccountAdministrationService accountAdministrationService,
            ICurrentUserService currentUserService)
        {
            _accountAdministrationService = accountAdministrationService;
            _currentUserService = currentUserService;
        }

        public override async Task<int?> Handle(
            AdminRevokeAllSessionsCommand command,
            CancellationToken cancellationToken) =>
            await _accountAdministrationService
                .RevokeAllSessionsAsync(_currentUserService.UserId, command.AccountId, cancellationToken)
                .ConfigureAwait(false);
    }
}
