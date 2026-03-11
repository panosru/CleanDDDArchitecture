using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Authenticate;

public sealed class AuthenticateUseCase
    : UseCase<AuthenticateInput, IAuthenticateOutput>
{
    public override async Task ExecuteAsync(
        AuthenticateInput input,
        CancellationToken cancellationToken = default)
    {
        OrchestratorResponse requestResult = await Orchestrator.SendCommandAsync(
                new AuthenticateCommand(
                    input.Username,
                    input.Password,
                    input.TwoFactorCode,
                    input.RecoveryCode,
                    input.TrustedDeviceToken,
                    input.RememberDevice,
                    input.DeviceName),
                cancellationToken)
           .ConfigureAwait(false);

        if (requestResult.Succeeded
         && requestResult.Payload() is not null)
            Output.Ok(requestResult.Payload());
        else
            Output.Unauthorized();
    }
}
