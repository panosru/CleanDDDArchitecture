using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.DeleteAccount;

public sealed class DeleteAccountRequestUseCase
    : UseCase<DeleteAccountRequestInput, IDeleteAccountRequestOutput>
{
    public override async Task ExecuteAsync(
        DeleteAccountRequestInput input,
        CancellationToken cancellationToken = default)
    {
        var result = await Orchestrator
            .SendCommandAsync(new DeleteAccountRequestCommand(input.CurrentPassword), cancellationToken)
            .ConfigureAwait(false);

        if (result.Succeeded && result.Payload() is AccountDeletionRequestResult payload && payload.Result.Succeeded)
        {
            Output.Accepted();
            return;
        }

        Output.Invalid(result.Messages.FirstOrDefault() ?? "Unable to request account deletion.");
    }
}
