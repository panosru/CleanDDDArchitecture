namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Hosts.RestApi.Application.UseCases.V1_0.UpdateDetails
{
    using System.Threading.Tasks;
    using CleanDDDArchitecture.Hosts.RestApi.Core;
    using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;
    using TodoItem.Application.UseCases.UpdateDetails;

    /// <summary>
    ///     Todo items endpoints
    /// </summary>
    [FeatureGate(Features.TodoItemUpdateDetails)]
    public class TodoItems
        : ApiController<TodoItemUpdatedetailsUseCase, TodoItems>,
          ITodoItemUpdateDetailsOutput
    {
        /// <summary>
        /// </summary>
        /// <param name="useCase"></param>
        public TodoItems([FromServices] TodoItemUpdatedetailsUseCase useCase)
            : base(useCase) => UseCase.SetOutput(this);

        #region ITodoItemUpdateDetailsOutput Members

        void ITodoItemUpdateDetailsOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

        #endregion


        /// <summary>
        ///     Update todo item details
        /// </summary>
        /// <response code="200">Todo item details updated successfully.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Not Found.</response>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns>Todo item with updated details.</returns>
        [HttpPut("[action]/{id:int}")]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Patch))]
        public async Task<IActionResult> UpdateDetails(
            [FromQuery] int                      id,
            [FromBody]  TodoItemUpdateDetailsDto dto)
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