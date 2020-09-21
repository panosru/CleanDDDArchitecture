namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Hosts.RestApi.Application.UseCases.V1_0.ExportList
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TodoList.Application.UseCases.Export;
    using TodoList.Application.UseCases.Export.ViewModels;

    /// <summary>
    ///     Export todo list items into csv file
    /// </summary>
    public class TodoLists : ApiController<ExportTodoListUseCase>, IExportTodoListOutput
    {
        public TodoLists([FromServices] ExportTodoListUseCase useCase)
            : base(useCase)
        { }

        #region IExportTodoListOutput Members

        void IExportTodoListOutput.Invalid(byte[] bytes) =>
            ViewModel = File(bytes, "text/plain", "Error.txt");

        void IExportTodoListOutput.Ok(ExportTodosVm exportTodosVm) =>
            ViewModel = File(exportTodosVm.Content, exportTodosVm.ContentType, exportTodosVm.FileName);

        #endregion


        /// <summary>
        ///     Get all todo lists with their items
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Export([FromRoute] int id)
        {
            await UseCase.ExecuteAsync(this, id)
               .ConfigureAwait(false);

            return ViewModel;
        }
    }
}