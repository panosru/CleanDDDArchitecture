using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ExternalLogins;

public sealed class ExternalLoginsUseCase : UseCase<IExternalLoginsOutput>
{
    public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var result = await Orchestrator.SendQueryAsync(new ExternalLoginsQuery(), cancellationToken).ConfigureAwait(false);
        Output.Ok((IReadOnlyCollection<Core.Identity.Dto.ExternalLoginDto>)(result.Payload() ?? Array.Empty<Core.Identity.Dto.ExternalLoginDto>()));
    }
}
