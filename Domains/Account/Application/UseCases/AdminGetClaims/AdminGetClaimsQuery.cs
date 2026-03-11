using Aviant.Application.Identity;
using Aviant.Application.Queries;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminGetClaims;

internal sealed record AdminGetClaimsQuery(Guid AccountId) : Query<IReadOnlyCollection<AccountClaimDto>>
{
    internal sealed class AdminGetClaimsQueryHandler
        : QueryHandler<AdminGetClaimsQuery, IReadOnlyCollection<AccountClaimDto>>
    {
        private readonly IAccountAdministrationService _accountAdministrationService;
        private readonly ICurrentUserService _currentUserService;

        public AdminGetClaimsQueryHandler(
            IAccountAdministrationService accountAdministrationService,
            ICurrentUserService currentUserService)
        {
            _accountAdministrationService = accountAdministrationService;
            _currentUserService = currentUserService;
        }

        public override async Task<IReadOnlyCollection<AccountClaimDto>> Handle(
            AdminGetClaimsQuery request,
            CancellationToken cancellationToken)
        {
            var claims = await _accountAdministrationService
                .GetClaimsAsync(_currentUserService.UserId, request.AccountId, cancellationToken)
                .ConfigureAwait(false);

            return claims ?? throw new KeyNotFoundException("Account was not found or administrator access is required.");
        }
    }
}
