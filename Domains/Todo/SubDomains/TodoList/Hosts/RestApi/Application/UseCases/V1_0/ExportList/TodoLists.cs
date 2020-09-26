namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Hosts.RestApi.Application.UseCases.V1_0.ExportList
{
    using System.Threading.Tasks;
    using CleanDDDArchitecture.Hosts.RestApi.Core;
    using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;
    using TodoList.Application.UseCases.Export;
    using TodoList.Application.UseCases.Export.ViewModels;

    /// <summary>
    ///     Export todo list items into csv file
    /// </summary>
    [AllowAnonymous]
    [FeatureGate(Features.TodoListExportList)]
    public class TodoLists
        : ApiController<ExportTodoListUseCase, TodoLists>,
          IExportTodoListOutput
    {
        /// <summary>
        /// </summary>
        /// <param name="useCase"></param>
        public TodoLists([FromServices] ExportTodoListUseCase useCase)
            : base(useCase) => UseCase.SetOutput(this);

        #region IExportTodoListOutput Members

        void IExportTodoListOutput.Invalid(byte[] bytes) =>
            ViewModel = File(bytes, "text/plain", "Error.txt");

        void IExportTodoListOutput.Ok(ExportTodosVm exportTodosVm) =>
            ViewModel = File(exportTodosVm.Content, exportTodosVm.ContentType, exportTodosVm.FileName);

        #endregion


        /// <summary>
        ///     Get all todo lists with their items
        /// </summary>
        /// <response code="200">Todo list expored successfully.</response>
        /// <response code="404">Not Found.</response>
        /// <returns>Returns todo list with items in csv file.</returns>
        [HttpGet("{id:int}")]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Get))]
        public async Task<IActionResult> Export([FromRoute] int id)
        {
            await UseCase
               .SetInput(id)
               .Execute()
               .ConfigureAwait(false);

            return ViewModel;
        }
    }
}