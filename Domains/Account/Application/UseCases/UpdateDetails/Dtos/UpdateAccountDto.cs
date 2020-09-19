namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.UpdateDetails.Dtos
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    public class UpdateAccountDto
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }
    }
}