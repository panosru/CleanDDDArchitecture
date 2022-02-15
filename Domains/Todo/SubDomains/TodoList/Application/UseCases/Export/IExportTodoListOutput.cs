namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Export;

using Aviant.Application.UseCases;

public interface IExportTodoListOutput : IUseCaseOutput
{
    public void Ok(ExportTodosVm exportTodosVm);

    public void Invalid(byte[] bytes);
}
