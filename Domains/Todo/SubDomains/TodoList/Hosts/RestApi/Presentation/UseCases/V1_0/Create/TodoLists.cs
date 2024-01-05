using System.Net.Mime;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Hosts.RestApi.Presentation.UseCases.V1_0.Create;

/// <inheritdoc
///     cref="CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Hosts.RestApi.Presentation.ApiController{TUseCase,TUseCaseOutput}" />
[FeatureGate(Features.TodoListCreate)]
public sealed class TodoLists
    : ApiController<CreateTodoListUseCase, TodoLists>,
      ICreateTodoListOutput
{
    /// <inheritdoc />
    public TodoLists([FromServices] CreateTodoListUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    #region ICreateTodoListOutput Members

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    void ICreateTodoListOutput.Invalid(string message) =>
        ViewModel = BadRequest(message);

    /// <summary>
    /// </summary>
    /// <param name="object"></param>
    void ICreateTodoListOutput.Ok(object? @object) =>
        ViewModel = Ok(@object);

    #endregion


    /// <summary>
    ///     Create new todo list
    /// </summary>
    /// <response code="200">Todo list already exists</response>
    /// <response code="201">Todo list created successfully</response>
    /// <response code="400">Bad request.</response>
    /// <param name="dto"></param>
    /// <returns>The newly created todo list.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Create))]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Create([FromBody] CreateTodoListDto dto)
    {
        await UseCase.ExecuteAsync(new CreateTodoListInput(dto.Title))
           .ConfigureAwait(false);

        return ViewModel;
    }
}
