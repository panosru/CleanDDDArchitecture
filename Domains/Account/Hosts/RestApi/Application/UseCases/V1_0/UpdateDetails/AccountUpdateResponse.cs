namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Application.UseCases.V1_0.UpdateDetails
{
    using Domains.Account.Application.Aggregates;

    public class AccountUpdateResponse
    {
        public AccountUpdateResponse(AccountAggregate accountAggregate)
        {
            FirstName = accountAggregate.FirstName;
            LastName  = accountAggregate.LastName;
            Email     = accountAggregate.Email;
        }

        public string FirstName { get; }

        public string LastName { get; }

        public string Email { get; }
    }
}