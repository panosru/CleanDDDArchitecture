namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create.Dtos
{
    #region

    using System.ComponentModel.DataAnnotations;

    #endregion

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