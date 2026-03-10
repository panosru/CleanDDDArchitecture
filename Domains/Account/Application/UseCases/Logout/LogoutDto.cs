using System.ComponentModel.DataAnnotations;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Logout;

public sealed class LogoutDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
