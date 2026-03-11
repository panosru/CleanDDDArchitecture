using Aviant.Application.Identity;
using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.PhoneVerification;

public sealed class RequestPhoneVerificationUseCase
    : UseCase<RequestPhoneVerificationInput, IRequestPhoneVerificationOutput>
{
    public override async Task ExecuteAsync(
        RequestPhoneVerificationInput input,
        CancellationToken cancellationToken = default)
    {
        var result = await Orchestrator
            .SendCommandAsync(new RequestPhoneVerificationCommand(input.PhoneNumber, input.IsChange), cancellationToken)
            .ConfigureAwait(false);

        if (result.Succeeded && result.Payload() is IdentityResult { Succeeded: true })
        {
            Output.Accepted();
            return;
        }

        Output.Invalid(result.Messages.FirstOrDefault() ?? "Unable to request phone verification.");
    }
}
