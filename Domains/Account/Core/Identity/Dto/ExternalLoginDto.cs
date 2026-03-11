namespace CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

public sealed class ExternalLoginDto
{
    public string Provider { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;
}
