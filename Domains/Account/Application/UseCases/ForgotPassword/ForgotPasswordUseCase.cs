using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ForgotPassword;

public sealed class ForgotPasswordUseCase : UseCase<ForgotPasswordInput, IForgotPasswordOutput>
{
    public override async Task ExecuteAsync(
        ForgotPasswordInput input,
        CancellationToken cancellationToken = default)
    {
        await Orchestrator.SendCommandAsync(
                new ForgotPasswordCommand(input.Email),
                cancellationToken)
            .ConfigureAwait(false);

        Output.Accepted();
    }
}
