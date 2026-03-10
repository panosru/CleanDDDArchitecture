using CleanDDDArchitecture.Domains.Account.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Entities;

public sealed class AccountRefreshSession
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public AccountUser User { get; set; } = null!;

    public string TokenHash { get; set; } = string.Empty;

    public Guid TokenFamilyId { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime ExpiresAtUtc { get; set; }

    public DateTime? LastUsedAtUtc { get; set; }

    public DateTime? RotatedAtUtc { get; set; }

    public DateTime? RevokedAtUtc { get; set; }

    public Guid? ReplacedBySessionId { get; set; }

    public string? CreatedByIp { get; set; }

    public string? UserAgent { get; set; }

    public string? Reason { get; set; }
}
