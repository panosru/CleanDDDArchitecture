namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Application.UseCases.V1_0.GetBy
{
    using Domains.Account.Application.Identity;

    public class AccountGetByResponse
    {
        public AccountGetByResponse(AccountUser accountUser)
        {
            Username  = accountUser.UserName;
            FirstName = accountUser.FirstName;
            LastName  = accountUser.LastName;
            Email     = accountUser.Email;
        }

        public string Username { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string Email { get; }
    }
}