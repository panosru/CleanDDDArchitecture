using CleanDDDArchitecture.Domains.Account.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Entities;

public sealed class AccountPhoneVerification
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public AccountUser User { get; set; } = null!;

    public string PhoneNumber { get; set; } = string.Empty;

    public string CodeHash { get; set; } = string.Empty;

    public string Purpose { get; set; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset ExpiresAtUtc { get; set; }

    public DateTimeOffset? ConsumedAtUtc { get; set; }

    public string? CreatedByIp { get; set; }
}
