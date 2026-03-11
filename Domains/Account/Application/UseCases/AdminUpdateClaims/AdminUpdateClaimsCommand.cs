using Aviant.Application.Commands;
using Aviant.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminUpdateClaims;

internal sealed record AdminUpdateClaimsCommand(Guid AccountId, IReadOnlyCollection<AccountClaimDto> Claims) : Command<IdentityResult>
{
    internal sealed class AdminUpdateClaimsCommandHandler : CommandHandler<AdminUpdateClaimsCommand, IdentityResult>
    {
        private readonly IAccountAdministrationService _accountAdministrationService;
        private readonly ICurrentUserService _currentUserService;

        public AdminUpdateClaimsCommandHandler(
            IAccountAdministrationService accountAdministrationService,
            ICurrentUserService currentUserService)
        {
            _accountAdministrationService = accountAdministrationService;
            _currentUserService = currentUserService;
        }

        public override async Task<IdentityResult> Handle(
            AdminUpdateClaimsCommand command,
            CancellationToken cancellationToken) =>
            await _accountAdministrationService
                .ReplaceClaimsAsync(_currentUserService.UserId, command.AccountId, command.Claims, cancellationToken)
                .ConfigureAwait(false);
    }
}
