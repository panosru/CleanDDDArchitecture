namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Identity;

using Aviant.DDD.Application.Identity;

internal static class IdentityResultExtensions
{
    public static IdentityResult ToApplicationResult(this Microsoft.AspNetCore.Identity.IdentityResult result)
    {
        return result.Succeeded
            ? IdentityResult.Success()
            : IdentityResult.Failure(result.Errors.Select(e => e.Description));
    }
}
