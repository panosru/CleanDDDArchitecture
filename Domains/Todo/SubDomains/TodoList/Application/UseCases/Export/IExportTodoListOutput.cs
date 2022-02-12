namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Export;

using Aviant.Foundation.Application.UseCases;

public interface IExportTodoListOutput : IUseCaseOutput
{
    public void Ok(ExportTodosVm exportTodosVm);

    public void Invalid(byte[] bytes);
}
