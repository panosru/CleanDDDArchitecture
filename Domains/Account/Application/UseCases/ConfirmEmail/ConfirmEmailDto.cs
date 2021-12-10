// ReSharper disable UnusedAutoPropertyAccessor.Global

#pragma warning disable 8618

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ConfirmEmail;

using System.ComponentModel.DataAnnotations;

public sealed class ConfirmEmailDto
{
    [Required]
    public string Token { get; set; }

    [Required]
    public string Email { get; set; }
}