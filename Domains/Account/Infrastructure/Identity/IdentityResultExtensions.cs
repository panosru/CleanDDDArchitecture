using Aviant.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Identity;

internal static class IdentityResultExtensions
{
    public static IdentityResult ToApplicationResult(this Microsoft.AspNetCore.Identity.IdentityResult result)
    {
        return result.Succeeded
            ? IdentityResult.Success()
            : IdentityResult.Failure(result.Errors.Select(e => e.Description));
    }
}
