namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.AddCity
{
    using System.ComponentModel.DataAnnotations;

    public struct AddCityDto
    {
        [Required]
        public string City { get; set; }
    }
}