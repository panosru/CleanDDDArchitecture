﻿namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Hosts.RestApi.Application.UseCases.V1_0.ExportList
{
    using System.Threading.Tasks;
    using CleanDDDArchitecture.Hosts.RestApi.Core;
    using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;
    using TodoList.Application.UseCases.Export;

    /// <inheritdoc
    ///     cref="CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Hosts.RestApi.Application.ApiController&lt;TUseCase,TUseCaseOutput&gt;" />
    [AllowAnonymous]
    [FeatureGate(Features.TodoListExportList)]
    public sealed class TodoLists
        : ApiController<ExportTodoListUseCase, TodoLists>,
          IExportTodoListOutput
    {
        /// <inheritdoc />
        public TodoLists([FromServices] ExportTodoListUseCase useCase)
            : base(useCase) => UseCase.SetOutput(this);

        #region IExportTodoListOutput Members

        /// <summary>
        /// </summary>
        /// <param name="bytes"></param>
        void IExportTodoListOutput.Invalid(byte[] bytes) =>
            ViewModel = File(bytes, "text/plain", "Error.txt");

        /// <summary>
        /// </summary>
        /// <param name="exportTodosVm"></param>
        void IExportTodoListOutput.Ok(ExportTodosVm exportTodosVm) =>
            ViewModel = File(exportTodosVm.Content, exportTodosVm.ContentType, exportTodosVm.FileName);

        #endregion


        /// <summary>
        ///     Get all todo lists with their items
        /// </summary>
        /// <response code="200">Todo list exported successfully.</response>
        /// <response code="404">Not Found.</response>
        /// <returns>Returns todo list with items in csv file.</returns>
        [HttpGet("{id:int}")]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Get))]
        public async Task<IActionResult> Export([FromRoute] int id)
        {
            await UseCase.ExecuteAsync(new ExportTodoListInput(id))
               .ConfigureAwait(false);

            return ViewModel;
        }
    }
}