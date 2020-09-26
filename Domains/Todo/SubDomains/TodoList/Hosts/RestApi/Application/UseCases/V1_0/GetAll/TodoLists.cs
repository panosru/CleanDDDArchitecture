namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Hosts.RestApi.Application.UseCases.V1_0.GetAll
{
    using System.Threading.Tasks;
    using CleanDDDArchitecture.Hosts.RestApi.Core;
    using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;
    using TodoList.Application.UseCases.GetAll;

    /// <summary>
    ///     Todo Lists endpoints
    /// </summary>
    [FeatureGate(Feature.TodoListGetAll)]
    [AllowAnonymous]
    public class TodoLists
        : ApiController<GetAllUseCase, TodoLists>,
          IGetAllOutput
    {
        public TodoLists([FromServices] GetAllUseCase useCase)
            : base(useCase) => UseCase.SetOutput(this);

        #region IGetAllOutput Members

        void IGetAllOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

        void IGetAllOutput.Ok(object? @object) =>
            ViewModel = Ok(@object);

        #endregion


        /// <summary>
        ///     Get all todo lists with their items
        /// </summary>
        /// <response code="200">The list of all todo lists with their items.</response>
        /// <response code="404">Not Found.</response>
        /// <returns>An asynchronous <see cref="IActionResult" />.</returns>
        [HttpGet]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.List))]
        public async Task<IActionResult> GetAll()
        {
            await UseCase.Execute()
               .ConfigureAwait(false);

            return ViewModel;
        }
    }
}