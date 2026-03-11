using Aviant.Application.Identity;
using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.PhoneVerification;

public sealed class VerifyPhoneVerificationUseCase
    : UseCase<VerifyPhoneVerificationInput, IVerifyPhoneVerificationOutput>
{
    public override async Task ExecuteAsync(
        VerifyPhoneVerificationInput input,
        CancellationToken cancellationToken = default)
    {
        var result = await Orchestrator
            .SendCommandAsync(
                new VerifyPhoneVerificationCommand(input.PhoneNumber, input.Code, input.IsChange),
                cancellationToken)
            .ConfigureAwait(false);

        if (result.Succeeded && result.Payload() is IdentityResult { Succeeded: true })
        {
            Output.Ok();
            return;
        }

        Output.Invalid(result.Messages.FirstOrDefault() ?? "Unable to verify the phone number.");
    }
}
