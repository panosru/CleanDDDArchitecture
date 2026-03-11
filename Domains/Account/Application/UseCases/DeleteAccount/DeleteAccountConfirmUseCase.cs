using Aviant.Application.Identity;
using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.DeleteAccount;

public sealed class DeleteAccountConfirmUseCase
    : UseCase<DeleteAccountConfirmInput, IDeleteAccountConfirmOutput>
{
    public override async Task ExecuteAsync(
        DeleteAccountConfirmInput input,
        CancellationToken cancellationToken = default)
    {
        var result = await Orchestrator
            .SendCommandAsync(new DeleteAccountConfirmCommand(input.Email, input.Token), cancellationToken)
            .ConfigureAwait(false);

        if (result.Succeeded && result.Payload() is IdentityResult { Succeeded: true })
        {
            Output.Ok();
            return;
        }

        Output.Invalid(result.Messages.FirstOrDefault() ?? "Unable to confirm account deletion.");
    }
}
