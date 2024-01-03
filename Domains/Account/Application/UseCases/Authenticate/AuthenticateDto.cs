// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.ComponentModel.DataAnnotations;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Authenticate;

public struct AuthenticateDto
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}