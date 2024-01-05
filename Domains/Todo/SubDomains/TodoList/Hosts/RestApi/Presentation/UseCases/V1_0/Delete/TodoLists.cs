using System.Net.Mime;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Delete;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Hosts.RestApi.Presentation.UseCases.V1_0.Delete;

/// <inheritdoc
///     cref="CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Hosts.RestApi.Presentation.ApiController{TUseCase,TUseCaseOutput}" />
[FeatureGate(Features.TodoListDelete)]
public sealed class TodoLists
    : ApiController<DeleteTodoListUseCase, TodoLists>,
      IDeleteTodoUseCaseOutput
{
    /// <inheritdoc />
    public TodoLists([FromServices] DeleteTodoListUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    #region IDeleteTodoUseCaseOutput Members

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    void IDeleteTodoUseCaseOutput.Invalid(string message) =>
        ViewModel = BadRequest(message);

    #endregion


    /// <summary>
    ///     Delete a todo list
    /// </summary>
    /// <response code="200">Todo list deleted successfully.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="404">Not Found.</response>
    /// <param name="id"></param>
    /// <returns>The deleted todo list id.</returns>
    [HttpDelete("{id:int}")]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Delete))]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        await UseCase.ExecuteAsync(new DeleteTodoListInput(id))
           .ConfigureAwait(false);

        return ViewModel;
    }
}
