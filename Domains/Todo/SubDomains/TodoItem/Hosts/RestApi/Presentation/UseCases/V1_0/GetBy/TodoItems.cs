namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Hosts.RestApi.Presentation.UseCases.V1_0.GetBy;

using System.Net.Mime;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TodoItem.Application.UseCases.GetBy;

/// <inheritdoc
///     cref="CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Hosts.RestApi.Presentation.ApiController{TUseCase,TUseCaseOutput}" />
[FeatureGate(Features.TodoItemGetBy)]
public sealed class TodoItems
    : ApiController<TodoItemGetByUseCase, TodoItems>,
      ITodoItemGetByOutput
{
    /// <inheritdoc />
    public TodoItems([FromServices] TodoItemGetByUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    #region ITodoItemGetByOutput Members

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    void ITodoItemGetByOutput.Invalid(string message) =>
        ViewModel = BadRequest(message);

    /// <summary>
    /// </summary>
    /// <param name="object"></param>
    void ITodoItemGetByOutput.Ok(object? @object) =>
        ViewModel = Ok(@object);

    #endregion


    /// <summary>
    ///     Get a todo item
    /// </summary>
    /// <response code="200">The todo item.</response>
    /// <response code="404">Not Found.</response>
    /// <param name="id"></param>
    /// <returns>An asynchronous <see cref="IActionResult" />.</returns>
    [HttpGet("{id:int}", Name = "GetTodoItem")]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Find))]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetBy([FromRoute] int id)
    {
        await UseCase.ExecuteAsync(new TodoItemGetByInput(id))
           .ConfigureAwait(false);

        return ViewModel;
    }
}
