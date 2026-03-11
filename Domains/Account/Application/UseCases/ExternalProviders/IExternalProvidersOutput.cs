using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ExternalProviders;

public interface IExternalProvidersOutput : IUseCaseOutput
{
    void Ok(IReadOnlyCollection<ExternalIdentityProviderDto> providers);
}
