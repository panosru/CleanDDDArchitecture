namespace CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

public sealed class ExternalAuthenticationStartDto
{
    public string Provider { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string AuthorizationUrl { get; set; } = string.Empty;

    public DateTimeOffset ExpiresAtUtc { get; set; }
}
