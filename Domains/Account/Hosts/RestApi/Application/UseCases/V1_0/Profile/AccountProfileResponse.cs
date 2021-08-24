// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Application.UseCases.V1_0.Profile
{
    using Domains.Account.Application.Identity;

    /// <summary>
    /// </summary>
    internal readonly struct AccountProfileResponse
    {
        /// <summary>
        /// </summary>
        /// <param name="accountUser"></param>
        public AccountProfileResponse(AccountUser accountUser)
        {
            Username  = accountUser.UserName;
            FirstName = accountUser.FirstName;
            LastName  = accountUser.LastName;
            Email     = accountUser.Email;
        }

        /// <summary>
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// </summary>
        public string FirstName { get; }

        /// <summary>
        /// </summary>
        public string LastName { get; }

        /// <summary>
        /// </summary>
        public string Email { get; }
    }
}