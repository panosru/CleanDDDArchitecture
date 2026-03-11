using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.ExternalProviders;

public sealed class ExternalProvidersResponse
{
    public ExternalProvidersResponse(IReadOnlyCollection<ExternalIdentityProviderDto> providers) => Providers = providers;

    public IReadOnlyCollection<ExternalIdentityProviderDto> Providers { get; }
}
