using Aviant.Application.Identity;
using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminUpdateClaims;

public sealed class AdminUpdateClaimsUseCase : UseCase<AdminUpdateClaimsInput, IAdminUpdateClaimsOutput>
{
    public override async Task ExecuteAsync(
        AdminUpdateClaimsInput input,
        CancellationToken cancellationToken = default)
    {
        var result = await Orchestrator
            .SendCommandAsync(new AdminUpdateClaimsCommand(input.AccountId, input.Claims), cancellationToken)
            .ConfigureAwait(false);

        if (result.Succeeded && result.Payload() is IdentityResult { Succeeded: true })
        {
            Output.Ok();
            return;
        }

        Output.Invalid(result.Messages.FirstOrDefault() ?? "Unable to update direct claims.");
    }
}
