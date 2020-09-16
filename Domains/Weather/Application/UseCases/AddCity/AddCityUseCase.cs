namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.AddCity
{
    #region

    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;

    #endregion

    public class AddCityUseCase : UseCase<AddCityInput, IAddCityOutput>
    {
        protected override async Task Execute()
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
                        Output.Ok(requestResult.Payload());
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

        protected override void SetInput<TInputData>(TInputData data)
        {
            if (!(data is AddCityDto dto))
                throw new TypeAccessException(
                    $"Expected type \"{nameof(AddCityDto)}\", but \"{data.GetType().Name}\" found instead.");

            Input = new AddCityInput(dto.City);
        }
    }
}