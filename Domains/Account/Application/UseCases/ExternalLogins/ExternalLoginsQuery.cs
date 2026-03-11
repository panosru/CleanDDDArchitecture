using Aviant.Application.Identity;
using Aviant.Application.Queries;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ExternalLogins;

internal sealed record ExternalLoginsQuery : Query<IReadOnlyCollection<ExternalLoginDto>>
{
    internal sealed class ExternalLoginsQueryHandler
        : QueryHandler<ExternalLoginsQuery, IReadOnlyCollection<ExternalLoginDto>>
    {
        private readonly IAccountAuthenticationService _authenticationService;
        private readonly ICurrentUserService _currentUserService;

        public ExternalLoginsQueryHandler(
            IAccountAuthenticationService authenticationService,
            ICurrentUserService currentUserService)
        {
            _authenticationService = authenticationService;
            _currentUserService = currentUserService;
        }

        public override async Task<IReadOnlyCollection<ExternalLoginDto>> Handle(
            ExternalLoginsQuery request,
            CancellationToken cancellationToken)
        {
            return await _authenticationService
                .GetExternalLoginsAsync(_currentUserService.UserId, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
