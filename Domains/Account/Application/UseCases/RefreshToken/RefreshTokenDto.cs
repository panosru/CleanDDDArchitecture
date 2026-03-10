using System.ComponentModel.DataAnnotations;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.RefreshToken;

public sealed class RefreshTokenDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
