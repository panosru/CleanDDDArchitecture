using System.Net.Mime;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.GetAll;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Hosts.RestApi.Presentation.UseCases.V1_0.GetAll;

/// <inheritdoc
///     cref="CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Hosts.RestApi.Presentation.ApiController{TUseCase,TUseCaseOutput}" />
[FeatureGate(Features.TodoListGetAll)]
public sealed class TodoLists
    : ApiController<GetAllUseCase, TodoLists>,
      IGetAllOutput
{
    /// <inheritdoc />
    public TodoLists([FromServices] GetAllUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    #region IGetAllOutput Members

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    void IGetAllOutput.Invalid(string message) =>
        ViewModel = BadRequest(message);

    /// <summary>
    /// </summary>
    /// <param name="object"></param>
    void IGetAllOutput.Ok(object? @object) =>
        ViewModel = Ok(@object);

    #endregion


    /// <summary>
    ///     Get all todo lists with their items
    /// </summary>
    /// <response code="200">The list of all todo lists with their items.</response>
    /// <response code="404">Not Found.</response>
    /// <returns>An asynchronous <see cref="IActionResult" />.</returns>
    [HttpGet]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.List))]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        await UseCase.ExecuteAsync(cancellationToken)
           .ConfigureAwait(false);

        return ViewModel;
    }
}
