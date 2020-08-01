using System;
using System.Threading.Tasks;

namespace Aviant.DDD.Application
{
    public interface IIdentityService
    {
        Task<object> Authenticate(string username, string password);

        Task<Result> ConfirmEmail(string toekn, string email);

        Task<string> GetUserNameAsync(Guid userId);

        Task<(Result Result, Guid UserId)> CreateUserAsync(string username, string password);

        Task<Result> DeleteUserAsync(Guid userId);
    }
}