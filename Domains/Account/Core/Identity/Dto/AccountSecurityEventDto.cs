namespace CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

public sealed class AccountSecurityEventDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid? ActorUserId { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTimeOffset OccurredAtUtc { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }
}
