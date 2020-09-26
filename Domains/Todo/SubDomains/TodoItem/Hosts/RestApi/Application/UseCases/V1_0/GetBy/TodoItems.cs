namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Hosts.RestApi.Application.UseCases.V1_0.GetBy
{
    using System.Threading.Tasks;
    using CleanDDDArchitecture.Hosts.RestApi.Core;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using TodoItem.Application.UseCases.GetBy;

    /// <summary>
    ///     Todo items endpoints
    /// </summary>
    public class TodoItems
        : ApiController<TodoItemGetByUseCase, TodoItems>,
          ITodoItemGetByOutput
    {
        public TodoItems([FromServices] TodoItemGetByUseCase useCase)
            : base(useCase) => UseCase.SetOutput(this);

        #region ITodoItemGetByOutput Members

        void ITodoItemGetByOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

        void ITodoItemGetByOutput.Ok(object? @object) =>
            ViewModel = Ok(@object);

        #endregion


        /// <summary>
        ///     Get a todo item
        /// </summary>
        /// <response code="200">The todo item.</response>
        /// <response code="404">Not Found.</response>
        /// <param name="id"></param>
        /// <returns>An asynchronous <see cref="IActionResult" />.</returns>
        [HttpGet("{id:int}", Name = "GetTodoItem")]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Find))]
        public async Task<IActionResult> GetBy([FromRoute] int id)
        {
            await UseCase
               .SetInput(id)
               .Execute()
               .ConfigureAwait(false);

            return ViewModel;
        }
    }
}