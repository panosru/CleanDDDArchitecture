namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Export
{
    using Aviant.DDD.Application.UseCases;
    using ViewModels;

    public interface IExportTodoListOutput : IUseCaseOutput
    {
        public void Ok(ExportTodosVm exportTodosVm);

        public void Invalid(byte[] bytes);
    }
}