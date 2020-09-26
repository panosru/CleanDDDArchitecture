namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.UpdateDetails.Dtos
{
    using System.ComponentModel.DataAnnotations;

    public class UpdateAccountDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }
    }
}