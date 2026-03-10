namespace CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

public sealed class MfaRecoveryCodesResult
{
    public IReadOnlyCollection<string> RecoveryCodes { get; set; } = Array.Empty<string>();
}
