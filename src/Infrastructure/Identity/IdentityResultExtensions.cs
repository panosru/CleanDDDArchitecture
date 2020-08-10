namespace CleanDDDArchitecture.Infrastructure.Identity
{
    using System.Linq;
    using Aviant.DDD.Application;
    using Aviant.DDD.Application.Identity;
    using Microsoft.AspNetCore.Identity;
    using IdentityResult = Aviant.DDD.Application.Identity.IdentityResult;

    public static class IdentityResultExtensions
    {
        public static IdentityResult ToApplicationResult(this Microsoft.AspNetCore.Identity.IdentityResult result)
        {
            return result.Succeeded
                ? IdentityResult.Success()
                : IdentityResult.Failure(result.Errors.Select(e => e.Description));
        }
    }
}