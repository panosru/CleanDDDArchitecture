using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ChangeEmailRequest;

public sealed class ChangeEmailRequestUseCase : UseCase<ChangeEmailRequestInput, IChangeEmailRequestOutput>
{
    public override async Task ExecuteAsync(
        ChangeEmailRequestInput input,
        CancellationToken cancellationToken = default)
    {
        await Orchestrator.SendCommandAsync(
                new ChangeEmailRequestCommand(input.NewEmail),
                cancellationToken)
            .ConfigureAwait(false);

        Output.Accepted();
    }
}
