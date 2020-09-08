namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Application
{
    using Account.Application.Aggregates;
    using Aviant.DDD.Application.Orchestration;
    using ApiControllerCore = CleanDDDArchitecture.Hosts.RestApi.Core.Controllers.ApiController;
    using Microsoft.Extensions.DependencyInjection;
    
    public abstract class ApiController :  ApiControllerCore
    {
        private IOrchestrator<AccountEntity, AccountId>? _orchestrator;

        protected new IOrchestrator<AccountEntity, AccountId> Orchestrator =>
            _orchestrator ??= HttpContext.RequestServices
               .GetService<IOrchestrator<AccountEntity, AccountId>>();
    }
}