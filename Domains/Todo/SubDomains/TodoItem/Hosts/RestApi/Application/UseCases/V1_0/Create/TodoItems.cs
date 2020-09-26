namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Hosts.RestApi.Application.UseCases.V1_0.Create
{
    using System.Threading.Tasks;
    using CleanDDDArchitecture.Hosts.RestApi.Core;
    using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;
    using TodoItem.Application.UseCases.Create;
    using TodoItem.Application.UseCases.Create.Dtos;

    /// <summary>
    ///     Todo items endpoints
    /// </summary>
    [FeatureGate(Features.TodoItemCreate)]
    public class TodoItems
        : ApiController<TodoItemCreateUseCase, TodoItems>,
          ITodoItemCreateOutput
    {
        /// <summary>
        /// </summary>
        /// <param name="useCase"></param>
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
        /// <response code="200">Todo item already exists</response>
        /// <response code="201">Todo item created successfully</response>
        /// <response code="400">Bad request.</response>
        /// <param name="dto"></param>
        /// <returns>The newly created todo item.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Create))]
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