using Aviant.Application.Identity;
using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminUpdateRoles;

public sealed class AdminUpdateRolesUseCase : UseCase<AdminUpdateRolesInput, IAdminUpdateRolesOutput>
{
    public override async Task ExecuteAsync(
        AdminUpdateRolesInput input,
        CancellationToken cancellationToken = default)
    {
        var result = await Orchestrator
            .SendCommandAsync(new AdminUpdateRolesCommand(input.AccountId, input.Roles), cancellationToken)
            .ConfigureAwait(false);

        if (result.Succeeded && result.Payload() is IdentityResult { Succeeded: true })
        {
            Output.Ok();
            return;
        }

        Output.Invalid(result.Messages.FirstOrDefault() ?? "Unable to update role assignments.");
    }
}
