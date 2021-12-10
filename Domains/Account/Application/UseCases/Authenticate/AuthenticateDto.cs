// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Authenticate;

using System.ComponentModel.DataAnnotations;

public struct AuthenticateDto
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}