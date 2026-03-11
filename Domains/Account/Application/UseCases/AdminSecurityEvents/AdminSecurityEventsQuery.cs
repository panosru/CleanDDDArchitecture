using Aviant.Application.Identity;
using Aviant.Application.Queries;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminSecurityEvents;

internal sealed record AdminSecurityEventsQuery(Guid AccountId) : Query<IReadOnlyCollection<AccountSecurityEventDto>>
{
    internal sealed class AdminSecurityEventsQueryHandler
        : QueryHandler<AdminSecurityEventsQuery, IReadOnlyCollection<AccountSecurityEventDto>>
    {
        private readonly IAccountAdministrationService _accountAdministrationService;
        private readonly ICurrentUserService _currentUserService;

        public AdminSecurityEventsQueryHandler(
            IAccountAdministrationService accountAdministrationService,
            ICurrentUserService currentUserService)
        {
            _accountAdministrationService = accountAdministrationService;
            _currentUserService = currentUserService;
        }

        public override async Task<IReadOnlyCollection<AccountSecurityEventDto>> Handle(
            AdminSecurityEventsQuery request,
            CancellationToken cancellationToken)
        {
            var events = await _accountAdministrationService
                .GetSecurityEventsAsync(_currentUserService.UserId, request.AccountId, cancellationToken)
                .ConfigureAwait(false);

            return events ?? throw new KeyNotFoundException("Account was not found or administrator access is required.");
        }
    }
}
