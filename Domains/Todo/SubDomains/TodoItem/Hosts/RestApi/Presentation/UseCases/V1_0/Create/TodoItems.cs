namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Hosts.RestApi.Presentation.UseCases.V1_0.Create;

using System.Net.Mime;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TodoItem.Application.UseCases.Create;

/// <inheritdoc
///     cref="CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Hosts.RestApi.Presentation.ApiController{TUseCase,TUseCaseOutput}" />
[FeatureGate(Features.TodoItemCreate)]
public sealed class TodoItems
    : ApiController<TodoItemCreateUseCase, TodoItems>,
      ITodoItemCreateOutput
{
    /// <inheritdoc />
    public TodoItems([FromServices] TodoItemCreateUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    #region ITodoItemCreateOutput Members

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    void ITodoItemCreateOutput.Invalid(string message) =>
        ViewModel = BadRequest(message);

    /// <summary>
    /// </summary>
    /// <param name="object"></param>
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
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Create([FromBody] TodoItemCreateDto dto)
    {
        await UseCase.ExecuteAsync(new TodoItemCreateInput(dto.ListId, dto.Title))
           .ConfigureAwait(false);

        return ViewModel;
    }
}
