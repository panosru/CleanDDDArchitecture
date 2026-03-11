namespace CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

public sealed class ExternalAuthenticationResultDto
{
    public string Provider { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool IsLinked { get; set; }

    public bool IsCreated { get; set; }

    public AuthResult? Tokens { get; set; }
}
