using System.ComponentModel.DataAnnotations;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.MfaVerify;

public sealed class MfaVerifyDto
{
    [Required]
    public string Code { get; set; } = string.Empty;
}
