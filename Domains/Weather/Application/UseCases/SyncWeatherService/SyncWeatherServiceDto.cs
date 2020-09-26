namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.SyncWeatherService
{
    using System.ComponentModel.DataAnnotations;

    public struct SyncWeatherServiceDto
    {
        [Required]
        public string City { get; set; }
    }
}