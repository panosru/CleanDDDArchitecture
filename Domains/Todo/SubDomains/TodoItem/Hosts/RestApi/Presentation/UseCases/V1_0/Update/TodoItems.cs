namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Hosts.RestApi.Presentation.UseCases.V1_0.Update;

using System.Net.Mime;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TodoItem.Application.UseCases.Update;

/// <inheritdoc
///     cref="CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Hosts.RestApi.Presentation.ApiController{TUseCase,TUseCaseOutput}" />
[FeatureGate(Features.TodoItemUpdate)]
public sealed class TodoItems
    : ApiController<TodoItemUpdateUseCase, TodoItems>,
      ITodoItemUpdateOutput
{
    /// <inheritdoc />
    public TodoItems([FromServices] TodoItemUpdateUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    #region ITodoItemUpdateOutput Members

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    void ITodoItemUpdateOutput.Invalid(string message) =>
        ViewModel = BadRequest(message);

    /// <summary>
    /// </summary>
    /// <param name="object"></param>
    void ITodoItemUpdateOutput.Ok(object? @object) =>
        ViewModel = Ok(@object);

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
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Update(
        [FromRoute] int               id,
        [FromBody]  TodoItemUpdateDto dto)
    {
        if (id != dto.Id)
            return BadRequest();

        await UseCase.ExecuteAsync(new TodoItemUpdateInput(dto.Id, dto.Title, dto.Done))
           .ConfigureAwait(false);

        return ViewModel;
    }
}
