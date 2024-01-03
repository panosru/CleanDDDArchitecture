using System.ComponentModel.DataAnnotations;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create;

public struct CreateAccountDto
{
    [Required]
    public string UserName { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    public string Email { get; set; }
}