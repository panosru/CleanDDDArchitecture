namespace CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

public sealed class AccountTrustedDeviceDto
{
    public Guid Id { get; set; }

    public string? DeviceName { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime ExpiresAtUtc { get; set; }

    public DateTime? LastUsedAtUtc { get; set; }

    public string? CreatedByIp { get; set; }

    public string? UserAgent { get; set; }
}
