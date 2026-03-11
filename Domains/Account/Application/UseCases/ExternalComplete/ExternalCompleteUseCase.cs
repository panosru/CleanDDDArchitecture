using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ExternalComplete;

public sealed class ExternalCompleteUseCase : UseCase<ExternalCompleteInput, IExternalCompleteOutput>
{
    public override async Task ExecuteAsync(
        ExternalCompleteInput input,
        CancellationToken cancellationToken = default)
    {
        var result = await Orchestrator
            .SendCommandAsync(
                new ExternalCompleteCommand(input.Provider, input.Code, input.State, input.RedirectUri),
                cancellationToken)
            .ConfigureAwait(false);

        if (result.Succeeded && result.Payload() is Core.Identity.Dto.ExternalAuthenticationResultDto response)
            Output.Ok(response);
        else
            Output.Invalid("External sign-in could not be completed.");
    }
}
