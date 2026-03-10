namespace CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

public sealed class MfaChallengeResult
{
    public bool RequiresTwoFactor { get; set; } = true;

    public string ChallengeType { get; set; } = "totp";
}
