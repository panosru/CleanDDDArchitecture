using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ExternalProviders;

public sealed class ExternalProvidersUseCase : UseCase<IExternalProvidersOutput>
{
    public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var result = await Orchestrator.SendQueryAsync(new ExternalProvidersQuery(), cancellationToken).ConfigureAwait(false);
        Output.Ok((IReadOnlyCollection<Core.Identity.Dto.ExternalIdentityProviderDto>)(result.Payload() ?? Array.Empty<Core.Identity.Dto.ExternalIdentityProviderDto>()));
    }
}
