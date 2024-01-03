// ReSharper disable MemberCanBeInternal

using Aviant.Application.Identity;

#pragma warning disable 8618

namespace CleanDDDArchitecture.Domains.Account.Application.Identity;

public sealed class AccountUser : ApplicationUser
{
    public string FirstName { get; set; }

    public string LastName { get; set; }
}
