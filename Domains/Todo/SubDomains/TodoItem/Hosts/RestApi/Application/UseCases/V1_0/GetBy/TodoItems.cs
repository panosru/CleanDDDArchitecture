namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Hosts.RestApi.Application.UseCases.V1_0.GetBy
{
    using System.Threading.Tasks;
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
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
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