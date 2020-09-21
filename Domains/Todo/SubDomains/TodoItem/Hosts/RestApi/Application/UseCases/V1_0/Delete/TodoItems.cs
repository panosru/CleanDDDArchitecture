namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Hosts.RestApi.Application.UseCases.V1_0.Delete
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using TodoItem.Application.UseCases.Delete;

    /// <summary>
    ///     Todo items endpoints
    /// </summary>
    public class TodoItems : ApiController<TodoItemDeleteUseCase>, ITodoItemDeleteOutput
    {
        public TodoItems([FromServices] TodoItemDeleteUseCase useCase)
            : base(useCase)
        { }

        #region ITodoItemDeleteOutput Members

        void ITodoItemDeleteOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

        #endregion


        /// <summary>
        ///     Delete a todo item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await UseCase.ExecuteAsync(this, id)
               .ConfigureAwait(false);

            return ViewModel;
        }
    }
}