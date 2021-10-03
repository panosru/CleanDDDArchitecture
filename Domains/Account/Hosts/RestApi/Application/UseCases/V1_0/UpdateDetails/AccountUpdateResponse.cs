// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Application.UseCases.V1_0.UpdateDetails
{
    using Domains.Account.Application.Aggregates;

    /// <summary>
    /// </summary>
    internal readonly struct AccountUpdateResponse
    {
        /// <summary>
        /// </summary>
        /// <param name="accountAggregate"></param>
        public AccountUpdateResponse(AccountAggregate accountAggregate)
        {
            FirstName = accountAggregate.FirstName;
            LastName  = accountAggregate.LastName;
            Email     = accountAggregate.Email;
        }

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
