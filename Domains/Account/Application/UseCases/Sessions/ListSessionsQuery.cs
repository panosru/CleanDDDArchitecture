using Aviant.Application.Identity;
using Aviant.Application.Queries;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Sessions;

internal sealed record ListSessionsQuery : Query<IReadOnlyCollection<AccountSessionDto>>
{
    internal sealed class ListSessionsQueryHandler : QueryHandler<ListSessionsQuery, IReadOnlyCollection<AccountSessionDto>>
    {
        private readonly IAccountAuthenticationService _authenticationService;
        private readonly ICurrentUserService _currentUserService;

        public ListSessionsQueryHandler(
            IAccountAuthenticationService authenticationService,
            ICurrentUserService currentUserService)
        {
            _authenticationService = authenticationService;
            _currentUserService = currentUserService;
        }

        public override async Task<IReadOnlyCollection<AccountSessionDto>> Handle(
            ListSessionsQuery request,
            CancellationToken cancellationToken) =>
            await _authenticationService
                .GetActiveSessionsAsync(_currentUserService.UserId, cancellationToken)
                .ConfigureAwait(false);
    }
}
