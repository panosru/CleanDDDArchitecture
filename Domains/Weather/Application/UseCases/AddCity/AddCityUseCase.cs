namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.AddCity
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;

    public sealed class AddCityUseCase
        : UseCase<AddCityInput, IAddCityOutput>
    {
        public override async Task ExecuteAsync(
            AddCityInput      input,
            CancellationToken cancellationToken = default)
        {
            switch (input.City)
            {
                case "Athens":
                    OrchestratorResponse requestResult = await Orchestrator.SendCommandAsync(
                            new AddCityCommand(input.City),
                            cancellationToken)
                       .ConfigureAwait(false);

                    if (requestResult.Succeeded)
                        Output.Ok(requestResult.Payload<string>());
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
    }
}
