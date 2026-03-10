using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ResendConfirmation;

public sealed class ResendConfirmationUseCase : UseCase<ResendConfirmationInput, IResendConfirmationOutput>
{
    public override async Task ExecuteAsync(
        ResendConfirmationInput input,
        CancellationToken cancellationToken = default)
    {
        await Orchestrator.SendCommandAsync(
                new ResendConfirmationCommand(input.Email),
                cancellationToken)
            .ConfigureAwait(false);

        Output.Accepted();
    }
}
