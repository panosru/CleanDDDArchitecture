using Aviant.Application.Queries;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ExternalProviders;

internal sealed record ExternalProvidersQuery : Query<IReadOnlyCollection<ExternalIdentityProviderDto>>
{
    internal sealed class ExternalProvidersQueryHandler
        : QueryHandler<ExternalProvidersQuery, IReadOnlyCollection<ExternalIdentityProviderDto>>
    {
        private readonly IAccountAuthenticationService _authenticationService;

        public ExternalProvidersQueryHandler(IAccountAuthenticationService authenticationService) =>
            _authenticationService = authenticationService;

        public override async Task<IReadOnlyCollection<ExternalIdentityProviderDto>> Handle(
            ExternalProvidersQuery request,
            CancellationToken cancellationToken)
        {
            return await _authenticationService.GetExternalProvidersAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
