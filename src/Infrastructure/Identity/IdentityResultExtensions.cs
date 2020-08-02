namespace CleanArchitecture.Infrastructure.Identity
{
    using Microsoft.AspNetCore.Identity;
    using System.Linq;
    using Aviant.DDD.Application;

    public static class IdentityResultExtensions
    {
        public static Result ToApplicationResult(this IdentityResult result)
        {
            return result.Succeeded
                ? Result.Success()
                : Result.Failure(result.Errors.Select(e => e.Description));
        }
    }
}