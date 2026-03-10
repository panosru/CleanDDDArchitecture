using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;
using Aviant.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ResetPassword;

public sealed class ResetPasswordUseCase : UseCase<ResetPasswordInput, IResetPasswordOutput>
{
    public override async Task ExecuteAsync(
        ResetPasswordInput input,
        CancellationToken cancellationToken = default)
    {
        var requestResult = await Orchestrator.SendCommandAsync(
                new ResetPasswordCommand(input.Email, input.Token, input.Password),
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
