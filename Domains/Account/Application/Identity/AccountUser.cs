// ReSharper disable MemberCanBeInternal

using Aviant.Application.Identity;

#pragma warning disable 8618

namespace CleanDDDArchitecture.Domains.Account.Application.Identity;

public sealed class AccountUser : ApplicationUser
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public AccountStatus Status { get; set; } = AccountStatus.Active;

    public string? StatusReason { get; set; }

    public DateTimeOffset? StatusChangedAtUtc { get; set; }
    
    public string FullName => $"{FirstName} {LastName}";
}
