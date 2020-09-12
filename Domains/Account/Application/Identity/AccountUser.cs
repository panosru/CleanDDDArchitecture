namespace CleanDDDArchitecture.Domains.Account.Application.Identity
{
    #region

    using Aviant.DDD.Application.Identity;

    #endregion

    public class AccountUser : ApplicationUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}