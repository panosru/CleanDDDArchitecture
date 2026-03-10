using System.ComponentModel.DataAnnotations;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ChangeEmailConfirm;

public sealed class ChangeEmailConfirmDto
{
    [Required]
    public string CurrentEmail { get; set; } = string.Empty;

    [Required]
    public string NewEmail { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;
}
