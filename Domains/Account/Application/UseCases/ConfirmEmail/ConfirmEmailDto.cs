// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.ComponentModel.DataAnnotations;

#pragma warning disable 8618

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ConfirmEmail;

public sealed class ConfirmEmailDto
{
    [Required]
    public string Token { get; set; }

    [Required]
    public string Email { get; set; }
}