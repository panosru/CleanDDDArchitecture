namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Hosts.RestApi.Application.UseCases.V1_0.Create
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TodoList.Application.UseCases.Create;

    /// <summary>
    ///     Todo Lists endpoints
    /// </summary>
    [AllowAnonymous]
    public class TodoLists
        : ApiController<CreateTodoListUseCase, TodoLists>,
          ICreateTodoListOutput
    {
        public TodoLists([FromServices] CreateTodoListUseCase useCase)
            : base(useCase) => UseCase.SetOutput(this);

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
        public async Task<IActionResult> Create([FromBody] CreateTodoListDto dto)
        {
            await UseCase
               .SetInput(dto)
               .Execute()
               .ConfigureAwait(false);

            return ViewModel;
        }
    }
}