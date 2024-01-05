namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Hosts.RestApi.Presentation.UseCases.V1_0.Delete;

using System.Net.Mime;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TodoItem.Application.UseCases.Delete;

/// <inheritdoc
///     cref="CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Hosts.RestApi.Presentation.ApiController{TUseCase,TUseCaseOutput}" />
[FeatureGate(Features.TodoItemDelete)]
public sealed class TodoItems
    : ApiController<TodoItemDeleteUseCase, TodoItems>,
      ITodoItemDeleteOutput
{
    /// <inheritdoc />
    public TodoItems([FromServices] TodoItemDeleteUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    #region ITodoItemDeleteOutput Members

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    void ITodoItemDeleteOutput.Invalid(string message) =>
        ViewModel = BadRequest(message);

    #endregion


    /// <summary>
    ///     Delete a todo item
    /// </summary>
    /// <response code="200">Todo item deleted successfully.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="404">Not Found.</response>
    /// <param name="id"></param>
    /// <returns>The deleted todo item id.</returns>
    [HttpDelete("{id:int}")]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Delete))]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        await UseCase.ExecuteAsync(new TodoItemDeleteInput(id))
           .ConfigureAwait(false);

        return ViewModel;
    }
}
