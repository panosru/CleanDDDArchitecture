namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Hosts.RestApi.Application.UseCases.V1_0.Update
{
    using System.Threading.Tasks;
    using CleanDDDArchitecture.Hosts.RestApi.Core;
    using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;
    using TodoItem.Application.UseCases.Update;

    /// <summary>
    ///     Todo items endpoints
    /// </summary>
    [FeatureGate(Features.TodoItemUpdate)]
    public class TodoItems
        : ApiController<TodoItemUpdateUseCase, TodoItems>,
          ITodoItemUpdateOutput
    {
        /// <summary>
        /// </summary>
        /// <param name="useCase"></param>
        public TodoItems([FromServices] TodoItemUpdateUseCase useCase)
            : base(useCase) => UseCase.SetOutput(this);

        #region ITodoItemUpdateOutput Members

        void ITodoItemUpdateOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

        #endregion


        /// <summary>
        ///     Update todo item name and status
        /// </summary>
        /// <response code="204">Todo item updated</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Not Found.</response>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns>Updated todo item data.</returns>
        [HttpPut("{id:int}")]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Update))]
        public async Task<IActionResult> Update(
            [FromRoute] int               id,
            [FromBody]  TodoItemUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            await UseCase.Execute(new TodoItemUpdateInput(dto.Id, dto.Title, dto.Done))
               .ConfigureAwait(false);

            return ViewModel;
        }
    }
}