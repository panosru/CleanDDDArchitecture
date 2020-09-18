namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Hosts.RestApi.Application.UseCases.V1_0.Create
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TodoList.Application.UseCases.Create;
    using TodoList.Application.UseCases.Create.Dtos;

    /// <summary>
    ///     Todo Lists endpoints
    /// </summary>
    public class TodoLists : ApiController<CreateTodoListUseCase>, ICreateTodoListOutput
    {
        public TodoLists([FromServices] CreateTodoListUseCase useCase)
            : base(useCase)
        { }

        #region ICreateTodoListOutput Members

        void ICreateTodoListOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

        void ICreateTodoListOutput.Ok(object? @object) =>
            ViewModel = Ok(@object);

        #endregion


        /// <summary>
        ///     Create new todo list
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] TodoListDto dto)
        {
            await UseCase.ExecuteAsync(this, dto);

            return ViewModel;
        }
    }
}