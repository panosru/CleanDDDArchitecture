namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Hosts.RestApi.Application.UseCases.V1_0.Update
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using TodoItem.Application.UseCases.Update;
    using TodoItem.Application.UseCases.Update.Dtos;

    /// <summary>
    ///     Todo items endpoints
    /// </summary>
    public class TodoItems
        : ApiController<TodoItemUpdateUseCase, TodoItems>,
          ITodoItemUpdateOutput
    {
        public TodoItems([FromServices] TodoItemUpdateUseCase useCase)
            : base(useCase) => UseCase.SetOutput(this);

        #region ITodoItemUpdateOutput Members

        void ITodoItemUpdateOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

        #endregion


        /// <summary>
        ///     Update todo item name and status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            [FromRoute] int               id,
            [FromBody]  TodoItemUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            await UseCase
               .SetInput(dto)
               .Execute()
               .ConfigureAwait(false);

            return ViewModel;
        }
    }
}