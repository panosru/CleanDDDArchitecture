using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.RefreshToken;

public sealed class RefreshTokenUseCase : UseCase<RefreshTokenInput, IRefreshTokenOutput>
{
    public override async Task ExecuteAsync(
        RefreshTokenInput input,
        CancellationToken cancellationToken = default)
    {
        OrchestratorResponse requestResult = await Orchestrator.SendCommandAsync(
                new RefreshTokenCommand(input.RefreshToken),
                cancellationToken)
            .ConfigureAwait(false);

        if (requestResult.Succeeded && requestResult.Payload() is AuthResult authResult)
            Output.Ok(authResult);
        else
            Output.Unauthorized();
    }
}
