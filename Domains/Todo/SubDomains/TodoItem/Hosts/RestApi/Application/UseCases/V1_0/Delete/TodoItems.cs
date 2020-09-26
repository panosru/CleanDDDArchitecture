namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Hosts.RestApi.Application.UseCases.V1_0.Delete
{
    using System.Threading.Tasks;
    using CleanDDDArchitecture.Hosts.RestApi.Core;
    using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;
    using TodoItem.Application.UseCases.Delete;

    /// <summary>
    ///     Todo items endpoints
    /// </summary>
    [FeatureGate(Features.TodoItemDelete)]
    public class TodoItems
        : ApiController<TodoItemDeleteUseCase, TodoItems>,
          ITodoItemDeleteOutput
    {
        public TodoItems([FromServices] TodoItemDeleteUseCase useCase)
            : base(useCase) => UseCase.SetOutput(this);

        #region ITodoItemDeleteOutput Members

        void ITodoItemDeleteOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

        #endregion


        /// <summary>
        ///     Delete a todo item
        /// </summary>
        /// <response code="200">Todo item deleted successfully.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Not Found.</response>
        /// <param name="id"></param>
        /// <returns>The deleted todo item id.</returns>
        [HttpDelete("{id:int}")]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Delete))]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await UseCase.Execute(new TodoItemDeleteInput(id))
               .ConfigureAwait(false);

            return ViewModel;
        }
    }
}