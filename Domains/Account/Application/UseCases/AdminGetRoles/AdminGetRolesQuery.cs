using Aviant.Application.Identity;
using Aviant.Application.Queries;
using CleanDDDArchitecture.Domains.Account.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminGetRoles;

internal sealed record AdminGetRolesQuery(Guid AccountId) : Query<IReadOnlyCollection<string>>
{
    internal sealed class AdminGetRolesQueryHandler
        : QueryHandler<AdminGetRolesQuery, IReadOnlyCollection<string>>
    {
        private readonly IAccountAdministrationService _accountAdministrationService;
        private readonly ICurrentUserService _currentUserService;

        public AdminGetRolesQueryHandler(
            IAccountAdministrationService accountAdministrationService,
            ICurrentUserService currentUserService)
        {
            _accountAdministrationService = accountAdministrationService;
            _currentUserService = currentUserService;
        }

        public override async Task<IReadOnlyCollection<string>> Handle(
            AdminGetRolesQuery request,
            CancellationToken cancellationToken)
        {
            var roles = await _accountAdministrationService
                .GetRolesAsync(_currentUserService.UserId, request.AccountId, cancellationToken)
                .ConfigureAwait(false);

            return roles ?? throw new KeyNotFoundException("Account was not found or administrator access is required.");
        }
    }
}
