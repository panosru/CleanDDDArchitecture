namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ConfirmEmail;

using Aviant.Foundation.Application.Identity;
using Aviant.Foundation.Application.Orchestration;
using Aviant.Foundation.Application.UseCases;

public sealed class ConfirmEmailUseCase
    : UseCase<ConfirmEmailInput, IConfirmEmailOutput>
{
    public override async Task ExecuteAsync(
        ConfirmEmailInput input,
        CancellationToken cancellationToken = default)
    {
        OrchestratorResponse requestResult = await Orchestrator.SendCommandAsync(
                new ConfirmEmailCommand(
                    input.Token,
                    input.Email),
                cancellationToken)
           .ConfigureAwait(false);

        if (requestResult.Succeeded)
        {
            var identityResult = requestResult.Payload<IdentityResult>();

            if (identityResult.Succeeded)
                Output.Ok();
            else
                Output.Invalid(identityResult.Errors.First());
        }
        else
        {
            Output.Invalid(requestResult.Messages.First());
        }
    }
}
