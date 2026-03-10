using Aviant.Application.Identity;
using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ChangePassword;

public sealed class ChangePasswordUseCase : UseCase<ChangePasswordInput, IChangePasswordOutput>
{
    public override async Task ExecuteAsync(
        ChangePasswordInput input,
        CancellationToken cancellationToken = default)
    {
        var requestResult = await Orchestrator.SendCommandAsync(
                new ChangePasswordCommand(input.CurrentPassword, input.NewPassword),
                cancellationToken)
            .ConfigureAwait(false);

        if (requestResult.Succeeded && requestResult.Payload() is IdentityResult { Succeeded: true })
        {
            Output.Ok();
            return;
        }

        if (requestResult.Payload() is IdentityResult identityResult)
        {
            Output.Invalid(identityResult.Errors);
            return;
        }

        Output.Invalid(requestResult.Messages);
    }
}
