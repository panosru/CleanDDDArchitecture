namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Application.UseCases.V1_0.Create
{
    using Domains.Account.Application.Aggregates;

    public class AccountCreateResponse
    {
        public AccountCreateResponse(AccountAggregate accountAggregate)
        {
            Username  = accountAggregate.UserName;
            FirstName = accountAggregate.FirstName;
            LastName  = accountAggregate.LastName;
            Email     = accountAggregate.Email;
            Message   = $"Welcome {FirstName}! Don't forget to confirm you email!";
        }

        public string Message { get; }

        public string Username { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string Email { get; }
    }
}