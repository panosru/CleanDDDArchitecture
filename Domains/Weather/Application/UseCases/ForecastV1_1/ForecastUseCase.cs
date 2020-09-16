namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.ForecastV1_1
{
    #region

    using System.Linq;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;

    #endregion

    public class ForecastUseCase : UseCase<IForecastOutput>
    {
        protected override async Task Execute()
        {
            RequestResult requestResult = await Orchestrator.SendQuery(new GetWeatherForecastsQueryNew());

            if (requestResult.Succeeded)
                Output.Ok(requestResult.Payload());
            else
                Output.Invalid(requestResult.Messages.First());
        }
    }
}