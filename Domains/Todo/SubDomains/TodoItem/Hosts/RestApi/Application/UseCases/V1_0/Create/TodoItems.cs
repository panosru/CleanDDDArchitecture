namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Hosts.RestApi.Application.UseCases.V1_0.Create
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using TodoItem.Application.UseCases.Create;
    using TodoItem.Application.UseCases.Create.Dtos;

    /// <summary>
    ///     Todo items endpoints
    /// </summary>
    public class TodoItems
        : ApiController<TodoItemCreateUseCase, TodoItems>,
          ITodoItemCreateOutput
    {
        public TodoItems([FromServices] TodoItemCreateUseCase useCase)
            : base(useCase) => UseCase.SetOutput(this);

        #region ITodoItemCreateOutput Members

        void ITodoItemCreateOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

        void ITodoItemCreateOutput.Ok(object? @object) =>
            ViewModel = Ok(@object);

        #endregion


        /// <summary>
        ///     Create a new todo item
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TodoItemCreateDto dto)
        {
            await UseCase
               .SetInput(dto)
               .Execute()
               .ConfigureAwait(false);

            return ViewModel;
        }
    }
}