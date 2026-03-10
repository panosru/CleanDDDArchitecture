namespace CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

public sealed class MfaSetupResult
{
    public string SharedKey { get; set; } = string.Empty;

    public string AuthenticatorUri { get; set; } = string.Empty;

    public bool IsEnabled { get; set; }
}
