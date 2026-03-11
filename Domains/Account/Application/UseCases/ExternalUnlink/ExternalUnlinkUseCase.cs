using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ExternalUnlink;

public sealed class ExternalUnlinkUseCase : UseCase<ExternalUnlinkInput, IExternalUnlinkOutput>
{
    public override async Task ExecuteAsync(
        ExternalUnlinkInput input,
        CancellationToken cancellationToken = default)
    {
        var result = await Orchestrator
            .SendCommandAsync(new ExternalUnlinkCommand(input.Provider), cancellationToken)
            .ConfigureAwait(false);

        if (result.Succeeded)
            Output.Ok();
        else
            Output.Invalid(result.Messages.FirstOrDefault() ?? "External login could not be removed.");
    }
}
