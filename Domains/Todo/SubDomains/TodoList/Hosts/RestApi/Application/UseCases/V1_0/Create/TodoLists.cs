namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Hosts.RestApi.Application.UseCases.V1_0.Create
{
    using System.Threading.Tasks;
    using CleanDDDArchitecture.Hosts.RestApi.Core;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
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
        /// <summary>
        /// </summary>
        /// <param name="useCase"></param>
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
        /// <response code="200">Todo list already exists</response>
        /// <response code="201">Todo list created successfully</response>
        /// <response code="400">Bad request.</response>
        /// <param name="dto"></param>
        /// <returns>The newly created todo list.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Create))]
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