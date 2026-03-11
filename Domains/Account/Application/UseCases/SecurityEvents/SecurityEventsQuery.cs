using Aviant.Application.Identity;
using Aviant.Application.Queries;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.SecurityEvents;

internal sealed record SecurityEventsQuery : Query<IReadOnlyCollection<AccountSecurityEventDto>>
{
    internal sealed class SecurityEventsQueryHandler
        : QueryHandler<SecurityEventsQuery, IReadOnlyCollection<AccountSecurityEventDto>>
    {
        private readonly IAccountAdministrationService _accountAdministrationService;
        private readonly ICurrentUserService _currentUserService;

        public SecurityEventsQueryHandler(
            IAccountAdministrationService accountAdministrationService,
            ICurrentUserService currentUserService)
        {
            _accountAdministrationService = accountAdministrationService;
            _currentUserService = currentUserService;
        }

        public override async Task<IReadOnlyCollection<AccountSecurityEventDto>> Handle(
            SecurityEventsQuery request,
            CancellationToken cancellationToken) =>
            await _accountAdministrationService
                .GetOwnSecurityEventsAsync(_currentUserService.UserId, cancellationToken)
                .ConfigureAwait(false);
    }
}
