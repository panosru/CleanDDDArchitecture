using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Export;

public interface IExportTodoListOutput : IUseCaseOutput
{
    public void Ok(ExportTodosVm exportTodosVm);

    public void Invalid(byte[] bytes);
}
