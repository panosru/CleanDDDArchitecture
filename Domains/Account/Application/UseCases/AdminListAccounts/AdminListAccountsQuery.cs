using Aviant.Application.Identity;
using Aviant.Application.Queries;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminListAccounts;

internal sealed record AdminListAccountsQuery(string? Query, string? Status, string? Role)
    : Query<IReadOnlyCollection<AccountAdminSummaryDto>>
{
    internal sealed class AdminListAccountsQueryHandler
        : QueryHandler<AdminListAccountsQuery, IReadOnlyCollection<AccountAdminSummaryDto>>
    {
        private readonly IAccountAdministrationService _accountAdministrationService;
        private readonly ICurrentUserService _currentUserService;

        public AdminListAccountsQueryHandler(
            IAccountAdministrationService accountAdministrationService,
            ICurrentUserService currentUserService)
        {
            _accountAdministrationService = accountAdministrationService;
            _currentUserService = currentUserService;
        }

        public override async Task<IReadOnlyCollection<AccountAdminSummaryDto>> Handle(
            AdminListAccountsQuery request,
            CancellationToken cancellationToken)
        {
            var accounts = await _accountAdministrationService
                .SearchAccountsAsync(
                    _currentUserService.UserId,
                    request.Query,
                    request.Status,
                    request.Role,
                    cancellationToken)
                .ConfigureAwait(false);

            return accounts ?? throw new KeyNotFoundException("Administrator access is required.");
        }
    }
}
