using Aviant.Application.Commands;
using Aviant.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminUpdateRoles;

internal sealed record AdminUpdateRolesCommand(Guid AccountId, IReadOnlyCollection<string> Roles) : Command<IdentityResult>
{
    internal sealed class AdminUpdateRolesCommandHandler : CommandHandler<AdminUpdateRolesCommand, IdentityResult>
    {
        private readonly IAccountAdministrationService _accountAdministrationService;
        private readonly ICurrentUserService _currentUserService;

        public AdminUpdateRolesCommandHandler(
            IAccountAdministrationService accountAdministrationService,
            ICurrentUserService currentUserService)
        {
            _accountAdministrationService = accountAdministrationService;
            _currentUserService = currentUserService;
        }

        public override async Task<IdentityResult> Handle(
            AdminUpdateRolesCommand command,
            CancellationToken cancellationToken) =>
            await _accountAdministrationService
                .ReplaceRolesAsync(_currentUserService.UserId, command.AccountId, command.Roles, cancellationToken)
                .ConfigureAwait(false);
    }
}
