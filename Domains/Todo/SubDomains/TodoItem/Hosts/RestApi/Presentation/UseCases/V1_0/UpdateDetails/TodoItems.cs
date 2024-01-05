namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Hosts.RestApi.Presentation.UseCases.V1_0.UpdateDetails;

using System.Net.Mime;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TodoItem.Application.UseCases.UpdateDetails;

/// <inheritdoc
///     cref="CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Hosts.RestApi.Presentation.ApiController{TUseCase,TUseCaseOutput}" />
[FeatureGate(Features.TodoItemUpdateDetails)]
public sealed class TodoItems
    : ApiController<TodoItemUpdatedetailsUseCase, TodoItems>,
      ITodoItemUpdateDetailsOutput
{
    /// <inheritdoc />
    public TodoItems([FromServices] TodoItemUpdatedetailsUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    #region ITodoItemUpdateDetailsOutput Members

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
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
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> UpdateDetails(
        [FromQuery] int                      id,
        [FromBody]  TodoItemUpdateDetailsDto dto)
    {
        if (id != dto.Id)
            return BadRequest();

        await UseCase.ExecuteAsync(
                new TodoItemUpdateDetailsInput(
                    dto.Id,
                    dto.ListId,
                    dto.Priority,
                    dto.Note))
           .ConfigureAwait(false);

        return ViewModel;
    }
}
