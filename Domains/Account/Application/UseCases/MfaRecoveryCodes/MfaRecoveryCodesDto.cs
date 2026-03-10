using System.ComponentModel.DataAnnotations;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.MfaRecoveryCodes;

public sealed class MfaRecoveryCodesDto
{
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;
}
