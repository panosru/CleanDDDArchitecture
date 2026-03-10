using System.ComponentModel.DataAnnotations;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.MfaDisable;

public sealed class MfaDisableDto
{
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;
}
