using CleanDDDArchitecture.Domains.Account.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Entities;

public sealed class AccountTrustedDevice
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public AccountUser User { get; set; } = null!;

    public string TokenHash { get; set; } = string.Empty;

    public string? DeviceName { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset ExpiresAtUtc { get; set; }

    public DateTimeOffset? LastUsedAtUtc { get; set; }

    public DateTimeOffset? RevokedAtUtc { get; set; }

    public string? CreatedByIp { get; set; }

    public string? UserAgent { get; set; }
}
