// ReSharper disable MemberCanBeInternal

#pragma warning disable 8618

namespace CleanDDDArchitecture.Domains.Account.Application.Identity
{
    using Aviant.DDD.Application.Identity;

    public sealed class AccountUser : ApplicationUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}