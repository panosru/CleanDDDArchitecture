using System.Net.Mime;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Update;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Hosts.RestApi.Presentation.UseCases.V1_0.Update;

/// <inheritdoc
///     cref="CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Hosts.RestApi.Presentation.ApiController{TUseCase,TUseCaseOutput}" />
[FeatureGate(Features.TodoListUpdate)]
public sealed class TodoLists
    : ApiController<UpdateTodoListUseCase, TodoLists>,
      IUpdateTodoListOutput
{
    /// <inheritdoc />
    public TodoLists([FromServices] UpdateTodoListUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    #region IUpdateTodoListOutput Members

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    void IUpdateTodoListOutput.Invalid(string message) =>
        ViewModel = BadRequest(message);

    #endregion


    /// <summary>
    ///     Update a todo list
    /// </summary>
    /// <response code="204">Todo list updated successfully.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="404">Not Found.</response>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns>updated todo list data.</returns>
    [HttpPut("{id:int}")]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Update))]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Update(
        [FromRoute] int               id,
        [FromBody]  UpdateTodoListDto dto)
    {
        if (id != dto.Id)
            return BadRequest();

        await UseCase.ExecuteAsync(new UpdateTodoListInput(dto.Id, dto.Title))
           .ConfigureAwait(false);

        return ViewModel;
    }
}
