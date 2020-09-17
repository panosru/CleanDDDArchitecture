namespace CleanDDDArchitecture.Domains.Account.Application.Identity
{
    using Aviant.DDD.Application.Identity;

    public class AccountUser : ApplicationUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}