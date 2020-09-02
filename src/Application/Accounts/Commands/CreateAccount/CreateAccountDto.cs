namespace CleanDDDArchitecture.Application.Accounts.Commands.CreateAccount
{
    using System.ComponentModel.DataAnnotations;

    public class CreateAccountDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }
    }
}