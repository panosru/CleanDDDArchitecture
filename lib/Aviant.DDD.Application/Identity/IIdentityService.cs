namespace Aviant.DDD.Application.Identity
{
    using System;
    using System.Threading.Tasks;

    public interface IIdentityService
    {
        Task<object> Authenticate(string username, string password);

        Task<IdentityResult> ConfirmEmail(string toekn, string email);

        Task<string> GetUserNameAsync(Guid userId);

        Task<(IdentityResult Result, Guid UserId)> CreateUserAsync(string username, string password);

        Task<IdentityResult> DeleteUserAsync(Guid userId);
    }
}