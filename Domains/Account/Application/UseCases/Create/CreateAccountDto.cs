namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create
{
    using System.ComponentModel.DataAnnotations;

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

        [Required]
        public string Roles { get; set; }

        public bool EmailConfirmed { get; set; } // default: false
    }
}