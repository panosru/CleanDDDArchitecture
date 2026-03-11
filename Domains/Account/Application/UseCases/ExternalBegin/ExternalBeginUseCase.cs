using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ExternalBegin;

public sealed class ExternalBeginUseCase : UseCase<ExternalBeginInput, IExternalBeginOutput>
{
    public override async Task ExecuteAsync(
        ExternalBeginInput input,
        CancellationToken cancellationToken = default)
    {
        var result = await Orchestrator
            .SendCommandAsync(new ExternalBeginCommand(input.Provider, input.RedirectUri), cancellationToken)
            .ConfigureAwait(false);

        if (result.Succeeded && result.Payload() is Core.Identity.Dto.ExternalAuthenticationStartDto response)
            Output.Ok(response);
        else
            Output.Invalid("External provider is not configured or the redirect URI is invalid.");
    }
}
