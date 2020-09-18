namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Hosts.RestApi.Application.UseCases.V1_0.Delete
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TodoList.Application.UseCases.Delete;

    /// <summary>
    ///     Todo Lists endpoints
    /// </summary>
    public class TodoLists : ApiController<DeleteTodoListUseCase>, IDeleteTodoUseCaseOutput
    {
        public TodoLists([FromServices] DeleteTodoListUseCase useCase)
            : base(useCase)
        { }

        #region IDeleteTodoUseCaseOutput Members

        void IDeleteTodoUseCaseOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

        #endregion


        /// <summary>
        ///     Delete a todo list
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await UseCase.ExecuteAsync(this, id);

            return ViewModel;
        }
    }
}