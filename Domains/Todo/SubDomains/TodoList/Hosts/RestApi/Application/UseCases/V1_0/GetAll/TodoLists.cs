namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Hosts.RestApi.Application.UseCases.V1_0.GetAll
{
    using System.Threading.Tasks;
    using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
    using Microsoft.AspNetCore.Authorization;
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
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            await UseCase.Execute()
               .ConfigureAwait(false);

            return ViewModel;
        }
    }
}