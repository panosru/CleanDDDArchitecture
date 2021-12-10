// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.UpdateDetails;

using System.ComponentModel.DataAnnotations;

public struct UpdateAccountDto
{
    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    public string Email { get; set; }
}