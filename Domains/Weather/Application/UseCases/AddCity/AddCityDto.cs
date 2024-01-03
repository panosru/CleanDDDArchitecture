// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.ComponentModel.DataAnnotations;

namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.AddCity;

public struct AddCityDto
{
    [Required]
    public string City { get; set; }
}