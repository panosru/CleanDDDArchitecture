namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ConfirmEmail
{
    using System.ComponentModel.DataAnnotations;

    public struct ConfirmEmailDto
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string Email { get; set; }
    }
}