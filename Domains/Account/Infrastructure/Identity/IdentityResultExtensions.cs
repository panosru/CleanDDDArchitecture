namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Identity
{
    #region

    using System.Linq;
    using Aviant.DDD.Application.Identity;

    #endregion

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