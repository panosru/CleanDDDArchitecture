namespace CleanDDDArchitecture.Domains.Weather.Hosts.RestApi.Application
{
    using Aviant.DDD.Application.UseCases;

    /// <inheritdoc />
    /// <summary>
    ///     Weather endpoints
    /// </summary>
    public abstract class ApiController : CleanDDDArchitecture.Hosts.RestApi.Core.Controllers.ApiController
    { }

    /// <inheritdoc />
    /// <summary>
    ///     Weather endpoints
    /// </summary>
    public abstract class ApiController<TUseCase, TUseCaseOutput>
        : CleanDDDArchitecture.Hosts.RestApi.Core.Controllers.ApiController<TUseCase, TUseCaseOutput>
        where TUseCase : class, IUseCase<TUseCaseOutput>
        where TUseCaseOutput : class, IUseCaseOutput
    {
        /// <inheritdoc />
        protected ApiController(TUseCase useCase)
            : base(useCase)
        { }
    }
}