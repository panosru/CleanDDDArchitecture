namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.AddCity
{
    using System.Linq;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;

    public class AddCityUseCase
        : UseCase<AddCityInput, IAddCityOutput>
    {
        public override async Task Execute()
        {
            switch (Input.City)
            {
                case "Athens":
                    RequestResult requestResult = await Orchestrator.SendCommand(
                        new AddCityCommand
                        {
                            City = Input.City
                        });

                    if (requestResult.Succeeded)
                        Output.Ok(requestResult.Payload()?.ToString());
                    else
                        Output.Invalid(requestResult.Messages.First());
                    break;

                case "Atlantis":
                    Output.NotFound();
                    break;

                default:
                    Output.Invalid("Wrong city!");
                    break;
            }
        }

        public AddCityUseCase SetInput(AddCityDto dto)
        {
            Input = new AddCityInput(dto.City);

            return this;
        }
    }
}