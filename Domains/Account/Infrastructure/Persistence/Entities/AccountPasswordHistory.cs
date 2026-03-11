using CleanDDDArchitecture.Domains.Account.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Entities;

public sealed class AccountPasswordHistory
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public AccountUser User { get; set; } = null!;

    public string PasswordHash { get; set; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; set; }
}
