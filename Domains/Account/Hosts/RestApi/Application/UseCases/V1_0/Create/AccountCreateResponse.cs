// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Application.UseCases.V1_0.Create
{
    using Domains.Account.Application.Aggregates;

    /// <summary>
    /// </summary>
    internal readonly struct AccountCreateResponse
    {
        /// <summary>
        /// </summary>
        /// <param name="accountAggregate"></param>
        public AccountCreateResponse(AccountAggregate accountAggregate)
        {
            Username  = accountAggregate.UserName;
            FirstName = accountAggregate.FirstName;
            LastName  = accountAggregate.LastName;
            Email     = accountAggregate.Email;
            Message   = $"Welcome {FirstName}! Don't forget to confirm you email!";
        }

        /// <summary>
        /// </summary>
        public string Message { get; }

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
