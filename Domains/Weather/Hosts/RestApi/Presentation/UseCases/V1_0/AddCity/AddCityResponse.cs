namespace CleanDDDArchitecture.Domains.Weather.Hosts.RestApi.Presentation.UseCases.V1_0.AddCity;

/// <summary>
/// </summary>
internal struct AddCityResponse
{
    /// <summary>
    /// </summary>
    /// <param name="city"></param>
    public AddCityResponse(string city) =>
        Message = $"City \"{city}\" has added";

    /// <summary>
    /// </summary>
    private string Message { get; }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Message;
}