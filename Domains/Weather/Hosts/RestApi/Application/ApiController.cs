namespace CleanDDDArchitecture.Domains.Weather.Hosts.RestApi.Application
{
    using Aviant.DDD.Application.UseCases;

    public class ApiController : CleanDDDArchitecture.Hosts.RestApi.Core.Controllers.ApiController
    { }
    
    public class ApiController<TUseCase> : CleanDDDArchitecture.Hosts.RestApi.Core.Controllers.ApiController<TUseCase>
        where TUseCase : class, IUseCase
    {
        public ApiController(TUseCase useCase)
            : base(useCase)
        { }
    }
}