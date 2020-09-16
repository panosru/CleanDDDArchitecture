namespace CleanDDDArchitecture.Domains.Weather.Hosts.RestApi.Application.UseCases.V1_0.AddCity
{
    public class AddCityResponse
    {
        public AddCityResponse(string city) => 
            Message = $"City \"{city}\" has added";

        public string Message { get; }

        public override string ToString() => Message;
    }
}