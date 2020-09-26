namespace CleanDDDArchitecture.Domains.Weather.Hosts.RestApi.Application
{
    using Aviant.DDD.Application.UseCases;

    public class ApiController : CleanDDDArchitecture.Hosts.RestApi.Core.Controllers.ApiController
    { }

    public class ApiController<TUseCase, TUseCaseOutput>
        : CleanDDDArchitecture.Hosts.RestApi.Core.Controllers.ApiController<TUseCase, TUseCaseOutput>
        where TUseCase : class, IUseCase<TUseCaseOutput>
        where TUseCaseOutput : class, IUseCaseOutput
    {
        public ApiController(TUseCase useCase)
            : base(useCase)
        { }
    }
}